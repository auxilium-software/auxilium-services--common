using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services
{
    public interface IDataEnumeratorService
    {
        Task<List<DataEnumeratorEntityModel>> GetAllEnumeratorsAsync(bool includeInactive = false, CancellationToken ct = default);
        Task<DataEnumeratorEntityModel?> GetEnumeratorAsync(Guid id, CancellationToken ct = default);
        Task<DataEnumeratorEntityModel?> GetEnumeratorByNameAsync(string name, bool activeValuesOnly = true, CancellationToken ct = default);
        Task<DataEnumeratorEntityModel> CreateEnumeratorAsync(string name, string? description, EnumDataTypeEnum dataType, UserEntityModel createdBy, CancellationToken ct = default);
        Task<DataEnumeratorEntityModel> UpdateEnumeratorAsync(Guid id, string? name, string? description, UserEntityModel updatedBy, CancellationToken ct = default);
        Task SetEnumeratorActiveAsync(Guid id, bool isActive, UserEntityModel updatedBy, CancellationToken ct = default);





        Task<List<DataEnumeratorValueEntityModel>> GetValuesAsync(Guid enumeratorId, bool includeInactive = false, CancellationToken ct = default);
        Task<DataEnumeratorValueEntityModel?> GetValueAsync(Guid valueId, CancellationToken ct = default);
        Task<DataEnumeratorValueEntityModel> CreateValueAsync(Guid enumeratorId, string displayName, string storedValue, UserEntityModel createdBy, int? sortOrder = null, CancellationToken ct = default);
        Task<DataEnumeratorValueEntityModel> UpdateValueAsync(Guid valueId, string? displayName, string? storedValue, UserEntityModel updatedBy, CancellationToken ct = default);
        Task SetValueActiveAsync(Guid valueId, bool isActive, UserEntityModel updatedBy, CancellationToken ct = default);
        Task ReorderValuesAsync(Guid enumeratorId, List<Guid> orderedValueIds, UserEntityModel updatedBy, CancellationToken ct = default);
    }
}
