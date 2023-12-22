using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Conventions;
using IdentityModel;
using LLMService.Baidu.ErnieVilg;
using LLMService.OpenAI.ChatGPT.Model;
using LLMService.Shared.DataProvider;
using LLMService.Shared.Models;
using LLMService.Shared.ServiceInterfaces;
using LLMServiceHub.Common;
using LLMServiceHub.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace LLMServiceHub
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// 
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Environment = env;
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                               .ReadFrom.Configuration(Configuration)
                               .CreateLogger();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            services.AddSingleton(appSettings);


            // Ensure HttpContext injected. It will be accessed in Chat service.
            services.AddHttpContextAccessor();


            /****************************************** Dependency Injection Goes Here ******************************************/

            // inject image storage provider
            services.AddTransient<IImageStorageProvider, LocalImageStorageProvider>();
            // inject chat data provider
            services.AddTransient<IChatDataProvider<ChatMessageBase, string>, ChatDataProvider<ChatMessageBase, string>>();
            services.AddTransient<IChatDataProvider<OpenAIChatMessage, List<OpenAIMessageContent>>, ChatDataProvider<OpenAIChatMessage, List<OpenAIMessageContent>>>();

            // inject service
            services.AddWenxinworkshop(Configuration);
            services.AddChatGPT(Configuration);
            services.AddErnieVilg(Configuration);

            /********************************************* End Dependency Injection **********************************************/


            // Add services to the container.
            services.AddRazorPages();

            //services.AddControllers()
            //        .AddJsonOptions(options => {
            //            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            //        });
            services.AddMvc();


            #region authentication

            // use cookies authentication by default.
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = AppConsts.DefaultAuthScheme;
                options.DefaultAuthenticateScheme = AppConsts.DefaultAuthScheme;
                options.DefaultChallengeScheme = AppConsts.DefaultAuthScheme;
                options.DefaultSignInScheme = AppConsts.DefaultAuthScheme;
                options.DefaultSignOutScheme = AppConsts.DefaultAuthScheme;
            })
            .AddCookie(AppConsts.DefaultAuthScheme, options =>
            {
                options.Cookie.Name = AppConsts.AuthCookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;

                options.LoginPath = new PathString("/");

                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.SlidingExpiration = true;

                options.Events.OnRedirectToLogin = context =>
                {
                    if(context.Request.Path.StartsWithSegments("/api")||
                       string.Equals(context.HttpContext.Request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
                       string.Equals(context.HttpContext.Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal))
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.FromResult(0);
                    }
                    context.Response.Redirect(context.RedirectUri);
                    return Task.FromResult(0);
                };
            })
            // api use jwt authentication
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = appSettings.IdentityServerBaseUrl;
                options.RequireHttpsMetadata = appSettings.RequireHttpsMetadata;
                options.Audience = appSettings.OidcApiName;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("api-oidc",
                    policy =>
                        //policy.RequireAssertion(context => context.User.HasClaim(c =>
                        //        ((c.Type == JwtClaimTypes.Role && c.Value == appSettings.AdministrationRole) ||
                        //        (c.Type == $"client_{JwtClaimTypes.Role}" && c.Value == appSettings.AdministrationRole))
                        //    ) && context.User.HasClaim(c => c.Type == JwtClaimTypes.Scope && c.Value == appSettings.OidcApiName)
                        //));
                        policy.RequireAssertion(context => context.User.HasClaim(c => c.Type == JwtClaimTypes.Scope && c.Value == appSettings.OidcApiName)
                        ));
            });

            #endregion

            #region API Versioning
            services.AddEndpointsApiExplorer();
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;

                //o.Conventions.Add(new VersionByNamespaceConvention());
                //o.ApiVersionReader = new UrlSegmentApiVersionReader();
                o.ApiVersionReader = ApiVersionReader.Combine(
                                        new QueryStringApiVersionReader(), 
                                        new UrlSegmentApiVersionReader(),
                                        new HeaderApiVersionReader("x-api-version"),
                                        new MediaTypeApiVersionReader("x-api-version")
                                        );
                //o.ApiVersionReader = ApiVersionReader.Combine(
                //        new QueryStringApiVersionReader()
                //        , new HeaderApiVersionReader()
                //        {
                //            HeaderNames = { "api-version" }
                //        }
                //        );
            })
            .AddMvc(options =>
            {
                options.Conventions.Add(new VersionByNamespaceConvention());
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVVV";
                options.SubstitutionFormat = "VVVV";
                options.AssumeDefaultVersionWhenUnspecified= true;
                //options.AddApiVersionParametersWhenVersionNeutral = true;
                options.SubstituteApiVersionInUrl = true;
                //options.ApiVersionParameterSource = new UrlSegmentApiVersionReader();


                options.ApiVersionParameterSource = ApiVersionReader.Combine(
                                        new QueryStringApiVersionReader(),
                                        new UrlSegmentApiVersionReader(),
                                        new HeaderApiVersionReader("x-api-version"),
                                        new MediaTypeApiVersionReader("x-api-version")
                                        );
                //options.ApiVersionParameterSource
            });
            #endregion

            #region SwashBuckle Config

            services.AddTransient<IConfigureNamedOptions<SwaggerGenOptions>, AppSwaggerGenOption>();

            services.ConfigureOptions<AppSwaggerGenOption>();

            
            services.AddSwaggerGen(option =>
            {
                option.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });

                //文档版本号
                option.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var versions = apiDesc.CustomAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v}" == docName);
                });

                option.UseOneOfForPolymorphism();

                option.UseAllOfForInheritance();

                option.UseAllOfToExtendReferenceSchemas();

                option.SelectSubTypesUsing(baseType =>
                {
                    return typeof(Program).Assembly.GetTypes().Where(type => type.IsSubclassOf(baseType));
                });

                option.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);

                option.IncludeXmlCommentsFromInheritDocs();

                //option.SchemaFilter<XEnumNamesSchemaFilter>();

                // 如果通过路由确定版本号，如项目按如下结构组织 V1/CaptchaController 则需要特殊处理版本规则
                //option.OperationFilter<RemoveVersionParameterOperationFilter>();
                //option.DocumentFilter<SetVersionInPathDocumentFilter>();

                //项目xml文档
                // ref:https://stackoverflow.com/questions/44643151/how-to-include-xml-comments-files-in-swagger-in-asp-net-core
                var xmls = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)), "*.xml");
                foreach (var filePath in xmls)
                {
                    try
                    {
                        option.IncludeXmlCommentsWithRemarks(filePath);
                        //option.SchemaFilter<EnumTypesSchemaFilter>(filePath);
                        ////option.SchemaFilter<SwaggeEnumSchemaFilter>(filePath);
                        //var doc = System.Xml.Linq.XDocument.Load(filePath);
                        //option.SchemaFilter<DescribeEnumMembers>(doc);
                    }
                    catch
                    {
                    }
                }

                // 增加Enum支持
                option.AddEnumsWithValuesFixFilters(o =>
                {
                    // add schema filter to fix enums (add 'x-enumNames' for NSwag or its alias from XEnumNamesAlias) in schema
                    o.ApplySchemaFilter = true;

                    // alias for replacing 'x-enumNames' in swagger document
                    o.XEnumNamesAlias = "x-enum-varnames";

                    // alias for replacing 'x-enumDescriptions' in swagger document
                    o.XEnumDescriptionsAlias = "x-enum-descriptions";

                    // add parameter filter to fix enums (add 'x-enumNames' for NSwag or its alias from XEnumNamesAlias) in schema parameters
                    o.ApplyParameterFilter = true;

                    // add document filter to fix enums displaying in swagger document
                    o.ApplyDocumentFilter = true;

                    // add descriptions from DescriptionAttribute or xml-comments to fix enums (add 'x-enumDescriptions' or its alias from XEnumDescriptionsAlias for schema extensions) for applied filters
                    o.IncludeDescriptions = true;

                    // add remarks for descriptions from xml-comments
                    o.IncludeXEnumRemarks = true;

                    // get descriptions from DescriptionAttribute then from xml-comments
                    o.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;

                    // get descriptions from xml-file comments on the specified path
                    // should use "options.IncludeXmlComments(xmlFilePath);" before
                    //o.IncludeXmlCommentsFrom(xmlFilePath);
                    // the same for another xml-files...
                });

                //option.UseInlineDefinitionsForEnums();

                //注册api签名公共参数
                //必须在 EnableAnnotations 之后注册，否则会被方法中的属性标签覆盖
                string description = @"api signature based authorization filter.<br />
                                       use app,ts,nonce,sig parameters building controller or action level authorization.basically,the signature algorithms is:<br />
                                       1.sort query parameters without signature in alphabet sequence<br /> 
                                       2.build query string for parameters: ?key1=encode({value1})&amp;key2=encde({value2})....<br /> 
                                       3.contact api endpoint with parameters generated in step2 as sigbase string<br />
                                       4.calulate HMACSHA256 hash using {sigbase} with appsecret";

                option.OperationFilter<SortedQueryWithSignatureParameterOperationFilter>(description);

                //oauth2
                option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{appSettings.IdentityServerBaseUrl}/connect/authorize"),
                            TokenUrl = new Uri($"{appSettings.IdentityServerBaseUrl}/connect/token"),
                            Scopes = new Dictionary<string, string> {
                                { appSettings.OidcApiName, appSettings.ApiName }
                            }
                        }
                    }
                });
                option.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            #endregion

            #region NSwag Config

            //services.AddOpenApiDocument();

            #endregion

            //services.AddSwaggerDoc();//（用于MarkDown生成）

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="appSettings"></param>
        /// <param name="logger"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppSettings appSettings, ILogger<Startup> logger)
        {
            // apply serilog middleware
            app.UseSerilogRequestLogging();

            logger.LogInformation($"app configuring with env:{env.EnvironmentName}");

            // Configure the HTTP request pipeline.
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            var apiVersionDescriptionProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();


            //Enable middleware to serve generated Swagger as a JSON endpoint.

            #region SwashBuclke Config
            app.UseSwagger(option =>
            {
                option.RouteTemplate = "/api-docs/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(option =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    option.SwaggerEndpoint($"/api-docs/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }

                option.RoutePrefix = "api-docs";
                option.DocumentTitle = "百度千帆大模型 API";

                option.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                option.DefaultModelExpandDepth(4);

                option.OAuthClientId(appSettings.OidcSwaggerUIClientId);
                option.OAuthAppName(appSettings.ApiName);
                option.OAuthUsePkce();
            });

            #endregion


            #region NSwag Config
            /*
            app.UseOpenApi(); // serve OpenAPI/Swagger documents
            app.UseSwaggerUI(); // serve Swagger UI
            app.UseReDoc(); // serve ReDoc UI
            */
            #endregion

            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseAuthorization();


            //app.UseOpenApi(); //添加swagger生成api文档（默认路由文档 /swagger/v1/swagger.json）
            //app.UseSwaggerUi3();//添加Swagger UI到请求管道中(默认路由: /swagger).


            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "area-route",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapSwagger(pattern: "/");
            });

        }


    }
}
