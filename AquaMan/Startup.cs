using Aqua_Control;
using AquaMan.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderingApplication
{
    public class Startup
    {
        #region Members
        private IAquaPinController _pinController;
        private IAquaI2CController _i2CController;
        private PinMasterController _pinMasterController;
        #endregion

        #region Properties
        public IConfigurationRoot Configuration { get; }
        #endregion

        #region ctor
        /// <summary>
        /// Creates a new <see cref="Startup"/> object
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            _pinController = new EmptyPinController();
            _i2CController = new EmptyI2CController();
            //_pinMasterController = new PinMasterController(_i2CController, _pinController);
        }

        #endregion

        #region Methods
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // MVC setup
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();

            // Autofac setup
            ContainerBuilder builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterInstance(_pinController);
            builder.RegisterInstance(_i2CController);

            builder.RegisterType<FormDataService>().As<IFormDataService>();

            return new AutofacServiceProvider(builder.Build());
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        #endregion
    }
}
