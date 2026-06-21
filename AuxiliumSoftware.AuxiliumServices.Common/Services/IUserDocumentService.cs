using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services
{
    public interface IUserDocumentService
    {
        Task<UserEntityModel?> GetDocumentAsync(
            Guid userId
        );
        Task SaveDocumentAsync(
            UserEntityModel userDoc
        );





        Task<List<UserAdditionalPropertyEntityModel>> GetAdditionalPropertiesAsync(
            Guid userId
        );
        Task<Guid> SaveAdditionalPropertyAsync(
            UserEntityModel currentUser,
            Guid userId,
            string additionalPropertyDisplayName,
            string additionalPropertyContent,
            string contentType
        );
        Task UpdateAdditionalPropertyAsync(
            Guid userId,
            Guid additionalPropertyId,
            string content,
            string contentType,
            Guid actorUserId
        );
        Task DeleteAdditionalPropertyAsync(
            Guid userId,
            Guid additionalPropertyId,
            Guid actorUserId
        );



        bool CheckUserAccess(
            Guid userId,
            UserEntityModel currentUser
        );



        Task WriteToAuditLog(
            UserEntityModel currentUser,
            UserEntityModel targetUser, AuditLogActionTypeEnum actionType,
            UserEntityTypeEnum entityType, Guid? entityId = null,
            string? propertyName = null, string? oldValue = null, string? newValue = null,
            CancellationToken ct = default
        );
    }
}
