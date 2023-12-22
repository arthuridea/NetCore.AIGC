using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LLMServiceHub.Configuration
{
    /// <summary>
    /// 
    /// </summary>>
    public class AppSwaggerGenOption
    : IConfigureNamedOptions<SwaggerGenOptions>
    {
        /// <summary>
        /// The provider
        /// </summary>
        private readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSwaggerGenOption"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public AppSwaggerGenOption(
            IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Configure each API discovered for Swagger Documentation
        /// </summary>
        /// <param name="options">The options instance to configure.</param>
        public void Configure(SwaggerGenOptions options)
        {
            // add swagger document for every API version discovered
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    CreateVersionInfo(description));
            }
        }

        /// <summary>
        /// Configure Swagger Options. Inherited from the Interface
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        /// <summary>
        /// Create information about the version of the API
        /// </summary>
        /// <param name="apiDescription">The desc.</param>
        /// <returns>
        /// Information about the API
        /// </returns>
        private OpenApiInfo CreateVersionInfo(
                ApiVersionDescription apiDescription)
        {
            var info = new OpenApiInfo()
            {
                Title = "Baidu Erniebot & OpenAI ChatGPT LLM API for .NET",
                Version = $"v{apiDescription.ApiVersion.ToString()}",
                Description = " API for Erniebot|ErnieVilg|ChatGPT. ",
                Contact = new OpenApiContact() { Name = "arthuridea", Email = "arthuridea@gmail.com" }
            };

            if (apiDescription.IsDeprecated)
            {
                info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";
            }

            return info;
        }
    }
}
