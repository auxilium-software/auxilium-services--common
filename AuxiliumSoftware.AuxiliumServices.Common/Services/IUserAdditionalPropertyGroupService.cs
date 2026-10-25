using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services
{
    public interface IUserAdditionalPropertyGroupService
    {
        Task<List<UserAdditionalPropertyGroupEntityModel>> GetGroupsAsync(CancellationToken ct = default);
        Task<UserAdditionalPropertyGroupEntityModel?> GetGroupAsync(Guid groupId, CancellationToken ct = default);

        Task<UserAdditionalPropertyGroupEntityModel> CreateGroupAsync(
            Guid actorUserId,
            string canonicalName,
            IReadOnlyDictionary<string, string>? translations = null,
            CancellationToken ct = default
        );

        Task RenameGroupAsync(Guid actorUserId, Guid groupId, string newCanonicalName, CancellationToken ct = default);
        Task ReorderGroupsAsync(Guid actorUserId, IReadOnlyList<Guid> orderedGroupIds, CancellationToken ct = default);
        Task<int> DeleteGroupAsync(Guid actorUserId, Guid groupId, CancellationToken ct = default);

        Task SetTranslationAsync(Guid actorUserId, Guid groupId, string languageCode, string translation, CancellationToken ct = default);
        Task RemoveTranslationAsync(Guid actorUserId, Guid groupId, string languageCode, CancellationToken ct = default);
    }

}
