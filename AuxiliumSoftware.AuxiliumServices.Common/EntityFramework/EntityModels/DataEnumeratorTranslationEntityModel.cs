using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class DataEnumeratorTranslationEntityModel : MutableTenantScopedEntityModel
    {
        /// <summary>
        /// </summary>
        public required Guid DataEnumeratorId { get; set; }





        /// <summary>
        /// </summary>
        public required string LanguageCode { get; set; }
        /// <summary>
        /// </summary>
        public required string Translation { get; set; }





        /// <summary>
        /// </summary>
        public DataEnumeratorEntityModel? Enum { get; set; }
    }
}
