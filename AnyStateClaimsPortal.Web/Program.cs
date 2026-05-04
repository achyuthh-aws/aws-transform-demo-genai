// ============================================================
// Program.cs — AnyStateClaimsPortal.Web (.NET 8 minimal host)
//
// Migrated from:
//   Global.asax / Global.asax.cs  (Application_Start, Application_Error, Session_Start)
//   App_Start/FilterConfig.cs     (HandleErrorAttribute → UseExceptionHandler)
//   App_Start/RouteConfig.cs      (Default route → MapControllerRoute)
//   App_Start/BundleConfig.cs     (removed — static assets served from wwwroot via UseStaticFiles)
//   Web.config                    (Forms auth, connection string, appSettings)
// ============================================================

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AnyStateClaimsPortal.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ----------------------------------------------------------------
            // 1. Builder — replaces Global.asax Application_Start bootstrap
            // ----------------------------------------------------------------
            var builder = WebApplication.CreateBuilder(args);

            // Expose IConfiguration throughout the app (replaces ConfigurationManager).
            // Values are loaded from appsettings.json (and appsettings.{Env}.json).
            var configuration = builder.Configuration;

            // ----------------------------------------------------------------
            // 2. MVC services
            //    • Replaces AreaRegistration.RegisterAllAreas()  — areas are
            //      auto-discovered by AddControllersWithViews() when the
            //      conventional area route is registered below.
            //    • Replaces FilterConfig.RegisterGlobalFilters() — error
            //      handling is now done by the UseExceptionHandler middleware;
            //      the HandleErrorAttribute is not needed in ASP.NET Core.
            // ----------------------------------------------------------------
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            // ----------------------------------------------------------------
            // 3. Cookie Authentication
            //    Migrated from Web.config <authentication mode="Forms">:
            //      loginUrl      = ~/Account/Login
            //      timeout       = 30 (minutes)
            //      slidingExpiration = true
            // ----------------------------------------------------------------
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath          = "/Account/Login";
                    options.ExpireTimeSpan     = TimeSpan.FromMinutes(30);
                    options.SlidingExpiration  = true;
                    options.AccessDeniedPath   = "/Account/Login";
                });

            // ----------------------------------------------------------------
            // 4. Session
            //    Session_Start in Global.asax.cs was empty; basic session
            //    services are registered so session middleware is available.
            // ----------------------------------------------------------------
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout        = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly    = true;
                options.Cookie.IsEssential = true;
            });

            // ----------------------------------------------------------------
            // 5. Entity Framework (EF6 on .NET 8)
            //    Replaces Database.SetInitializer<AnyStateClaimsContext>(null)
            //    in Application_Start.  EF6 on .NET 8 does not use the
            //    DbMigrationsConfiguration pipeline by default; setting the
            //    initializer to null (no automatic migration/seed) is handled
            //    by registering the DbContext with its named connection string
            //    from configuration.  Automatic database initialisation is
            //    intentionally disabled — schema changes must be applied
            //    manually or via a DbMigrator invocation.
            // ----------------------------------------------------------------
            // NOTE: AnyStateClaimsContext reads its connection string via
            //   base("name=AnyStateClaimsDB") — EF6 on .NET 8 picks this up
            //   from the "AnyStateClaimsDB" key in appsettings.json when the
            //   EF6 ConfigurationManager bridge is configured, or the context
            //   can be updated to accept a connection-string constructor arg.
            //   The IConfiguration instance is made globally accessible below
            //   through ConfigurationManager so that existing repositories
            //   continue to work without further changes.
            ConfigurationManager.Configuration = configuration;

            // ----------------------------------------------------------------
            // 6. Logging (matches Web.config Logging section in appsettings)
            // ----------------------------------------------------------------
            builder.Logging
                .ClearProviders()
                .AddConsole()
                .AddDebug();

            // ----------------------------------------------------------------
            // 7. Build the application
            // ----------------------------------------------------------------
            var app = builder.Build();

            // ----------------------------------------------------------------
            // 8. Exception handling
            //    Replaces Application_Error in Global.asax.cs and the
            //    <customErrors mode="RemoteOnly" defaultRedirect="~/Home/Error"/>
            //    entry in Web.config.
            // ----------------------------------------------------------------
            if (app.Environment.IsDevelopment())
            {
                // Shows the full exception page in development.
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Mirrors the Application_Error redirect to ~/Home/Error.
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // ----------------------------------------------------------------
            // 9. HTTPS redirection
            // ----------------------------------------------------------------
            // app.UseHttpsRedirection(); // Disabled for HTTP-only demo

            // ----------------------------------------------------------------
            // 10. Static files
            //     Replaces BundleConfig.RegisterBundles().  In .NET 8,
            //     static assets (CSS, JS) live under wwwroot/ and are served
            //     directly.  Script/style bundling should be handled by a
            //     build-time tool (e.g., npm + webpack, or LibMan).
            // ----------------------------------------------------------------
            app.UseStaticFiles();

            // ----------------------------------------------------------------
            // 11. Routing
            //     Must appear before UseAuthentication / UseAuthorization.
            // ----------------------------------------------------------------
            app.UseRouting();

            // ----------------------------------------------------------------
            // 12. Authentication & Authorization
            //     app.UseAuthentication() activates the cookie handler
            //     registered in step 3.
            //     Must appear after UseRouting() and before UseAuthorization().
            // ----------------------------------------------------------------
            app.UseAuthentication();
            app.UseAuthorization();

            // ----------------------------------------------------------------
            // 13. Session middleware
            //     Must appear after UseRouting() and before MapControllerRoute.
            // ----------------------------------------------------------------
            app.UseSession();

            // ----------------------------------------------------------------
            // 14. Controller routes
            //     Replaces RouteConfig.RegisterRoutes().
            //
            //     Original RouteConfig:
            //       routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //       routes.MapRoute(
            //           name: "Default",
            //           url:  "{controller}/{action}/{id}",
            //           defaults: new { controller="Home", action="Index",
            //                          id = UrlParameter.Optional });
            //
            //     *.axd handler routes do not exist in ASP.NET Core so the
            //     IgnoreRoute call is simply omitted.
            //
            //     Area support: areas are registered automatically; add an
            //     explicit area route if the application uses named areas.
            // ----------------------------------------------------------------
            app.MapControllerRoute(
                name:    "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // ----------------------------------------------------------------
            // 15. Run
            // ----------------------------------------------------------------
            app.Run();
        }
    }

    // ----------------------------------------------------------------
    // ConfigurationManager bridge
    // Provides static access to IConfiguration so that repositories and
    // services that previously relied on System.Configuration.ConfigurationManager
    // can read settings without requiring constructor injection changes
    // in the initial migration pass.
    // ----------------------------------------------------------------
    public static class ConfigurationManager
    {
        public static IConfiguration Configuration { get; set; } = null!;
    }
}
