using LLMService.DataProvider.Relational.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Tls;

namespace LLMService.DataProvider.Migrator
{
    /// <summary>
    /// 
    /// </summary>
    internal class ChatDbContextFactory : ChatDbContextFactoryGeneric<ChatDbContext>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory{TContext}" />
    internal class ChatDbContextFactoryGeneric<TContext> : IDesignTimeDbContextFactory<TContext>
        where TContext : DbContext
    {
        const string connectionKey = "DbConnection";
        const string assemblyName = "LLMService.DataProvider.Migrator";

        /// <summary>
        /// Creates a new instance of a derived context.
        /// </summary>
        /// <param name="args">Arguments provided by the design-time service.</param>
        /// <returns>
        /// An instance of <typeparamref name="TContext" />.
        /// </returns>
        public TContext CreateDbContext(string[] args)
        {
            // 
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .AddCommandLine(args)
                 .Build();
            var dbContextBuilder = new DbContextOptionsBuilder<TContext>();
            var connectionString = configuration.GetConnectionString(connectionKey);

            string migrationDbProvider = configuration["DbProvider"] ?? "SqlServer";
            //string contextTypeName = configuration["contextType"] ?? nameof(TContext);
            /*
             *  Sample DI code goes here:
                services.AddDbContext<TContext>(
                    options => _ = provider switch
                    {
                        "Sqlite" => options.UseSqlite(
                            configuration.GetConnectionString("SqliteConnection"),
                            x => x.MigrationsAssembly("SqliteMigrations")),

                        "SqlServer" => options.UseSqlServer(
                            configuration.GetConnectionString("SqlServerConnection"),
                            x => x.MigrationsAssembly("SqlServerMigrations")),

                        _ => throw new Exception($"Unsupported provider: {provider}")
                    });
             * 
             */

            if (migrationDbProvider.Contains("SqlServer", StringComparison.InvariantCultureIgnoreCase)){
                dbContextBuilder.UseSqlServer(connectionString, action => action.MigrationsAssembly(assemblyName));
            }
            else if(migrationDbProvider.Contains("MySql", StringComparison.InvariantCultureIgnoreCase))
            {
                dbContextBuilder.UseMySQL(connectionString, action => action.MigrationsAssembly(assemblyName));
            }
            else
            {
                throw new InvalidOperationException("Invalid DbProvider");
            }

            var dbContextInstance = (TContext)Activator.CreateInstance(typeof(TContext), dbContextBuilder.Options);

            return dbContextInstance;
        }
    }
}
