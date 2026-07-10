using System.Reflection;
using Microsoft.EntityFrameworkCore;
using BehavioralReportEngine.Web.Data;

namespace BehavioralReportEngine.Web.Services
{
    public class PropertyDescriptor
    {
        public required PropertyInfo ClrProperty { get; init; }
        public required string Name { get; init; }
        public required Type ClrType { get; init; }
        public bool IsRequired { get; init; }
        public int? MaxLength { get; init; }
        public bool IsForeignKey { get; init; }
        public Type? PrincipalType { get; init; }
        public string? PrincipalDisplayProperty { get; init; }
    }

    public class EntityMetadata
    {
        public required Type ClrType { get; init; }
        public required PropertyInfo PrimaryKeyProperty { get; init; }
        public required Type PrimaryKeyClrType { get; init; }
        public required List<PropertyDescriptor> Properties { get; init; }
    }

    // Reads entity shape straight from EF Core's own model metadata at runtime, so the generic
    // CRUD pages (GenericEntityList/Form/Details) work for any of the 33 entities without a
    // per-entity hand-written page. The only thing EF metadata can't tell us is which property
    // is the "friendly" one to show for a FK - that comes from EntityRegistry.DisplayPropertyOverrides.
    public class EntityMetadataService
    {
        private static readonly HashSet<string> AuditColumnNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "IsDeleted", "DeletedAt", "DeletedBy"
        };

        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly Dictionary<Type, EntityMetadata> _cache = new();
        private readonly object _lock = new();

        public EntityMetadataService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public EntityMetadata GetMetadata(Type entityType)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(entityType, out var cached))
                    return cached;
            }

            using var context = _factory.CreateDbContext();
            var efEntityType = context.Model.FindEntityType(entityType)
                ?? throw new InvalidOperationException($"{entityType.Name} is not a mapped EF Core entity.");

            var pk = efEntityType.FindPrimaryKey()!.Properties[0];
            var pkProp = entityType.GetProperty(pk.Name)!;

            var fkByPropertyName = efEntityType.GetForeignKeys()
                .SelectMany(fk => fk.Properties.Select(p => (Property: p, ForeignKey: fk)))
                .GroupBy(x => x.Property.Name)
                .ToDictionary(g => g.Key, g => g.First().ForeignKey);

            var descriptors = new List<PropertyDescriptor>();
            foreach (var efProp in efEntityType.GetProperties())
            {
                if (efProp.Name == pk.Name) continue;
                if (AuditColumnNames.Contains(efProp.Name)) continue;

                var clrProp = entityType.GetProperty(efProp.Name);
                if (clrProp == null) continue;

                fkByPropertyName.TryGetValue(efProp.Name, out var fk);
                Type? principalType = fk?.PrincipalEntityType.ClrType;
                string? principalDisplay = principalType != null &&
                    EntityRegistry.DisplayPropertyOverrides.TryGetValue(principalType, out var dp)
                        ? dp
                        : null;

                descriptors.Add(new PropertyDescriptor
                {
                    ClrProperty = clrProp,
                    Name = efProp.Name,
                    ClrType = Nullable.GetUnderlyingType(efProp.ClrType) ?? efProp.ClrType,
                    IsRequired = !efProp.IsNullable,
                    MaxLength = efProp.GetMaxLength(),
                    IsForeignKey = fk != null,
                    PrincipalType = principalType,
                    PrincipalDisplayProperty = principalDisplay,
                });
            }

            var metadata = new EntityMetadata
            {
                ClrType = entityType,
                PrimaryKeyProperty = pkProp,
                PrimaryKeyClrType = pk.ClrType,
                Properties = descriptors,
            };

            lock (_lock)
            {
                _cache[entityType] = metadata;
            }
            return metadata;
        }

        // DbContext has no public non-generic Set(Type) overload, only Set<TEntity>(); this
        // invokes the generic method via reflection for the FK-principal-type-at-runtime case.
        public static List<object> GetAll(DbContext context, Type entityType)
        {
            var method = typeof(DbContext).GetMethods()
                .First(m => m.Name == nameof(DbContext.Set) && m.IsGenericMethodDefinition && m.GetParameters().Length == 0)
                .MakeGenericMethod(entityType);
            var dbSet = method.Invoke(context, null)!;
            return ((System.Collections.IEnumerable)dbSet).Cast<object>().ToList();
        }

        public static string GetDisplayText(object entity, PropertyDescriptor fkDescriptor)
        {
            var principalDisplayProp = fkDescriptor.PrincipalDisplayProperty != null
                ? entity.GetType().GetProperty(fkDescriptor.PrincipalDisplayProperty)
                : null;
            var value = principalDisplayProp?.GetValue(entity);
            return value?.ToString() ?? entity.ToString() ?? string.Empty;
        }
    }
}
