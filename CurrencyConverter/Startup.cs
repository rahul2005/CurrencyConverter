using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Reflection;
using CurrencyConverter.Services;
using CurrencyConverter.Models;

namespace CurrencyConverter
{
    /// <summary>
    /// Class <see cref="Startup"/>
    /// </summary>
    public class Startup
    {
        private readonly string _myAllowSpecificOrigins = "_myAllowSpecificOrigins";
        /// <summary>
        /// Constructor for <see cref="Startup"/>
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// <see cref="IConfiguration"/>
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configuring services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {            
            services.AddCors(options =>
            {
                options.AddPolicy(_myAllowSpecificOrigins,
                           builder =>
                           {
                               builder.AllowAnyOrigin()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                           });
            });

            services.AddControllers().AddNewtonsoftJson();

            services.AddHttpContextAccessor();
            services.Configure<ConvertorOptions>(Configuration.GetSection("ConvertorOptions"));
            services.AddScoped(typeof(IRatesService), typeof(RatesService));
            services.AddApiVersioning();
            
            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(descriptions =>
                {
                    return descriptions.First();
                });

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                c.UseInlineDefinitionsForEnums();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.0",
                    Title = "Rates Convertor Api",
                    Description = "API for Rates Convertor ",
                    Contact = new OpenApiContact
                    {
                        Name = "NEO"
                    }
                });
            });
        }

        /// <summary>
        /// Configuring app
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });


            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                
            });

            app.UseCors(builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
                endpoints.MapGet("v1/_status", async context =>
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = MediaTypeNames.Application.Json;

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new StatusModel()));
                });
            });
        }
    }
    /// <summary>
    /// Status Model for health check status
    /// </summary>
    public class StatusModel
    {
        /// <summary>
        /// <see cref="Status"/>
        /// </summary>
        public string Status => "OK";
    }
}
