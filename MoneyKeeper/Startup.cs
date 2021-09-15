using MoneyKeeper.Authentication.Helpers;
using BL;
using DAL.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoneyKeeper.BL;
using MoneyKeeper.DAL;
using MoneyKeeper.Providers;
using MoneyKeeper.Authentication;
using MoneyKeepeer.Authentication;
using System.Text;

namespace MoneyKeeper
{
	public class Startup
	{
		private readonly string allowFrontendPolicy = "allowFrontend";

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(options =>
					{
						options.RequireHttpsMetadata = true;
						options.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuer = true,
							ValidIssuer = JwtAuthOptions.ISSUER,
							ValidateAudience = true,
							ValidAudience = JwtAuthOptions.AUDIENCE,
							ValidateLifetime = true,
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
								Configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>().Secret)
							),
							ValidateIssuerSigningKey = true,
						};
					});

			services.AddCors(options =>
			{
				options.AddPolicy(allowFrontendPolicy, builder =>
				{
					builder.WithOrigins("https://gifted-minsky-943f80.netlify.app").AllowAnyHeader().AllowCredentials().AllowAnyMethod();
					builder.WithOrigins("https://localhost:4200").AllowAnyHeader().AllowCredentials().AllowAnyMethod();
				});
			});

			services.AddHttpContextAccessor();

			services.AddMemoryCache();

			services.ConfigureApiServices();
			services.ConfigureAuthServices();
			services.ConfigureRepos();

			services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

			services.Configure<AuthSettings>(Configuration.GetSection(nameof(AuthSettings)));
			services.Configure<DapperSettings>(Configuration.GetSection(nameof(DapperSettings)));

			services.AddControllers(options =>
			{
				options.SuppressAsyncSuffixInActionNames = false;
			});
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "MoneyKeeper", Version = "v1" });

			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MoneyKeeper v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors(allowFrontendPolicy);

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
