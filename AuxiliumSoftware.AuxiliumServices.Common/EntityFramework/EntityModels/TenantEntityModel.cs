using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class TenantEntityModel
    {
        [Key]
        public required Guid Id { get; set; }

        public required string Domain { get; set; }





        public ICollection<UserEntityModel>? Users { get; set; }
    }
}
