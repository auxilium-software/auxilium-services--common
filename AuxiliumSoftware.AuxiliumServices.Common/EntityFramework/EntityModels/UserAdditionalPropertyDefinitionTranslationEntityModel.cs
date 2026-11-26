using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class UserAdditionalPropertyDefinitionTranslationEntityModel : MutableEntityModelBase
    {
        public required Guid UserAdditionalPropertyDefinitionId { get; set; }





        public required string LanguageCode { get; set; }
        public required string Translation { get; set; }





        public UserAdditionalPropertyDefinitionEntityModel? UserAdditionalPropertyDefinition { get; set; }
    }
}
