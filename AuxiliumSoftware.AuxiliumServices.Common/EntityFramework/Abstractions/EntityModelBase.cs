using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions
{
    public abstract class EntityModelBase
    {
        public required Guid Id { get; set; }





        public required DateTime CreatedAtUtc { get; set; }
    }
}
