using AuxiliumSoftware.AuxiliumServices.Common.Configuration;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Services;
using AuxiliumSoftware.AuxiliumServices.Common.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services.Implementations;

public class CaseDocumentService : ICaseDocumentService
{
    private readonly ISystemSettingsService _systemSettings;
    private readonly AuxiliumDbContext _db;
    private readonly ILogger<CaseDocumentService> _logger;

    public CaseDocumentService(
        ISystemSettingsService systemSettings,
        AuxiliumDbContext db,
        ILogger<CaseDocumentService> logger
    )
    {
        this._systemSettings = systemSettings;
        this._db = db;
        this._logger = logger;
    }

    #region ========================= CASE DOCUMENT OPERATIONS =========================
    public async Task<CaseEntityModel?> GetDocumentAsync(Guid caseId)
    {
        return await _db.Cases
            .Include(c => c.Workers)
            .Include(c => c.Clients)
            .Include(c => c.AdditionalProperties)
            .Include(c => c.Messages)
            .Include(c => c.Files)
            .Include(c => c.Todos)
            .FirstOrDefaultAsync(c => c.Id == caseId);
    }

    public async Task SaveDocumentAsync(CaseEntityModel caseDoc)
    {
        caseDoc.LastUpdatedAtUtc = DateTime.UtcNow;

        // check if the entity is tracked
        var entry = _db.Entry(caseDoc);
        if (entry.State == EntityState.Detached)
        {
            _db.Cases.Update(caseDoc);
        }

        await _db.SaveChangesAsync();
    }

    #endregion
    #region ========================= PEOPLE MANAGEMENT =========================
    public async Task AddClientAsync(Guid caseId, Guid userId)
    {
        try
        {
            // check if the relationship already exists
            var exists = await _db.CaseClients
                .AnyAsync(cc => cc.CaseId == caseId && cc.UserId == userId);

            if (exists)
            {
                _logger.LogInformation("Client {UserId} already assigned to case {CaseId}", userId, caseId);
                return;
            }

            // add the new client relationship to the case
            var caseClient = new CaseClientEntityModel
            {
                Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.Case_Client),
                CaseId = caseId,
                UserId = userId,
                CreatedByUserId = userId, // TODO: pass through current user
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.CaseClients.Add(caseClient);

            // update the LastUpdatedAt timestamp for the case
            var caseEntity = await _db.Cases.FindAsync(caseId);
            if (caseEntity != null)
            {
                caseEntity.LastUpdatedAtUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Added client {UserId} to case {CaseId}", userId, caseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add client {UserId} to case {CaseId}", userId, caseId);
            throw;
        }
    }

