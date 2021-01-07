using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using AutoMapper;
using FRF.Core.Base;
using FRF.Core.Services;
using FRF.DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FRF.Web
{
	public class Startup
	{

		public static readonly IEnumerable<Profile> AutoMapperProfiles = new Profile[]
        {
            new FRF.Web.Dtos.AutoMapperProfile(),
            new FRF.Core.AutoMapperProfile(),
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
			services.AddCors(opt =>
			{
				opt.AddPolicy("CorsPolicy",
					policy => { policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000/"); });
			});

			//Start Cognito Authorization and Identity
            var awsCognitoCredential = Configuration.GetSection(AwsCognito.AwsCognitoCredential).Get<AwsCognito>();
			var provider = 
                new AmazonCognitoIdentityProviderClient(awsCognitoCredential.AccessKeyId, awsCognitoCredential.SecretAccKey, RegionEndpoint.USEast1);
			var cognitoUserPool =
				new CognitoUserPool(awsCognitoCredential.UserPoolId,awsCognitoCredential.ClientId, provider);
			services.AddSingleton<IAmazonCognitoIdentityProvider>(provider);
			services.AddSingleton(cognitoUserPool);
			services.AddCognitoIdentity();
			//End Cognito
            
            services.Configure<AwsPricing>(Configuration.GetSection(AwsPricing.AwsPricingOptions));
            services.AddHttpClient();
			services.AddSingleton(new AmazonPricingClient(Configuration["AwsAccessKeys:AccessKeyId"], Configuration["AwsAccessKeys:SecretAccessKey"], RegionEndpoint.USEast1));
			services.AddTransient<IProjectsService, ProjectsService>();
			services.AddTransient<ISignUpService, SignUpService>();
			services.AddTransient<ISignInService, SignInService>();
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<IArtifactsService, ArtifactsService>();
			services.AddTransient<ICategoriesService, CategoriesService>();
			services.AddTransient<IArtifactsProviderService, AwsArtifactsProviderService>();

            
			
            services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo {Title = "Five Rocking Fingers", Version = "v1"});
				c.CustomSchemaIds(i => i.FullName);
			});

            services.AddDbContext<DataAccessContext>(c =>
            {
                c.UseSqlServer(Configuration.GetConnectionString("FiveRockingFingers"));
            });

			// In production, the React files will be served from this directory
			services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

			
			services.ConfigureApplicationCookie(option =>
			{
				option.Events = new CookieAuthenticationEvents()
				{
					OnRedirectToLogin = (ctx) =>
					{
						if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
						{
							ctx.Response.StatusCode = 401;
						}

						return Task.CompletedTask;
					},
					OnRedirectToAccessDenied = (ctx) =>
					{
						if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
						{
							ctx.Response.StatusCode = 403;
						}

						return Task.CompletedTask;
					}
				};
			});

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

			app.UseCors("CorsPolicy");

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
