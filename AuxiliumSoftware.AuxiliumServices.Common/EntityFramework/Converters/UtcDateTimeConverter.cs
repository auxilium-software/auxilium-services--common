using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Converters
{
    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter() : base(
            v => v,                                         // writing: already UTC
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)) // reading: tag as UTC
        { }
    }
}
