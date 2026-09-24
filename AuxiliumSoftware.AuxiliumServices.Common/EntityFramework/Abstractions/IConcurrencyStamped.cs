using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions
{
    public interface IConcurrencyStamped
    {
        Guid ConcurrencyStamp { get; set; }
    }
}
