using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SabotageSms.Models.DbModels;
using SabotageSms.Providers;

namespace SabotageSms
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }
        
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.default.json")
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInstance<IConfigurationRoot>(Configuration);
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));
            
            // Use SMS provider configured in app settings
            switch (Configuration["OutgoingSmsProvider"])
            {
                case "Nexmo":
                    services.AddSingleton<ISmsProvider, NexmoSmsProvider>();
                    break;
                case "Plivo":
                    services.AddSingleton<ISmsProvider, PlivoSmsProvider>();
                    break;
                default:
                    services.AddSingleton<ISmsProvider, PlivoSmsProvider>();
                    break;
            }
            
            services.AddSingleton<ParsingProvider, ParsingProvider>();
            services.AddScoped<IGameDataProvider, EfGameDataProvider>();
            services.AddMvc()
                .AddXmlSerializerFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseIISPlatformHandler();
            
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvcWithDefaultRoute();
            
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                    .Database.Migrate();
            }
        }

        // Entry point for the application.
        public static void Main(string[] args) => Microsoft.AspNet.Hosting.WebApplication.Run<Startup>(args);
    }
}
