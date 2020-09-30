using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using AutoMapper;
using FRF.Core.Services;
using FRF.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace FiveRockingFingers
{
    public class Startup
    {
        public static readonly IEnumerable<Profile> AutoMapperProfiles = new Profile[]
        {
            new FRF.Core.AutoMapperProfile(),
            new FRF.Web.Dtos.AutoMapperProfile(),
        };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddDbContext<DataAccessContext>(c =>
            {
                c.UseSqlServer(Configuration.GetConnectionString("FiveRockingFingers"));
            });
            services.AddCors();

            //Start Cognito Authorization and Identity
            services.AddScoped<IConfigurationService, ConfigurationService>();
            var CognitoCredencials = services.BuildServiceProvider().GetService<IConfigurationService>().GetConfigurationSettings();
            var provider = new AmazonCognitoIdentityProviderClient(CognitoCredencials.AccessKeyId,
                CognitoCredencials.SecretAccKey,
                RegionEndpoint.USWest2);
            var cognitoUserPool = new CognitoUserPool(CognitoCredencials.UserPoolId, CognitoCredencials.ClientId, provider);
            services.AddSingleton<IAmazonCognitoIdentityProvider>(provider);
            services.AddSingleton(cognitoUserPool);

            services.AddCognitoIdentity();
            //End Cognito 

            /*Authorization : pendiente realizado login page 
            services.AddAuthorization(opt =>
            {
                opt.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                
            });
            
            services.ConfigureApplicationCookie(options => options.LoginPath = "/api/SignIn/login"); //<-cambiar por login page.
            */
            services.AddTransient<IProjectsService, ProjectsService>();
            services.AddTransient<ISignUpService, SignUpService>();
            services.AddTransient<ISignInService, SignInService>();
            services.AddTransient<IUserService, UserService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Five Rocking Fingers", Version = "v1"});
            });


            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

            var autoMapperProfileTypes = AutoMapperProfiles.Select(p => p.GetType()).ToArray();
            services.AddAutoMapper(autoMapperProfileTypes);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseCors();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Five Rocking Fingers"); });


            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}