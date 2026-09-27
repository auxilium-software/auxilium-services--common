using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Utilities
{
    public static class DataMergeReferenceUtilities
    {
        public static void VerifyCoverage(
            IModel model,
            Type principalClrType,
            IReadOnlyList<IEntityMergeReference> registry,
            params string[] referenceColumnSuffixes
        )
        {
            List<string> problems = new();
            Dictionary<(Type, string), IEntityMergeReference> registered = new();

            foreach (IEntityMergeReference reference in registry)
            {
                string label = $"{reference.EntityType.Name}.{reference.PropertyName}";

                if (!registered.TryAdd((reference.EntityType, reference.PropertyName), reference))
                {
                    problems.Add($"{label} is registered more than once.");
                }

                var entityType = model.FindEntityType(reference.EntityType);
                if (entityType is null)
                {
                    problems.Add($"{label} is registered, but {reference.EntityType.Name} is not part of the model.");
                    continue;
                }

                if (entityType.FindProperty(reference.PropertyName) is null)
                {
                    problems.Add($"{label} is registered, but the model has no such property.");
                }

                if (reference.CollidesOnPropertyName is not null && entityType.FindProperty(reference.CollidesOnPropertyName) is null)
                {
                    problems.Add($"{label} collides on {reference.CollidesOnPropertyName}, but the model has no such property.");
                }
            }

            foreach (var entityType in model.GetEntityTypes().OrderBy(t => t.ClrType.Name, StringComparer.Ordinal))
            {
                foreach (var fk in entityType.GetForeignKeys().Where(f => f.PrincipalEntityType.ClrType == principalClrType))
                {
                    IProperty fkProperty = fk.Properties[0];
                    string label = $"{entityType.ClrType.Name}.{fkProperty.Name}";

                    if (fk.Properties.Count != 1)
                    {
                        problems.Add($"{label} is part of a composite foreign key, which merging does not support.");
                        continue;
                    }

                    if (!registered.TryGetValue((entityType.ClrType, fkProperty.Name), out var reference))
                    {
                        problems.Add($"{label} references {principalClrType.Name} but is not in the merge registry.");
                        continue;
                    }

                    if (reference.Behaviour != DataMergeBehaviourEnum.Repoint) continue;

                    foreach (var index in entityType.GetIndexes().Where(i => i.IsUnique && i.Properties.Contains(fkProperty)))
                    {
                        List<string> partners = index.Properties.Where(p => p != fkProperty).Select(p => p.Name).ToList();
                        bool matches = partners.Count == 1 && partners[0] == reference.CollidesOnPropertyName;

                        if (!matches)
                        {
                            problems.Add(
                                $"{label} is repointed and sits in a unique index over" +
                                $" ({string.Join(", ", index.Properties.Select(p => p.Name))})," +
                                $" so it needs collidesOn: {(partners.Count == 1 ? partners[0] : "(not expressible as a single column)")}.");
                        }
                    }
                }

                string? table = entityType.GetTableName();
                if (table is null)
                {
                    continue;
                }
                StoreObjectIdentifier store = StoreObjectIdentifier.Table(table, entityType.GetSchema());

                foreach (IProperty property in entityType.GetProperties())
                {
                    if (property.ClrType != typeof(Guid) && property.ClrType != typeof(Guid?))
                    {
                        continue;
                    }

                    if (property.IsPrimaryKey() || property.IsForeignKey())
                    {
                        continue;
                    }

                    var column = property.GetColumnName(store);
                    if (
                        column is null
                        || !referenceColumnSuffixes.Any(
                            s => column.EndsWith(
                                s,
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                    )
                    {
                        continue;
                    }

                    if (!registered.ContainsKey((entityType.ClrType, property.Name)))
                    {

                        problems.Add(
                            $"{entityType.ClrType.Name}.{property.Name} ({table}.{column})" +
                            $" looks like a reference but has no foreign key and is not in the merge registry." +
                            $"  Add a foreign key, or register it."
                        );
                    }
                }
            }

            if (problems.Count > 0)
            {
                throw new InvalidOperationException(
                    $"The merge registry for {principalClrType.Name} is out of step with the EF model:"
                    + Environment.NewLine
                    + " - "
                    + string.Join(Environment.NewLine + " - ", problems)
                );
            }
        }
    }
}
