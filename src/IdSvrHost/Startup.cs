using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.Extensions.Serialization;
using Couchbase.Linq;
using Couchbase.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Couchbase.Services;
using Identity.Couchbase;
using IdentityServer4.Couchbase;
using Microsoft.AspNetCore.Http;

namespace IdSvrHost
{
    public class Startup
    {
        readonly IHostingEnvironment _environment;
        readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");
            _configuration = builder.Build();

            _environment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "idsrv4test.pfx"), "idsrv3test");           

            services.AddSingleton(p =>
            {
                var loggerFactory = p.GetService<ILoggerFactory>();

                var logger = loggerFactory.CreateLogger("Couchbase");

                ClusterHelper.Initialize(new ClientConfiguration(logger)
                {
                    Servers = new List<Uri>()
                    {
                        new Uri(_configuration["couchbase:server"])
                    },
                    // We use our own serializer wrapper to inset the claims converter
                    Serializer = IdentityServerCouchbaseSerializer.GetSerializer()
                }, logger);

                return ClusterHelper.GetBucket(_configuration["couchbase:bucket"]);
            });
            services.AddSingleton<IBucketContext>(p =>
            {
                var loggerFactory = p.GetService<ILoggerFactory>();

                var logger = loggerFactory.CreateLogger("Couchbase.Linq");

                return new BucketContext(p.GetService<IBucket>(), logger);
            });
            services.AddSingleton(p => p.GetService<IBucket>().CreateManager(_configuration["couchbase:username"],
                    _configuration["couchbase:password"]));

            services.AddIdentityServer(options =>
                {
                    options.UserInteractionOptions.LoginUrl = "/ui/login";
                    options.UserInteractionOptions.LogoutUrl = "/ui/logout";
                    options.UserInteractionOptions.ConsentUrl = "/ui/consent";
                    options.UserInteractionOptions.ErrorUrl = "/ui/error";
                })
                .SetSigningCredential(cert)
                .AddCouchbaseClients()
                .AddCouchbaseScopes()
                .AddCouchbaseUsers<CouchbaseUser>()
                .AddCustomGrantValidator<DeviceGrantValidator>();

            services.AddIdentity<CouchbaseUser, CouchbaseRole>()
                .AddCouchbaseStores<CouchbaseUser, CouchbaseRole>();

            services.AddLogging();

            // for the UI
            services
                .AddMvc()
                .AddRazorOptions(razor =>
                {
                    razor.ViewLocationExpanders.Add(new UI.CustomViewLocationExpander  ());
                });

            services.AddTransient<UI.Login.LoginService>();

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IBucketManager bucketManager, ICouchbaseClientStore clientStore, ICouchbaseScopeStore scopeStore)
        {
            loggerFactory.AddConsole(_configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            bucketManager.CreateN1qlPrimaryIndex("primary", false);

            // Our client for the Mvc sample site
            var client = new Client
            {
                ClientId = "mvc_implicit",
                ClientName = "MVC Implicit",
                ClientUri = "http://identityserver.io",

                RedirectUris = new List<string>
                {
                    "http://localhost:64538/signin-oidc"
                },                
                AllowAccessTokensViaBrowser = true,
                AllowedScopes = new List<string>
                {
                    StandardScopes.OpenId.Name,
                    StandardScopes.Profile.Name,
                    StandardScopes.Email.Name,
                    StandardScopes.Roles.Name,

                    "api1",
                    "api2"
                },               
            };



            clientStore.StoreClientAsync(client);
            
            var device = new Client
            {
                ClientId = "device",
                ClientName = "Device",
                ClientUri = "http://identityserver.io",

               // Flow = Flows.Custom,

                AllowedGrantTypes = new List<string>()
                {
                    "device",
                },
                AllowedScopes = new List<string>
                {
                    StandardScopes.OpenId.Name,
                    StandardScopes.Profile.Name,
                    StandardScopes.Email.Name,
                    StandardScopes.Roles.Name,

                    "api1",
                    "api2"
                },
                AlwaysSendClientClaims =  true,
                ClientSecrets = new List<Secret>()
                {
                    new Secret("test".Sha256(), "test")
                },
                Claims = new List<Claim>()
                {
                    new Claim("test2", "test2")
                }
            };
            clientStore.StoreClientAsync(device);

            // Scopes supported by IdentityServer
            scopeStore.StoreScopeAsync(StandardScopes.OpenId);
            scopeStore.StoreScopeAsync(StandardScopes.Profile);
            scopeStore.StoreScopeAsync(StandardScopes.Email);
            scopeStore.StoreScopeAsync(StandardScopes.Roles);

            var apiScope = new Scope
            {
                Name = "api1",
                DisplayName = "API 1",
                Description = "API 1 features and data",
                Type = ScopeType.Identity,
                IncludeAllClaimsForUser = true,
                ScopeSecrets = new List<Secret>
                {
                    new Secret("secret".Sha256())
                },
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim("role"),
                    new ScopeClaim("ross"),
                    new ScopeClaim("test"),
                    new ScopeClaim("test2")
                }                
            };

            scopeStore.StoreScopeAsync(apiScope);

            app.UseIdentity();

            var userManager = app.ApplicationServices.GetService<UserManager<CouchbaseUser>>();
            var result = userManager.CreateAsync(new CouchbaseUser()
            {
                Username = "AliceSmith@email.com",
                Email = "AliceSmith@email.com",
                SubjectId = Guid.NewGuid().ToString(),
                Claims = new List<Claim>()
                {
                      new Claim(JwtClaimTypes.Name, "AliceSmith@email.com"),
                }
            }, "Alice1#").Result;

            loggerFactory.AddDebug();

            app.UseMiddleware<ErrorCodeMiddleware>();

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }

    public class ErrorCodeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorCodeMiddleware(RequestDelegate next, ILogger<ErrorCodeMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                httpContext.Response.Headers.Remove("Server");
                await _next(httpContext);
            } 
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError("Unauthorized", ex);
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("Bad Request", ex);
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal Server Error", ex);
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}
