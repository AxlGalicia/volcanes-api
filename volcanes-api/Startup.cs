
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using volcanes_api.Interfaces;
using volcanes_api.Models;
using volcanes_api.Services;

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
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1",new OpenApiInfo { 
                    Version = "v1.0.0",
                    Title = "Volcanes de Guatemala",
                    Description = "Esta API nos devuelve los volcanes de Guatemala con su descripcion, altura, ecosistema e imagen.",
                    Contact = new OpenApiContact { 
                        Name = "Contactar con el creador Axl Galicia",
                        Email = "axlgreatga@gmail.com",
                        Url = new Uri("https://github.com/AxlGalicia")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Link del Repositorio",
                        Url = new Uri("https://github.com/AxlGalicia/volcanes-api")
                    }

                });
            });
            services.AddDbContext<volcanesDBContext>(options =>
            {
                //Necesario para utilizar Docker
                string conexion = Environment.GetEnvironmentVariable("ConnectionString");
                options.UseMySql(conexion,ServerVersion.AutoDetect(conexion));
                
                //Variedad de opciones para utilizar Mysql o SqlServer
                //options.UseMySql(Configuration["ConnectionStrings"],ServerVersion.AutoDetect(Configuration["ConnectionStrings"]));
                //options.UseSqlServer(Configuration["ConnectionStrings"]);
            });
            services.AddLogging();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<ISpacesDigitalOceanService,SpacesDigitalOceanService>();

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

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
            

        }



    }
}
