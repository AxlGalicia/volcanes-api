
using Microsoft.EntityFrameworkCore;
using volcanes_api.Models;

namespace volcanes_api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;  
        }

        public void ConfigureServices(IServiceCollection services) 
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddDbContext<volcanesDBContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionStrings"]);
            });
            services.AddLogging();
            services.AddSingleton<IConfiguration>(Configuration);

            //services.AddCors(opciones => {
            //    opciones.AddDefaultPolicy(builder => {
            //        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            //    });
            //});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI();

            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
            

        }



    }
}
