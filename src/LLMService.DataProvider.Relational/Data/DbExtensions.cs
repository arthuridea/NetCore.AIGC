using LLMService.DataProvider.Relational.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namotion.Reflection;

namespace LLMService.DataProvider.Relational.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class DbExtensions
    {
        /// <summary>
        /// Sets the comment.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        public static void SetComment(this ModelBuilder modelBuilder)
        {
            
            var entityTypes = modelBuilder.Model.GetEntityTypes();
            foreach (var item in entityTypes)
            {
                var entityType = item.ClrType;
                var summary = entityType?.GetXmlDocsSummary();
                if (!string.IsNullOrEmpty(summary))
                {
                    item.SetComment($"{summary}");
                }

                var typeProperties = entityType.GetProperties();

                var properties = item.GetProperties();
                foreach (var property in properties)
                {
                    var typeprop = typeProperties.FirstOrDefault(a => a.Name == property.Name);
                    summary = typeprop?.GetXmlDocsSummary();

                    if (!string.IsNullOrEmpty(summary))
                    {
                        property.SetComment(summary);
                    }
                }
            }
            
        }

        /// <summary>
        /// Configurations the entities that implemented from i domain entity.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="dbContext">The database context.</param>
        /// <returns></returns>
        public static ModelBuilder ConfigEntitiesThatImplementedFromIDomainEntity(this ModelBuilder modelBuilder, DbContext dbContext)
        {
            foreach (Type domain in from e in modelBuilder.Model.GetEntityTypes()
                                    where !e.IsOwned()
                                    select e.ClrType)
            {
                modelBuilder.Entity(domain, delegate (EntityTypeBuilder b)
                {

                    if (domain.IsDerivedFrom(typeof(ICreatedUtcTime)))
                    {
                        string defaultDateSql = "";
                        if (dbContext.Database.IsSqlServer())
                        {
                            defaultDateSql = "getutcdate()";
                        }
                        else if(dbContext.Database.IsMysql())
                        {
                            defaultDateSql = "UTC_DATE()";
                        }
                        else
                        {
                            throw new Exception($"DbType {dbContext.Database.ProviderName} not supported");
                        }

                        b.Property("CreatedUtcTime").HasDefaultValueSql(defaultDateSql);
                    }

                });
            }

            return modelBuilder;
        }

        /// <summary>
        /// Determines whether [is derived from] [the specified pattern].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns>
        ///   <c>true</c> if [is derived from] [the specified pattern]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// type
        /// or
        /// pattern
        /// </exception>
        public static bool IsDerivedFrom(this Type type, Type pattern)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }

            if (type.IsSubclassOf(pattern))
            {
                return true;
            }

            if (pattern.IsAssignableFrom(type))
            {
                return true;
            }

            if (type.GetInterfaces().Any(IsTheRawGenericType))
            {
                return true;
            }

            while (type != null && type != typeof(object))
            {
                if (IsTheRawGenericType(type))
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
            bool IsTheRawGenericType(Type test)
            {
                return pattern == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
            }
        }

        /// <summary>
        /// Determines whether [is SQL server].
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>
        ///   <c>true</c> if [is SQL server] [the specified database]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSqlServer(this DatabaseFacade database)
        {
            return database.ProviderName.Equals("Microsoft.EntityFrameworkCore.SqlServer", StringComparison.InvariantCultureIgnoreCase);
        }
        /// <summary>
        /// Determines whether this instance is mysql.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>
        ///   <c>true</c> if the specified database is mysql; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMysql(this DatabaseFacade database)
        {
            return database.ProviderName.Contains("Mysql", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
