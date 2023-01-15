using AskMeAQuestion.Data;
using AskMeAQuestion.Services;
using AskMeAQuestion.Services.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;

namespace AskMeAQuestion
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(option =>
            {
#if DEBUG
                var cn = Configuration.GetConnectionString("dbcn");
                option.UseSqlServer(cn)
                    .EnableSensitiveDataLogging();
#endif

#if RELEASE
                var cn = Configuration.GetConnectionString("rlcn");
                option.UseSqlServer(cn);
#endif
            });

            services.AddAuthentication("AskMeQuestionCookie")
                .AddCookie("AskMeQuestionCookie", options =>
                {
                    options.LogoutPath = "/home/Index";
                    options.LoginPath = "/home/Login";
                    options.AccessDeniedPath = "/home/AccessDenied";
                    options.ReturnUrlParameter = "returnUrl";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                });

            services.AddScoped<ISubscriptionService, SubscriptionService>();

            services.AddScoped<ILoginService, LoginService>();

            services.AddScoped<IPollService, PollService>();

            services.AddScoped<IVoteService, VoteService>();

            services.AddHttpContextAccessor();

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Lors d'un déploiement en ligne
            var cultureInfo = new CultureInfo("fr-FR");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
