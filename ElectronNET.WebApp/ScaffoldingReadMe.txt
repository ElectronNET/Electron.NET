
ASP.NET MVC core dependencies have been added to the project.
(These dependencies include packages required to enable scaffolding)

However you may still need to do make changes to your project.

1. Suggested changes to Startup class:
    1.1 Add a constructor:
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    1.2 Add MVC services:
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
       }

    1.3 Configure web app to use use Configuration and use MVC routing:

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
