using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Interfaces
{
    public interface IEntityMergeReference
    {
        Type EntityType { get; }
        string PropertyName { get; }
        string? CollidesOnPropertyName { get; }
        DataMergeBehaviourEnum Behaviour { get; }

        Task<DataMergeReferenceCounts> CountAsync(AuxiliumDbContext db, Guid duplicateId, Guid survivorId, CancellationToken ct = default);

        Task<DataMergeReferenceChange> ApplyAsync(AuxiliumDbContext db, Guid duplicateId, Guid survivorId, CancellationToken ct = default);
    }
}
