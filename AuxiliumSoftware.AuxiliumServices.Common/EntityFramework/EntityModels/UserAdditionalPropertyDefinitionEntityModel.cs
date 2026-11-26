using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class UserAdditionalPropertyDefinitionEntityModel : MutableEntityModelBase
    {
        public required string CanonicalName { get; set; }
        public string? Description { get; set; }
        public required AdditionalPropertyDataTypeEnum DataType { get; set; }
        public Guid? DataEnumeratorId { get; set; }
        public Guid? DefaultGroupId { get; set; }





        public required bool IsBuiltIn { get; set; } = false;
        public required bool IsActive { get; set; } = true;
        public required int SortOrder { get; set; } = 0;





        public DataEnumeratorEntityModel? DataEnumerator { get; set; }
        public UserAdditionalPropertyGroupEntityModel? DefaultGroup { get; set; }
        public ICollection<UserAdditionalPropertyDefinitionTranslationEntityModel>? Translations { get; set; }
        public ICollection<UserAdditionalPropertyDefinitionAliasEntityModel>? Aliases { get; set; }
        public ICollection<UserAdditionalPropertyEntityModel>? Properties { get; set; }
    }
}
