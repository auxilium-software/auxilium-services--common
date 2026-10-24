using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseAdditionalPropertyGroupEntityModel : MutableTenantScopedEntityModelBase
    {
        public required string CanonicalName { get; set; }
        public int SortOrder { get; set; }




        public ICollection<CaseAdditionalPropertyGroupTranslationEntityModel>? Translations { get; set; }
        public ICollection<CaseAdditionalPropertyEntityModel>? Properties { get; set; }
    }
}
