using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services
{
    public interface ICaseDocumentService
    {
        Task<CaseEntityModel?> GetDocumentAsync(
            Guid caseId
        );
        Task SaveDocumentAsync(
            CaseEntityModel caseDoc
        );





        Task AddClientAsync(
            Guid caseId,
            Guid userId,
            Guid actorUserId
        );
        Task RemoveClientAsync(
            Guid caseId,
            Guid userId,
            Guid actorUserId
        );
        Task AddWorkerAsync(
            Guid caseId,
            Guid userId,
            Guid actorUserId
        );
        Task RemoveWorkerAsync(
            Guid caseId,
            Guid userId,
            Guid actorUserId
        );





        Task<List<CaseAdditionalPropertyEntityModel>> GetAdditionalPropertiesAsync(
            Guid caseId
        );
        Task<Guid> SaveAdditionalPropertyAsync(
            UserEntityModel currentUser,
            Guid caseId,
            string additionalPropertyDisplayName,
            string additionalPropertyContent,
            string contentType
        );
        Task UpdateAdditionalPropertyAsync(
            Guid caseId,
            Guid additionalPropertyId,
            string content,
            string contentType,
            Guid actorUserId
        );
        Task DeleteAdditionalPropertyAsync(
            Guid caseId,
            Guid additionalPropertyId,
            Guid actorUserId
        );





        Task<CaseTodoEntityModel> CreateTodoAsync(
            Guid caseId,
            string summary,
            string? description,
            TodoPriorityEnum priority,
            Guid createdBy,
            DateTime? dueDate = null,
            Guid? assignedTo = null,
            DateTime? reminder = null
        );
        Task<CaseTodoEntityModel?> GetTodoAsync(
            Guid caseId,
            Guid todoId
        );
        Task<List<CaseTodoEntityModel>> GetTodosAsync(
            Guid caseId
        );
        Task UpdateTodoStatusAsync(
            Guid caseId,
            Guid todoId,
            Guid actorUserId,
            TodoStatusEnum newStatus,
            Guid? completedBy = null,
            string? completionNotes = null
        );
        Task UpdateTodoAsync(
            Guid caseId,
            Guid todoId,
            Guid actorUserId,
            string? summary = null,
            string? description = null,
            TodoPriorityEnum? priority = null,
            DateTime? dueDate = null,
            Guid? assignedTo = null,
            DateTime? reminder = null
        );
        Task DeleteTodoAsync(
            Guid caseId,
            Guid todoId,
            Guid actorUserId
        );





        Task<CaseTimelineEntryEntityModel> CreateTimelineEntryAsync(
            Guid caseId,
            DateTime occuredAt,
            string title,
            string description,
            Guid createdBy
        );
        Task<CaseTimelineEntryEntityModel?> GetTimelineEntryAsync(
            Guid caseId,
            Guid timelineEntryId
        );
        Task<List<CaseTimelineEntryEntityModel>> GetTimelineEntriesAsync(
            Guid caseId
        );
        Task UpdateTimelineEntryAsync(
            Guid caseId,
            Guid timelineEntryId,
            DateTime? occuredAt,
            Guid updatedBy,
            string? title = null,
            string? description = null
        );
        Task DeleteTimelineEntryAsync(
            Guid caseId,
            Guid timelineEntryId,
            Guid actorUserId
        );





        Task<bool> CheckUserAccessAsync(
            Guid caseId,
            UserEntityModel currentUser
        );





        Task WriteToAuditLog(
            Guid actorUserId,
            Guid caseId, AuditLogActionTypeEnum actionType,
            CaseEntityTypeEnum entityType, Guid? entityId = null,
            string? propertyName = null, string? oldValue = null, string? newValue = null,
            CancellationToken ct = default
        );
    }
}
