using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MultitenantTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static MultitenantContainer ApplicationContainer { get; set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                //.AddAutofacMultitenantRequestServices(() => ApplicationContainer)
                .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ContainerBuilder builder = new ContainerBuilder();
            builder.Populate(services);



            builder.RegisterType<TenantIdentificationStrategy>()
                .As<ITenantIdentificationStrategy>();

            var container = builder.Build();

            var strategy = new TenantIdentificationStrategy(container.Resolve<IHttpContextAccessor>());

            MultitenantContainer mtc = new MultitenantContainer(strategy, container);

            mtc.ConfigureTenant("tenantA", cb => cb.RegisterType<TenantA>().As<ITenantTest>().InstancePerLifetimeScope());
            mtc.ConfigureTenant("tenantB", cb => cb.RegisterType<TenantB>().As<ITenantTest>().InstancePerLifetimeScope());

            ApplicationContainer = mtc;

            return new AutofacServiceProvider(mtc);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
