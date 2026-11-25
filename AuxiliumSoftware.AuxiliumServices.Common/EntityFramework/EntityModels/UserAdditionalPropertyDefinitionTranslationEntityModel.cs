using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class UserAdditionalPropertyDefinitionTranslationEntityModel : MutableTenantScopedEntityModelBase
    {
        public required Guid UserAdditionalPropertyDefinitionId { get; set; }





        public required string LanguageCode { get; set; }
        public required string Translation { get; set; }





        public UserAdditionalPropertyDefinitionEntityModel? UserAdditionalPropertyDefinition { get; set; }
    }
}
