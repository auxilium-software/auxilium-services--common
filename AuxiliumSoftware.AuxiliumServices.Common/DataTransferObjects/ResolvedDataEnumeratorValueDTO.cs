using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects
{
    public class ResolvedDataEnumeratorValueDTO
    {
        public required Guid ValueId { get; set; }
        public required Guid EnumTypeId { get; set; }
        public required string ValueCanonicalName { get; set; }
        public required string ValueDisplay { get; set; }
        public required string EnumCanonicalName { get; set; }
        public required string EnumDisplay { get; set; }
    }
}
