// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.HttpsPolicy;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
// using Microsoft.OpenApi.Models;

// namespace API_oficial_5._0
// {
//     public class Startup
//     {
//         public Startup(IConfiguration configuration)
//         {
//             Configuration = configuration;
//         }

//         public IConfiguration Configuration { get; }

//         // This method gets called by the runtime. Use this method to add services to the container.
//         public void ConfigureServices(IServiceCollection services)
//         {

//             services.AddControllers();
//             services.AddSwaggerGen(c =>
//             {
//                 c.SwaggerDoc("v1", new OpenApiInfo { Title = "API_oficial_5._0", Version = "v1" });
//             });
//         }

//         // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//         public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//         {
//             app.UseCors(builder => builder.AllowAnyMethod()
//                                           .AllowAnyOrigin()
//                                           .AllowAnyHeader());

//             if (env.IsDevelopment())
//             {
//                 app.UseDeveloperExceptionPage();
//                 app.UseSwagger();
//                 app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_oficial_5._0 v1"));
//             } else
//             {
//                 app.UseHsts();
//             }

//             app.UseHttpsRedirection();
//             app.UseRouting();
//             app.UseAuthorization();
//             app.UseAuthentication();
//             app.UseEndpoints(endpoints =>
//             {
//                 endpoints.MapControllers();
//             });
//         }
//     }
// }

using System;
using System.Text;
using API.Brokers;
using API.Models;
using Api_Empresa.Middlewares;
using AutoMapper;
using Brokers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace API_oficial_5._0
{
  public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllers();
            services.AddCors();
            services.AddAutoMapper(typeof(Startup));
            AtivaAutenticacaoJWT(services);
            InicializaBancoDeDados(services);
        }

        private void AtivaAutenticacaoJWT(IServiceCollection services)
        {
            var jwtSettings = new JwtModel();
            Configuration.Bind(nameof(JwtModel), jwtSettings);
            services.AddSingleton(jwtSettings);
            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        private void InicializaBancoDeDados(IServiceCollection services)
        {
            string conString = Configuration.GetConnectionString("Banco");
            services.AddDbContext<Database>(context => context.UseNpgsql(conString));
            services.AddDbContext<ClienteDatabase>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder => builder.AllowAnyMethod()
                                          .AllowAnyOrigin()
                                          .AllowAnyHeader());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
            {
                app.UseHsts();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEmpresaIdentificador();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

