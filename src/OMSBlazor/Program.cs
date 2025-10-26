using BitzArt.Blazor.Cookies;
using BoldReports.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using OMSBlazor.Client.Constants;
using OMSBlazor.Client.Pages.Dashboard.CustomerStastics;
using OMSBlazor.Client.Pages.Dashboard.EmployeeStastics;
using OMSBlazor.Client.Pages.Dashboard.OrderStastics;
using OMSBlazor.Client.Pages.Dashboard.ProductStastics;
using OMSBlazor.Client.Pages.Order.Create;
using OMSBlazor.Client.Pages.Order.Journal;
using OMSBlazor.Client.Pages.Report;
using OMSBlazor.Client.Services.HubConnectionsService;
using OMSBlazor.Client.Services.StatisticsReader;
using OMSBlazor.Components;
using OMSBlazor.Components.Account;
using OMSBlazor.Data;
using OMSBlazor.Services.Hosted;
using Reporting.Pages.Services;
using Reporting.Services;
using StripeModule.Pages;

namespace OMSBlazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Bold.Licensing.BoldLicenseProvider.RegisterLicense("gFVmCnZi2bVTJyccaSRxm5thTNY+P9ONI5ME6zQR0p0=");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient(Constants.BaseHttpClientTitel, x =>
            {
                x.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? throw new NullReferenceException());
            });
            // Add MudBlazor services
            builder.Services.AddMudServices();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            builder.Services.AddMemoryCache();

            builder.Services.AddSingleton<IHubConnectionsService, HubConnectionsService>();
            builder.Services.AddSingleton<IStatisticsDataReader, StatisticsDataReader>();

            builder.Services.AddScoped<CustomerStasticsViewModel>();
            builder.Services.AddScoped<EmployeeStasticsViewModel>();
            builder.Services.AddScoped<OrderStasticsViewModel>();
            builder.Services.AddScoped<ProductStasticsViewModel>();
            builder.Services.AddScoped<JournalViewModel>();
            builder.Services.AddScoped<CreateViewModel>();
            builder.Services.AddSingleton<IJsonDataSourceUpdater, JsonDataSourceUpdater>();
            builder.Services.AddHostedService<JsonReportDataSourceSeeder>();

            builder.Services.AddHttpClient<CreateViewModel>(x =>
            {
                x.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? throw new NullReferenceException());
            });
            builder.Services.AddHttpClient<JournalViewModel>(x =>
            {
                x.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? throw new NullReferenceException());
            });
            builder.Services.AddHttpClient<CustomerStasticsViewModel>(x=>
            {
                x.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? throw new NullReferenceException());
            });
            builder.Services.AddHttpClient<EmployeeStasticsViewModel>(x =>
            {
                x.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? throw new NullReferenceException());
            });
            builder.Services.AddHttpClient<OrderStasticsViewModel>(x =>
            {
                x.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? throw new NullReferenceException());
            });
            builder.Services.AddHttpClient<ProductStasticsViewModel>(x =>
            {
                x.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? throw new NullReferenceException());
            });

            builder.AddBlazorCookies();

            builder.Services.AddControllers();

            var app = builder.Build();

            ReportConfig.DefaultSettings = new ReportSettings().RegisterExtensions(new List<string> {"BoldReports.Data.WebData",
                                                                                        "BoldReports.Data.PostgreSQL",
                                                                                        "BoldReports.Data.Excel",
                                                                                        "BoldReports.Data.Csv",
                                                                                        "BoldReports.Data.Oracle",
                                                                                        "BoldReports.Data.ElasticSearch",
                                                                                        "BoldReports.Data.Snowflake",
                                                                                        "BoldReports.Data.SSAS"});

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.MapStaticAssets();
            app.UseStaticFiles();

            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly, typeof(StripeModule.Pages.OrderPaid).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();
            
            app.MapControllers();

            app.Run();
        }
    }
}
