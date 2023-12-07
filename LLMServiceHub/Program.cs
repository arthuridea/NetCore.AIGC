using Serilog;

namespace LLMServiceHub
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        const string APPLICATION_NAME = "BAIDU WENXIN API";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Main(string[] args)
        {
            try
            {                
                var host = CreateHostBuilder(args).Build();
                Log.Information($"Welcome:  {APPLICATION_NAME}...");
                //var migrationStatus = RunDatabaseMigration(host);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"{APPLICATION_NAME} terminated:{ex.ToString()}");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config = _loadConfig(args, hostingContext, config);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseIISIntegration()
                        .UseStartup<Startup>();
                });

        private static IConfigurationBuilder _loadConfig(string[] args, HostBuilderContext hostingContext, IConfigurationBuilder config = null)
        {
            config = config ?? new ConfigurationBuilder();

            config.SetBasePath(Directory.GetCurrentDirectory());
            var env = hostingContext.HostingEnvironment;
            config.AddJsonFile("appsettings.json",
                                 optional: true, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                                 optional: true, reloadOnChange: true)
                  .AddJsonFile($"serilog.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables();

            if (args != null)
            {
                config.AddCommandLine(args);
            }

            if (hostingContext.HostingEnvironment.IsDevelopment())
            {
                config.AddUserSecrets<Program>();
            }

            return config;
        }

    }
}