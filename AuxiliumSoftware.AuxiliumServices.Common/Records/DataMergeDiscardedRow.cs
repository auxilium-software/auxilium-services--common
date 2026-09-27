using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Records
{
    public sealed record DataMergeDiscardedRow(
        Guid Id,
        string? SnapshotJson
    );
}
