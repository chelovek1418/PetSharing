using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PetSharing.API.SignalR;
using PetSharing.Data.Contexts;
using PetSharing.Data.Entities;
using PetSharing.Data.UnitOfWorks;
using PetSharing.Domain.Dtos;
using PetSharing.Domain.Models;
using PetSharing.Domain.Services;

namespace PetSharingAPI
{
    public class Startup
    {
        private readonly string PetSharingDbConnection = nameof(PetSharingDbConnection);
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();
            services.AddCors();
            //services.AddTransient<IUserIdProvider, CustomUserIdProvider>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUnitOfWork, EFUnitOfWork>();
            services.AddScoped<IService<PetProfileDto>, PetProfileService>();
            services.AddScoped<IService<PostDto>, PostService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var cnct = Configuration.GetConnectionString("PetSharingDb");

            services.AddDbContext<PetSharingDbContext>(options => options.UseSqlServer(cnct, b => b.MigrationsAssembly("PetSharing.Data")));

            services.AddIdentity<User, IdentityRole>(
                opts =>
                {
                    opts.Password.RequiredLength = 5;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequireUppercase = false;
                    opts.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<PetSharingDbContext>()
                .AddDefaultTokenProviders();

            services.AddSignalR();

            var key = Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:JWT_Secret"].ToString());
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSession();

            app.UseCors(builder => builder.WithOrigins(Configuration["ApplicationSettings:Client_URL"]).AllowAnyMethod().AllowAnyHeader());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                //{
                //    HotModuleReplacement = true
                //});
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller=Home}/{action=Index}/{id?}");
            });
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chat");
            });
        }
    }
}