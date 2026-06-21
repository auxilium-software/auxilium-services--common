using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services
{
    public interface ICaseDocumentService
    {
        Task<CaseEntityModel?> GetDocumentAsync(Guid caseId);
        Task SaveDocumentAsync(CaseEntityModel caseDoc);
        Task<bool> CheckUserAccessAsync(Guid caseId, UserEntityModel currentUser);





        Task AddClientAsync(Guid caseId, Guid userId);
        Task RemoveClientAsync(Guid caseId, Guid userId);
        Task AddWorkerAsync(Guid caseId, Guid userId);
        Task RemoveWorkerAsync(Guid caseId, Guid userId);





        Task<List<CaseAdditionalPropertyEntityModel>> GetAdditionalPropertiesAsync(Guid caseId);
        Task<Guid> SaveAdditionalPropertyAsync(UserEntityModel currentUser, Guid userId, string additionalPropertyDisplayName, string content, string contentType);
        Task DeleteAdditionalPropertyAsync(Guid caseId, Guid additionalPropertyId);





        Task<CaseTimelineEntryEntityModel> CreateTimelineEntryAsync(
            Guid caseId,
            DateTime occuredAt,
            string title,
            string description,
            Guid createdBy
        );
        Task<CaseTimelineEntryEntityModel?> GetTimelineEntryAsync(Guid caseId, Guid timelineEntryId);
        Task<List<CaseTimelineEntryEntityModel>> GetTimelineEntriesAsync(Guid caseId);
        Task UpdateTimelineEntryAsync(
            Guid caseId,
            Guid timelineEntryId,
            DateTime? occuredAt = null,
            string? title = null,
            string? description = null,
            Guid? updatedBy = null
        );
        Task DeleteTimelineEntryAsync(Guid caseId, Guid timelineEntryId);





        Task<CaseTodoEntityModel> CreateTodoAsync(
            Guid caseId,
            string summary,
            string? description,
            TodoPriorityEnum priority,
            Guid createdBy,
            DateTime? dueDate = null,
            Guid? assignedTo = null,
            DateTime? reminder = null);
        Task<CaseTodoEntityModel?> GetTodoAsync(Guid caseId, Guid todoId);
        Task<List<CaseTodoEntityModel>> GetTodosAsync(Guid caseId);
        Task UpdateTodoStatusAsync(
            Guid caseId,
            Guid todoId,
            TodoStatusEnum status,
            Guid? completedBy = null,
            string? completionNotes = null);
        Task UpdateTodoAsync(
            Guid caseId,
            Guid todoId,
            string? summary = null,
            string? description = null,
            TodoPriorityEnum? priority = null,
            DateTime? dueDate = null,
            Guid? assignedTo = null,
            DateTime? reminder = null);
        Task DeleteTodoAsync(Guid caseId, Guid todoId);





        Task WriteToAuditLog(
            UserEntityModel currentUser,
            CaseEntityModel targetCase, CaseEntityTypeEnum entityType, Guid entityId,
            AuditLogActionTypeEnum actionType,
            string? propertyName = null, string? oldValue = null, string? newValue = null,
            CancellationToken ct = default
        );
    }
}
