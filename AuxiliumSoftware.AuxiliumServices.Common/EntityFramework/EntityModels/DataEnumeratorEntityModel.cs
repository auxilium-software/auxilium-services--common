using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class DataEnumeratorEntityModel : MutableTenantScopedEntityModel
    {
        /// <summary>
        /// </summary>
        public required DataEnumeratorScopeEnum Scope { get; set; }
        /// <summary>
        /// </summary>
        public required string CanonicalName { get; set; }
        /// <summary>
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// </summary>
        public required bool IsActive { get; set; }





        /// <summary>
        /// </summary>
        public ICollection<DataEnumeratorValueEntityModel>? EnumeratorValues { get; set; }
        /// <summary>
        /// </summary>
        public ICollection<DataEnumeratorTranslationEntityModel>? Translations { get; set; }
    }
}
