using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class UserAdditionalPropertyDefinitionAliasEntityModel : MutableTenantScopedEntityModelBase
    {
        public required Guid UserAdditionalPropertyDefinitionId { get; set; }





        public required string Alias { get; set; }
        public required string NormalisedAlias { get; set; }





        public UserAdditionalPropertyDefinitionEntityModel? UserAdditionalPropertyDefinition { get; set; }
    }
}