    public async Task RemoveClientAsync(Guid caseId, Guid userId)
    {
        try
        {
            var caseClient = await _db.CaseClients
                .FirstOrDefaultAsync(cc => cc.CaseId == caseId && cc.UserId == userId);

            if (caseClient != null)
            {
                _db.CaseClients.Remove(caseClient);

                // update the LastUpdatedAt timestamp for the case
                var caseEntity = await _db.Cases.FindAsync(caseId);
                if (caseEntity != null)
                {
                    caseEntity.LastUpdatedAtUtc = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync();

                _logger.LogInformation("Removed client {UserId} from case {CaseId}", userId, caseId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove client {UserId} from case {CaseId}", userId, caseId);
            throw;
        }
    }

    public async Task AddWorkerAsync(Guid caseId, Guid userId)
    {
        try
        {
            // check if the relationship already exists
            var exists = await _db.CaseWorkers
                .AnyAsync(cw => cw.CaseId == caseId && cw.UserId == userId);

            if (exists)
            {
                _logger.LogInformation("Worker {UserId} already assigned to case {CaseId}", userId, caseId);
                return;
            }

            // add the new worker to the case
            var caseWorker = new CaseWorkerEntityModel
            {
                Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.Case_Worker),
                CaseId = caseId,
                UserId = userId,
                CreatedByUserId = userId, // TODO: pass through current user
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.CaseWorkers.Add(caseWorker);

            // update the LastUpdatedAt timestamp for the case
            var caseEntity = await _db.Cases.FindAsync(caseId);
            if (caseEntity != null)
            {
                caseEntity.LastUpdatedAtUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Added worker {UserId} to case {CaseId}", userId, caseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add worker {UserId} to case {CaseId}", userId, caseId);
            throw;
        }
    }

    public async Task RemoveWorkerAsync(Guid caseId, Guid userId)
    {
        try
        {
            var caseWorker = await _db.CaseWorkers
                .FirstOrDefaultAsync(cw => cw.CaseId == caseId && cw.UserId == userId);

            if (caseWorker != null)
            {
                _db.CaseWorkers.Remove(caseWorker);

                // update the LastUpdatedAt timestamp for the case
                var caseEntity = await _db.Cases.FindAsync(caseId);
                if (caseEntity != null)
                {
                    caseEntity.LastUpdatedAtUtc = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync();

                _logger.LogInformation("Removed worker {UserId} from case {CaseId}", userId, caseId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove worker {UserId} from case {CaseId}", userId, caseId);
            throw;
        }
    }
    #endregion
    #region ========================= ADDITIONAL PROPERTIES =========================
    public async Task<List<CaseAdditionalPropertyEntityModel>> GetAdditionalPropertiesAsync(Guid caseId)
    {
        try
        {
            return await _db.CaseAdditionalProperties
                .Where(a => a.CaseId == caseId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get additional properties for case {CaseId}", caseId);
            throw;
        }
    }

    public async Task SaveAdditionalPropertyAsync(
        UserEntityModel currentUser,
        Guid caseId,
        string additionalPropertyOriginalName,
        string additionalPropertyUrlSlug,
        string additionalPropertyContent,
        string contentType)
    {
        try
        {
            // check if the property already exists
            var existing = await _db.CaseAdditionalProperties
                .FirstOrDefaultAsync(a => a.CaseId == caseId && a.UrlSlug == additionalPropertyUrlSlug);

            if (existing != null)
            {
                throw new Exception($"Additional property {additionalPropertyOriginalName} already exists for case {caseId}");
            }

            var newProperty = new CaseAdditionalPropertyEntityModel
            {
                Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.Case_AdditionalProperty),
                CaseId = caseId,
                ContentType = contentType ?? "text/plain",
                CreatedByUserId = currentUser.Id,
                CreatedAtUtc = DateTime.UtcNow,
                OriginalName = additionalPropertyOriginalName,
                UrlSlug = additionalPropertyUrlSlug,
                Content = additionalPropertyContent,
            };

            _db.CaseAdditionalProperties.Add(newProperty);

            // update the LastUpdatedAt timestamp for the case
            var caseEntity = await _db.Cases.FindAsync(caseId);
            if (caseEntity != null)
            {
                caseEntity.LastUpdatedAtUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Saved property {AdditionalPropertyName} for case {CaseId}", additionalPropertyOriginalName, caseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save property {AdditionalPropertyName} for case {CaseId}", additionalPropertyOriginalName, caseId);
            throw;
        }
    }

    public async Task DeleteAdditionalPropertyAsync(Guid caseId, Guid additionalPropertyId)
    {
        try
        {
            var property = await _db.CaseAdditionalProperties
                .FirstOrDefaultAsync(a => a.CaseId == caseId && a.Id == additionalPropertyId);

            if (property != null)
            {
                _db.CaseAdditionalProperties.Remove(property);

                // update the LastUpdatedAt timestamp for the case
                var caseEntity = await _db.Cases.FindAsync(caseId);
                if (caseEntity != null)
                {
                    caseEntity.LastUpdatedAtUtc = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync();

                _logger.LogInformation("Deleted property {AdditionalPropertyId} from case {CaseId}", additionalPropertyId, caseId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete property {AdditionalPropertyId} from case {CaseId}", additionalPropertyId, caseId);
            throw;
        }
    }
    #endregion
    #region ========================= TODO OPERATIONS =========================
    public async Task<CaseTodoEntityModel> CreateTodoAsync(
        Guid caseId,
        string summary,
        string? description,
        TodoPriorityEnum priority,
        Guid createdBy,
        DateTime? dueDate = null,
        Guid? assignedTo = null,
        DateTime? reminder = null)
    {
        try
        {
            // just make sure that the case actually exists
            var caseExists = await _db.Cases.AnyAsync(c => c.Id == caseId);
            if (!caseExists)
            {
                throw new KeyNotFoundException($"Case {caseId} not found");
            }

            // create the todo entity/model
            var todo = new CaseTodoEntityModel
            {
                Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.Case_Todo),
                CaseId = caseId,
                Summary = summary,
                Description = description ?? string.Empty,
                Status = TodoStatusEnum.NeedsAction,
                Priority = priority,
                CreatedByUserId = createdBy,
                CreatedAtUtc = DateTime.UtcNow,
                DueDate = dueDate,
                AssignedToUserId = assignedTo,
                ReminderUtc = reminder
            };

            _db.CaseTodos.Add(todo);

            // update the LastUpdatedAt timestamp for the case
            var caseEntity = await _db.Cases.FindAsync(caseId);
            if (caseEntity != null)
            {
                caseEntity.LastUpdatedAtUtc = DateTime.UtcNow;
                caseEntity.LastUpdatedByUserId = createdBy;
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Created todo {TodoId} in case {CaseId}", todo.Id, caseId);

            return todo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create todo in case {CaseId}", caseId);
            throw;
        }
    }

    public async Task<CaseTodoEntityModel?> GetTodoAsync(Guid caseId, Guid todoId)
    {
        try
        {
            return await _db.CaseTodos
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Include(t => t.CompletedByUser)
                .FirstOrDefaultAsync(t => t.CaseId == caseId && t.Id == todoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get todo {TodoId} from case {CaseId}", todoId, caseId);
            return null;
        }
    }

    public async Task<List<CaseTodoEntityModel>> GetTodosAsync(Guid caseId)
    {
        try
        {
            return await _db.CaseTodos
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Include(t => t.CompletedByUser)
                .Where(t => t.CaseId == caseId)
                .OrderByDescending(t => t.CreatedAtUtc)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get todos for case {CaseId}", caseId);
            throw;
        }
    }

    public async Task UpdateTodoStatusAsync(
        Guid caseId,
        Guid todoId,
        TodoStatusEnum newStatus,
        Guid? completedBy = null,
        string? completionNotes = null)
    {
        try
        {
            var todo = await _db.CaseTodos
                .FirstOrDefaultAsync(t => t.CaseId == caseId && t.Id == todoId)
                ?? throw new KeyNotFoundException($"Todo {todoId} not found in case {caseId}");

            todo.Status = newStatus;
            todo.LastUpdatedAtUtc = DateTime.UtcNow;

            if (newStatus == TodoStatusEnum.Completed && completedBy != null)
            {
                todo.CompletedAtUtc = DateTime.UtcNow;
                todo.CompletedByUserId = completedBy;
                todo.CompletionNote = completionNotes;
            }

            // update the LastUpdatedAt timestamp for the case
            var caseEntity = await _db.Cases.FindAsync(caseId);
            if (caseEntity != null)
            {
                caseEntity.LastUpdatedAtUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Updated todo {TodoId} status to {Status} in case {CaseId}",
                todoId, newStatus, caseId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update todo {TodoId} status", todoId);
            throw;
        }
    }

    public async Task UpdateTodoAsync(
        Guid caseId,
        Guid todoId,
        string? summary = null,
        string? description = null,
        TodoPriorityEnum? priority = null,
        DateTime? dueDate = null,
        Guid? assignedTo = null,
        DateTime? reminder = null)
    {
        try
        {
            var todo = await _db.CaseTodos
                .FirstOrDefaultAsync(t => t.CaseId == caseId && t.Id == todoId)
                ?? throw new KeyNotFoundException($"Todo {todoId} not found in case {caseId}");

            if (summary != null) todo.Summary = summary;
            if (description != null) todo.Description = description;
            if (priority.HasValue) todo.Priority = priority.Value;
            if (dueDate.HasValue) todo.DueDate = dueDate;
            if (assignedTo.HasValue) todo.AssignedToUserId = assignedTo;
            if (reminder.HasValue) todo.ReminderUtc = reminder;

            todo.LastUpdatedAtUtc = DateTime.UtcNow;

            // update the LastUpdatedAt timestamp for the case
            var caseEntity = await _db.Cases.FindAsync(caseId);
            if (caseEntity != null)
            {
                caseEntity.LastUpdatedAtUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Updated todo {TodoId} in case {CaseId}", todoId, caseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update todo {TodoId}", todoId);
            throw;
        }
    }

    public async Task DeleteTodoAsync(Guid caseId, Guid todoId)
    {
        try
        {
            var todo = await _db.CaseTodos
                .FirstOrDefaultAsync(t => t.CaseId == caseId && t.Id == todoId)
                ?? throw new KeyNotFoundException($"Todo {todoId} not found in case {caseId}");

            _db.CaseTodos.Remove(todo);

            // update the LastUpdatedAt timestamp for the case
            var caseEntity = await _db.Cases.FindAsync(caseId);
            if (caseEntity != null)
            {
                caseEntity.LastUpdatedAtUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Deleted todo {TodoId} from case {CaseId}", todoId, caseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete todo {TodoId}", todoId);
            throw;
        }
    }
    #endregion
    #region ========================= PERMISSION CHECKS =========================
    public async Task<bool> CheckUserAccessAsync(Guid caseId, UserEntityModel currentUser)
    {
        try
        {
            // admins have access to everything
            if (currentUser.IsAdministrator) return true;

            // check whether user is client or worker
            var hasAccess = await _db.Cases
                .Where(c => c.Id == caseId)
                .AnyAsync(c =>
                    c.Clients!.Any(cl => cl.UserId == currentUser.Id) ||
                    c.Workers!.Any(w => w.UserId == currentUser.Id)
                );

            return hasAccess;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check access for user {UserId} on case {CaseId}", currentUser.Id, caseId);
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
        CaseEntityModel targetCase, CaseEntityTypeEnum entityType, Guid entityId,
        AuditLogActionTypeEnum actionType,
        string? propertyName = null, string? oldValue = null, string? newValue = null,
        CancellationToken ct = default
    )
    {
        // verification
        switch(entityType)
        {
            case CaseEntityTypeEnum.Case:
                if (actionType == AuditLogActionTypeEnum.Creation       && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_Cases_LogCreations, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Modification   && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_Cases_LogModifications, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Deletion       && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_Cases_LogDeletions, ct)) return;
                break;
            case CaseEntityTypeEnum.Case_AdditionalProperty:
                if (actionType == AuditLogActionTypeEnum.Creation       && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseAdditionalProperties_LogCreations, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Modification   && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseAdditionalProperties_LogModifications, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Deletion       && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseAdditionalProperties_LogDeletions, ct)) return;
                break;
            case CaseEntityTypeEnum.Case_Worker:
                if (actionType == AuditLogActionTypeEnum.Assignment     && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseWorkers_LogAssignments, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Unassignment   && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseWorkers_LogUnAssignments, ct)) return;
                break;
            case CaseEntityTypeEnum.Case_Client:
                if (actionType == AuditLogActionTypeEnum.Assignment     && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseClients_LogAssignments, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Unassignment   && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseClients_LogUnAssignments, ct)) return;
                break;
            case CaseEntityTypeEnum.Case_Message:
                if (actionType == AuditLogActionTypeEnum.Send           && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseMessages_LogSends, ct)) return;
                if (actionType == AuditLogActionTypeEnum.View           && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseMessages_LogViews, ct)) return;
                break;
            case CaseEntityTypeEnum.Case_File:
                if (actionType == AuditLogActionTypeEnum.Upload         && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseFiles_LogUploads, ct)) return;
                if (actionType == AuditLogActionTypeEnum.View           && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseFiles_LogViews, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Deletion       && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseFiles_LogDeletions, ct)) return;
                break;
            case CaseEntityTypeEnum.Case_Todo:
                if (actionType == AuditLogActionTypeEnum.Creation       && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseTodos_LogCreations, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Modification   && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseTodos_LogModifications, ct)) return;
                if (actionType == AuditLogActionTypeEnum.Deletion       && !await this._systemSettings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_EntityActions_CaseTodos_LogDeletions, ct)) return;
                break;
        }

        // actually logging
        var logEntry = new LogCaseModificationEventEntityModel
        {
            Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.Log_CaseModification_EventEntry),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = currentUser.Id,
            CaseId = targetCase.Id,
            EntityType = entityType,
            EntityId = entityId,
            Action = actionType,
            PropertyName = actionType == AuditLogActionTypeEnum.Modification ? propertyName : null,
            PreviousValue = actionType == AuditLogActionTypeEnum.Modification ? oldValue : null,
            NewValue = actionType == AuditLogActionTypeEnum.Modification ? newValue : null,
        };

        // commiting
        _db.Log_CaseModificationEvents.Add(logEntry);
    }
    #endregion
}
