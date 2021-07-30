using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoneyKeeper.Repositories;
using MoneyKeeper.Repositories.Categories;
using MoneyKeeper.Services;
using MoneyKeeper.Settings;

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
						options.RequireHttpsMetadata = false;
						options.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuer = true,
							ValidIssuer = JwtAuthOptions.ISSUER,
							ValidateAudience = true,
							ValidAudience = JwtAuthOptions.AUDIENCE,
							ValidateLifetime = true,
							IssuerSigningKey = JwtAuthOptions.SymmetricSecurityKey,
							ValidateIssuerSigningKey = true,
						};
			});

			services.AddCors(options =>
			{
				options.AddPolicy(allowFrontendPolicy, builder =>
				{
					builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowCredentials().AllowAnyMethod();
				});
			});

			services.AddSingleton<IUserService, UserService>();
			services.AddSingleton<ICategoryService, CategoryService>();
			services.AddSingleton<IDapperRepository, DapperRepository>();
			services.AddSingleton<IUsersRepository, DapperUsersRepository>();
			services.AddSingleton<ICategoriesRepository, DapperCategoriesRepository>();

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
