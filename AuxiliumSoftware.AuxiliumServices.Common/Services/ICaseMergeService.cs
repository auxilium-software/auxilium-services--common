using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services
{
    public interface ICaseMergeService
    {
        Task<DataMergePreviewDTO> PreviewAsync(
            Guid survivorCaseId,
            Guid duplicateCaseId,
            CancellationToken ct = default
        );

        Task<DataMergeResultDTO> MergeAsync(
            Guid survivorCaseId,
            Guid duplicateCaseId,
            string fingerprint,
            IReadOnlyDictionary<string, DataMergeSourceEnum>? fieldChoices,
            string justification,
            Guid actorUserId,
            CancellationToken ct = default
        );
    }
}
