using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services
{
    public interface IUserMergeService
    {
        Task<DataMergePreviewDTO> PreviewAsync(
            Guid survivorUserId,
            Guid duplicateUserId,
            Guid actorUserId,
            CancellationToken ct = default
        );

        Task<DataMergeResultDTO> MergeAsync(
            Guid survivorUserId,
            Guid duplicateUserId,
            string fingerprint,
            IReadOnlyDictionary<string, DataMergeSourceEnum>? fieldChoices,
            string justification,
            Guid actorUserId,
            CancellationToken ct = default
        );
    }
}
