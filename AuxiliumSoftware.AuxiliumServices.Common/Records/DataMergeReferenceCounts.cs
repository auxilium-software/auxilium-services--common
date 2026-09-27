using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Records
{
    public sealed record DataMergeReferenceCounts(
        long Total,
        long Colliding
    );
}
