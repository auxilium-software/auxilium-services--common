using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Interfaces
{
    public class IMandatoryFieldsEntityModel
    {
        [Key]
        public required Guid Id { get; set; }
        [Key]
        public required Guid TenantId { get; set; }





        public required DateTime CreatedAtUtc { get; set; }





        public TenantEntityModel? Tenant { get; set; }
    }
}
