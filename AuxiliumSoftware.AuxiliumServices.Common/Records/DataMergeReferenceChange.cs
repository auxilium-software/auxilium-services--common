using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Records
{
    public sealed record DataMergeReferenceChange(
        string Entity,
        string Property,
        DataMergeBehaviourEnum Behaviour,
        long RowsAffected,
        List<Guid> MovedRowIds,
        List<DataMergeDiscardedRow> DiscardedRows,
        long KeptOnDuplicate
    );
}
