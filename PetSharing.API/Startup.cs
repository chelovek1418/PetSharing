using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUnitOfWork, EFUnitOfWork>();
            services.AddTransient<IService<PetProfileDto>, PetProfileService>();
            services.AddTransient<IService<PostDto>, PostService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var cnct = Configuration.GetConnectionString("PetSharingDb");

            services.AddDbContext<PetSharingDbContext>(options =>
                options.UseSqlServer(cnct, b => b.MigrationsAssembly("PetSharing.Data")));

            services.AddIdentity<User, IdentityRole>(
                opts =>
                {
                    opts.Password.RequiredLength = 5;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<PetSharingDbContext>()
                .AddDefaultTokenProviders();

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSession();

            app.UseCors(builder => builder.WithOrigins("http://localhost:8080").AllowAnyMethod().AllowAnyHeader());
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