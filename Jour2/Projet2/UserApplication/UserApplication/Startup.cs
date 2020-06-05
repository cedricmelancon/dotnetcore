using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using UserApplication.Configurations;
using UserApplication.Data;
using UserApplication.Services;

namespace UserApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            int x;
            int x;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var dbConfig = AddConfiguration<SqlConfiguration>(services, "AzureSQL");
            services.AddScoped<IUserService, UserService>();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"}); });

            services.AddDbContext<UserContext>(options =>
            {
                options.UseSqlServer(dbConfig.ConnectionString);
            });

            services.AddScoped<DbContext, UserContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middle-ware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middle-ware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        #region private
        private T AddConfiguration<T>(IServiceCollection services, string key) where T : class, new()
        {
            var options = new T();

            Configuration.Bind(key, options);
            services.AddSingleton(options);
            return options;
        }
        #endregion
    }
}
