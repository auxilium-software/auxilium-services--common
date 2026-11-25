using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseAdditionalPropertyDefinitionAliasEntityModel : MutableTenantScopedEntityModelBase
    {
        public required Guid CaseAdditionalPropertyDefinitionId { get; set; }





        public required string Alias { get; set; }
        public required string NormalisedAlias { get; set; }





        public CaseAdditionalPropertyDefinitionEntityModel? CaseAdditionalPropertyDefinition { get; set; }
    }
}
