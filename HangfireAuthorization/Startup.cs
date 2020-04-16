using System.IdentityModel.Tokens.Jwt;
using Hangfire;
using HangfireAuthorization.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace HangfireAuthorization
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //This would typically be the package extension method for MS
            services.AddSingleton<IMembershipService, MembershipService>();

            services.AddHangfire(opt => opt.UseSqlServerStorage("Server=(local);Database=HangfireTest;Integrated Security=SSPI;"));

            //Everything else in this method is standard OIDC implementation covered by the UWS auth package
            services.AddControllersWithViews(options => {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });

            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "https://demo.identityserver.io";
                options.RequireHttpsMetadata = false;
                options.ClientId = "interactive.confidential";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                options.SaveTokens = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IMembershipService membershipService)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireServer();
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter(membershipService) }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
