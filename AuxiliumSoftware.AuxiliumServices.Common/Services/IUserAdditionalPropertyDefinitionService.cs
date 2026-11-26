using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services
{
    public interface IUserAdditionalPropertyDefinitionService
    {
        Task<List<UserAdditionalPropertyDefinitionEntityModel>> GetDefinitionsAsync(bool includeInactive = true, CancellationToken ct = default);
        Task<UserAdditionalPropertyDefinitionEntityModel?> GetDefinitionAsync(Guid definitionId, CancellationToken ct = default);

        Task<UserAdditionalPropertyDefinitionEntityModel> CreateDefinitionAsync(
            Guid actorUserId,
            string canonicalName,
            AdditionalPropertyDataTypeEnum dataType,
            string? description = null,
            Guid? dataEnumeratorId = null,
            Guid? defaultGroupId = null,
            IReadOnlyDictionary<string, string>? translations = null,
            IReadOnlyList<string>? aliases = null,
            CancellationToken ct = default
        );

        Task UpdateDefinitionAsync(
            Guid actorUserId,
            Guid definitionId,
            string canonicalName,
            AdditionalPropertyDataTypeEnum dataType,
            string? description,
            Guid? dataEnumeratorId,
            Guid? defaultGroupId,
            bool isActive,
            CancellationToken ct = default
        );

        Task ReorderDefinitionsAsync(Guid actorUserId, IReadOnlyList<Guid> orderedDefinitionIds, CancellationToken ct = default);

        Task<int> DeleteDefinitionAsync(Guid actorUserId, Guid definitionId, CancellationToken ct = default);

        Task SetTranslationAsync(Guid actorUserId, Guid definitionId, string languageCode, string translation, CancellationToken ct = default);
        Task RemoveTranslationAsync(Guid actorUserId, Guid definitionId, string languageCode, CancellationToken ct = default);

        Task<(UserAdditionalPropertyDefinitionAliasEntityModel Alias, int LinkedPropertyCount)> AddAliasAsync(
            Guid actorUserId,
            Guid definitionId,
            string alias,
            CancellationToken ct = default
        );

        Task RemoveAliasAsync(Guid actorUserId, Guid definitionId, Guid aliasId, CancellationToken ct = default);

        Task<List<UnlinkedAdditionalPropertyNameDTO>> GetUnlinkedPropertyNamesAsync(CancellationToken ct = default);

        Task<AdditionalPropertyDefinitionSeedResultDTO> SeedBuiltInDefinitionsAsync(Guid? actorUserId, CancellationToken ct = default);
    }
}
