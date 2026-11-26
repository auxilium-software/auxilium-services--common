using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseAdditionalPropertyDefinitionTranslationEntityModel : MutableEntityModelBase
    {
        public required Guid CaseAdditionalPropertyDefinitionId { get; set; }





        public required string LanguageCode { get; set; }
        public required string Translation { get; set; }





        public CaseAdditionalPropertyDefinitionEntityModel? CaseAdditionalPropertyDefinition { get; set; }
    }
}
