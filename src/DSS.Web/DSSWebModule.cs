using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DSS.EntityFrameworkCore;
using DSS.Localization;
using DSS.MultiTenancy;
using DSS.Web.Menus;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity.Web;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using DSS.Web.Pages.Configuration.Measures;
using Volo.Abp.Identity;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.EventBus.Distributed;
using DSS.AssetTypeParametersData;
using DSS.AssetTypeParameters;
using DSS.Repositories;
using DSS.OthersParameters.Dtos;
using DSS.OthersParameters;
using DSS.Counties;


namespace DSS.Web;

[DependsOn(
    typeof(DSSHttpApiModule),
    typeof(DSSApplicationModule),
    typeof(DSSEntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpSettingManagementWebModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpTenantManagementWebModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
    )]
public class DSSWebModule : AbpModule
{
 
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(DSSResource),
                typeof(DSSDomainModule).Assembly,
                typeof(DSSDomainSharedModule).Assembly,
                typeof(DSSApplicationModule).Assembly,
                typeof(DSSApplicationContractsModule).Assembly,
                typeof(DSSWebModule).Assembly
            );
        });

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("DSS");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        ConfigureAuthentication(context, configuration);
        ConfigureUrls(configuration);
        ConfigureBundles();
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices();
        ConfigureAutoApiControllers();
        ConfigureSwaggerServices(context.Services);

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<DSSWebAutoMapperProfile>(); // Add your custom AutoMapper profile
        });

        context.Services.AddScoped<IdentityUserManager>();
        context.Services.AddScoped<IMeasureRepository<Measure, Guid>, MeasureRepository>();
        context.Services.AddScoped<IAssetTypeParameterDataAppService, AssetTypeParameterDataAppService>();
        context.Services.AddScoped<IOtherParameterAppService, OtherParameterAppService>();
        context.Services.AddScoped<ICountyAppService, CountiesAppService>();
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication()
            .AddOpenIdConnect("AzureAdB2C", "Azure AD B2C", options =>
            {
                options.Authority = $"{configuration["AzureAdB2C:Instance"]}{configuration["AzureAdB2C:Domain"]}/{configuration["AzureAdB2C:SignUpSignInPolicyId"]}/v2.0/";
                options.ClientId = configuration["AzureAdB2C:ClientId"];
                options.ClientSecret = configuration["AzureAdB2C:GraphApi:ClientSecret"];
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.CallbackPath = configuration["AzureAdB2C:GraphApi:CallbackPath"];
                options.RequireHttpsMetadata = true;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("email");
                options.Scope.Add(configuration["AzureAdB2C:ClientId"]);

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
                options.SignInScheme = IdentityConstants.ExternalScheme;

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = async ticketReceivedContext =>
                    {
                        var userEmailClaim = ticketReceivedContext.Principal.FindFirstValue("emails");
                        var userName = ticketReceivedContext.Principal.FindFirstValue("name");

                        if (userEmailClaim != null)
                        {
                            var unitOfWorkManager = context.Services.GetRequiredService<IUnitOfWorkManager>();
                            var userRepository = context.Services.GetRequiredService<IRepository<Volo.Abp.Identity.IdentityUser, Guid>>();
                            var httpContextAccessor = context.Services.GetRequiredService<IHttpContextAccessor>();
                            var dataFilter = context.Services.GetRequiredService<IDataFilter>();
                            var currentTenant = context.Services.GetRequiredService<ICurrentTenant>();
                            var tenantRepository = context.Services.GetRequiredService<ITenantRepository>();
                            var tenantManager = context.Services.GetRequiredService<ITenantManager>();
                            var dataSeeder = context.Services.GetRequiredService<IDataSeeder>();
                            var distributedEventBus = context.Services.GetRequiredService<IDistributedEventBus>();
                            var identityUserManager = context.Services.GetRequiredService<IdentityUserManager>();
                            var identityRoleManager = context.Services.GetRequiredService<IdentityRoleManager>();


                            using (var uow = unitOfWorkManager.Begin(requiresNew: true))
                            {
                                Volo.Abp.Identity.IdentityUser userExists;
                                using (dataFilter.Disable<IMultiTenant>())
                                {
                                    userExists = await userRepository.FirstOrDefaultAsync(u => u.Email == userEmailClaim);
                                }
                                if (userExists == null)
                                {
                                    var guestTenant = await CheckGuestTenant(tenantRepository, tenantManager,
                                        currentTenant, dataSeeder, distributedEventBus, identityRoleManager);

                                    using (currentTenant.Change(guestTenant))
                                    {
                                        var newUser = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), userEmailClaim, userEmailClaim);
                                        newUser.Name = userName;

                                        await userRepository.InsertAsync(newUser);
                                        await unitOfWorkManager.Current.SaveChangesAsync();

                                        var guestRole = await identityRoleManager.FindByNameAsync("Guest");

                                        await identityUserManager.AddToRoleAsync(newUser, "Guest");
                                        // Optionally add roles or claims to the user here
                                    }
                                }
                                await uow.CompleteAsync();

                                // Redirect to the specified URL
                                var httpContext = httpContextAccessor.HttpContext;
                                httpContext.Response.Redirect(configuration["AzureAdB2C:RedirectUrl"]);
                            }
                        }
                        else
                        {
                            // Handle the scenario where the email claim is missing from the token
                        }
                    },

                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        var logoutUri = $"{configuration["AzureAdB2C:Instance"]}{configuration["AzureAdB2C:Domain"]}/oauth2/v2.0/logout?client_id={configuration["AzureAdB2C:ClientId"]}";
                        logoutUri += $"&post_logout_redirect_uri={Uri.EscapeDataString(configuration["AzureAdB2C:RedirectUrl"])}";
                        context.Response.Redirect(logoutUri);
                        context.HttpContext.SignOutAsync();
                        context.HandleResponse();
                        return Task.CompletedTask;
                    },

                    OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.Prompt = "login";
                        return Task.CompletedTask;
                    }
                };
            });
    }

    private async Task<Guid> CheckGuestTenant(ITenantRepository tenantRepository,
        ITenantManager tenantManager,
        ICurrentTenant currentTenant,
        IDataSeeder dataSeeder,
        IDistributedEventBus distributedEventBus,
        IdentityRoleManager identityRoleManager)
    {
        var guestTenant = await tenantRepository.FindByNameAsync("Guest");

        if (guestTenant == null)
        {
            try
            {
                var tenantDto = new TenantCreateDto
                {
                    Name = "Guest",
                    AdminEmailAddress = "guest@admin.com",
                    AdminPassword = "guest@123",
                };
                var tenant = await CreateAsync(tenantManager, tenantRepository, currentTenant, dataSeeder, distributedEventBus, tenantDto);
                await CheckRole(identityRoleManager, currentTenant, tenant);

                return tenant;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        await CheckRole(identityRoleManager, currentTenant, guestTenant.Id);
        return guestTenant.Id;
    }

    public virtual async Task<Guid> CreateAsync(ITenantManager tenantManager,
        ITenantRepository tenantRepository, 
        ICurrentTenant currentTenant,
        IDataSeeder dataSeeder,
        IDistributedEventBus distributedEventBus, TenantCreateDto input)
    {
        var tenant = await tenantManager.CreateAsync(input.Name);
        
        await tenantRepository.InsertAsync(tenant);

      
        await distributedEventBus.PublishAsync(
            new TenantCreatedEto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Properties =
                {
                        { "AdminEmail", input.AdminEmailAddress },
                        { "AdminPassword", input.AdminPassword }
                }
            });

        using (currentTenant.Change(tenant.Id, tenant.Name))
        {
            //TODO: Handle database creation?
            // TODO: Seeder might be triggered via event handler.
            await dataSeeder.SeedAsync(
                            new DataSeedContext(tenant.Id)
                                .WithProperty("AdminEmail", input.AdminEmailAddress)
                                .WithProperty("AdminPassword", input.AdminPassword)
                            );
        }

        return tenant.Id;
    }


    private async Task CheckRole(IdentityRoleManager identityRoleManager, ICurrentTenant currentTenant, Guid tenantId)
    {
        using (currentTenant.Change(tenantId))
        {
            var guestRole = await identityRoleManager.FindByNameAsync("Guest");

            if (guestRole == null)
            {
                guestRole = new Volo.Abp.Identity.IdentityRole(Guid.NewGuid(), "Guest", tenantId);
                await identityRoleManager.CreateAsync(guestRole);
            }
        }
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<DSSWebModule>();
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<DSSDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}DSS.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<DSSDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}DSS.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<DSSApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}DSS.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<DSSApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}DSS.Application"));
                options.FileSets.ReplaceEmbeddedByPhysical<DSSWebModule>(hostingEnvironment.ContentRootPath);
            });
        }
    }

    private void ConfigureNavigationServices()
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new DSSMenuContributor());
        });
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(DSSApplicationModule).Assembly);
        });
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "DSS API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
    }
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "DSS API");
        });

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();

        // Redirect to /Account/Login by default
        app.Use(async (context, next) =>
        
        {
            if (!context.Request.Path.HasValue || context.Request.Path == "/")
            {
                // If the user is not authenticated, redirect to the login page
                if (!context.User.Identity.IsAuthenticated)
                {
                    context.Response.Redirect("/Account/Login");
                    return;
                }                
                else if (context.User.IsInRole("admin"))
                {
                    context.Response.Redirect("/Identity/Users");
                    return;
                }
                else
                {
                    // If the user is authenticated, redirect to the dashboard
                    context.Response.Redirect("/Dashboard");
                    return;
                }
            }

            await next();
        });

        app.UseConfiguredEndpoints();
    }


 

}
