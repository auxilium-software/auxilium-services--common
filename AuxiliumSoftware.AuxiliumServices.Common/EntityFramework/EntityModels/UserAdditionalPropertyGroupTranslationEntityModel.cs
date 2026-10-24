using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class UserAdditionalPropertyGroupTranslationEntityModel : MutableTenantScopedEntityModelBase
    {
        /// <summary>
        /// </summary>
        public required Guid UserAdditionalPropertyGroupId { get; set; }





        /// <summary>
        /// </summary>
        public required string LanguageCode { get; set; }
        /// <summary>
        /// </summary>
        public required string Translation { get; set; }





        /// <summary>
        /// </summary>
        public UserAdditionalPropertyGroupEntityModel? UserAdditionalPropertyGroup { get; set; }
    }
}
