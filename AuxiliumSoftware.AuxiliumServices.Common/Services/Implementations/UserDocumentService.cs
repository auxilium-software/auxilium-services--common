using AuxiliumSoftware.AuxiliumServices.Common.Configuration;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Services;
using AuxiliumSoftware.AuxiliumServices.Common.Utilities;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services.Implementations;

public class UserDocumentService : IUserDocumentService
{
    private readonly ISystemSettingsService _systemSettings;
    private readonly AuxiliumDbContext _db;
    private readonly ILogger<UserDocumentService> _logger;

    public UserDocumentService(
        ISystemSettingsService systemSettings,
        AuxiliumDbContext db,
        ILogger<UserDocumentService> logger
    )
    {
        this._systemSettings = systemSettings;
        _db = db;
        _logger = logger;
    }

    #region ========================= USER OPERATIONS =========================
    public async Task<UserEntityModel?> GetDocumentAsync(Guid userId)
    {
        try
        {
            return await _db.Users
                .Include(u => u.AdditionalProperties)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user {UserId}", userId);
            return null;
        }
    }

    public async Task SaveDocumentAsync(UserEntityModel userDoc)
    {
        try
        {
            userDoc.LastUpdatedAtUtc = DateTime.UtcNow;

            var entry = _db.Entry(userDoc);
            if (entry.State == EntityState.Detached)
            {
                _db.Users.Update(userDoc);
            }

            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save user {UserId}", userDoc.Id);
            throw;
        }
    }
    #endregion
    #region ========================= ADDITIONAL PROPERTIES =========================
    public async Task<List<UserAdditionalPropertyEntityModel>> GetAdditionalPropertiesAsync(Guid caseId)
    {
        try
        {
            return await _db.UserAdditionalProperties
                .Where(a => a.UserId == caseId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get additional properties for case {CaseId}", caseId);
            throw;
        }
    }

    public async Task<Guid> SaveAdditionalPropertyAsync(
        UserEntityModel currentUser,
        Guid caseId,
        string additionalPropertyDisplayName,
        string additionalPropertyContent,
        string contentType)
    {
        try
        {
            var newProperty = new CaseAdditionalPropertyEntityModel
            {
                Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.Case_AdditionalProperty),
                CaseId = caseId,
                ContentType = contentType ?? "text/plain",
                CreatedByUserId = currentUser.Id,
                CreatedAtUtc = DateTime.UtcNow,
                DisplayName = additionalPropertyDisplayName,
                Content = additionalPropertyContent,
            };

            _db.CaseAdditionalProperties.Add(newProperty);

            var caseEntity = await _db.Cases.FindAsync(caseId);
            if (caseEntity != null)
            {
                caseEntity.LastUpdatedAtUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Saved property {AdditionalPropertyId} ({Name}) for case {CaseId}",
                newProperty.Id, additionalPropertyDisplayName, caseId);

            return newProperty.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save property {AdditionalPropertyName} for case {CaseId}",
                additionalPropertyDisplayName, caseId);
            throw;
        }
    }

    public async Task DeleteAdditionalPropertyAsync(Guid userId, Guid additionalPropertyId)
    {
        try
        {
            var property = await _db.UserAdditionalProperties
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == additionalPropertyId);

            if (property != null)
            {
                _db.UserAdditionalProperties.Remove(property);

                // update the LastUpdatedAt timestamp for the case
                var userEntity = await _db.Users.FindAsync(userId);
                if (userEntity != null)
                {
                    userEntity.LastUpdatedAtUtc = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync();

                _logger.LogInformation("Deleted property {AdditionalPropertyId} from case {;}", additionalPropertyId, userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete property {AdditionalPropertyId} from case {;}", additionalPropertyId, userId);
            throw;
        }
    }
    #endregion
    #region ========================= PERMISSION CHECKS =========================
    public bool CheckUserAccess(Guid userId, UserEntityModel currentUser)
    {
        try
        {
            // admins can access absolutely everything
            if (currentUser.IsAdministrator) return true;

            // users can access their own data
            if (currentUser.Id == userId) return true;

            // case workers can view other users (for assigning to cases)
            if (currentUser.IsCaseWorker) return true;

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check access for user {UserId}", userId);
            return false;
        }
    }
    #endregion
    #region ========================= AUDIT LOGGING OPERATIONS =========================
    /// <summary>
    /// This method writes an entry to the audit log for case-related actions.
    /// IT WILL NOT SAVE CHANGES TO THE DATABASE - CALLER MUST COMMIT CHANGES.
    /// </summary>
    /// <param name="currentUser">The currently logged in User // the User that did the Action.</param>
    /// <param name="targetCase">The Case the Action that is being Audit Logged is for.</param>
    /// <param name="entityType">The type of Case-related Entity that is being Audit Logged.</param>
    /// <param name="entityId">The unique identifier for the Case-related Entity.</param>
    /// <param name="actionType">The type of Action that is being Audit Logged.</param>
    /// <param name="propertyName">This value is MANDATORY when actionType is set to `Modification` - it specifies the target Property that has been Modified.</param>
    /// <param name="oldValue">This value is MANDATORY when actionType is set to `Modification` - it specifies the old value of the Property that has been Modified.</param>
    /// <param name="newValue">This value is MANDATORY when actionType is set to `Modification` - it specifies the new value of the Property that has been Modified.</param>
    public async Task WriteToAuditLog(
        UserEntityModel currentUser,
        UserEntityModel targetUser, UserEntityTypeEnum entityType, Guid entityId,
        AuditLogActionTypeEnum actionType,
        string? propertyName = null, string? oldValue = null, string? newValue = null,
        CancellationToken ct = default
    )
    {
        // verification
        switch (entityType)
        {
            case UserEntityTypeEnum.User:
                if (actionType == AuditLogActionTypeEnum.Creation       && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_Users_LogCreations, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Modification   && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_Users_LogModifications, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Deletion       && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_Users_LogDeletions, ct)) return;
                break;
            case UserEntityTypeEnum.User_AdditionalProperty:
                if (actionType == AuditLogActionTypeEnum.Creation       && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_UserAdditionalProperties_LogCreations, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Modification   && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_UserAdditionalProperties_LogModifications, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Deletion       && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_UserAdditionalProperties_LogDeletions, ct)) return;
                break;
            case UserEntityTypeEnum.User_File:
                if (actionType == AuditLogActionTypeEnum.Upload         && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_UserFiles_LogUploads, ct)) return;
                if (actionType == AuditLogActionTypeEnum.View           && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_UserFiles_LogViews, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Deletion       && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_UserFiles_LogDeletions, ct)) return;
                break;
        }

        // actually logging
        var logEntry = new LogUserModificationEventEntityModel
        {
            Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.Log_UserModification_EventEntry),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = currentUser.Id,
            UserId = targetUser.Id,
            EntityType = entityType,
            EntityId = entityId,
            Action = actionType,
            PropertyName = actionType == AuditLogActionTypeEnum.Modification ? propertyName : null,
            PreviousValue = actionType == AuditLogActionTypeEnum.Modification ? oldValue : null,
            NewValue = actionType == AuditLogActionTypeEnum.Modification ? newValue : null,
        };

        // commiting
        _db.Log_UserModificationEvents.Add(logEntry);
    }
    #endregion
}
