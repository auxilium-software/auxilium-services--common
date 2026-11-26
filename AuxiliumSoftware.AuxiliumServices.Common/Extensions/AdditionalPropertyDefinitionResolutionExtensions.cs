using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AuxiliumSoftware.AuxiliumServices.Common.Extensions
{
    public static class AdditionalPropertyDefinitionResolutionExtensions
    {
        public static async Task<UserAdditionalPropertyDefinitionEntityModel?> ResolveUserAdditionalPropertyDefinitionAsync(
            this AuxiliumDbContext db,
            string propertyName,
            CancellationToken ct = default
        )
        {
            var key = PropertyNameHandlingUtilities.NormaliseForMatching(propertyName);
            if (key.Length == 0) return null;

            return await db.UserAdditionalProperty_Definition_Aliases
                .Where(a => a.NormalisedAlias == key && a.UserAdditionalPropertyDefinition!.IsActive)
                .Select(a => a.UserAdditionalPropertyDefinition!)
                .FirstOrDefaultAsync(ct);
        }

        public static async Task<CaseAdditionalPropertyDefinitionEntityModel?> ResolveCaseAdditionalPropertyDefinitionAsync(
            this AuxiliumDbContext db,
            string propertyName,
            CancellationToken ct = default
        )
        {
            var key = PropertyNameHandlingUtilities.NormaliseForMatching(propertyName);
            if (key.Length == 0) return null;

            return await db.CaseAdditionalProperty_Definition_Aliases
                .Where(a => a.NormalisedAlias == key && a.CaseAdditionalPropertyDefinition!.IsActive)
                .Select(a => a.CaseAdditionalPropertyDefinition!)
                .FirstOrDefaultAsync(ct);
        }
    }
}
