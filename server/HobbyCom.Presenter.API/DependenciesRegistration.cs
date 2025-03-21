using HobbyCom.Application.src.IServices;
using HobbyCom.Application.src.Services;
using HobbyCom.Presenter.API.src.Services;
using HobbyCom.Domain.src.IRepositories;
using HobbyCom.Infrastructure.src.Databases;
using HobbyCom.Infrastructure.src.Repositories;
using HobbyCom.Presenter.API.src.HostedServices;
using Microsoft.EntityFrameworkCore;
using HobbyCom.Presenter.API.src.Middlewares;
using Supabase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace HobbyCom.Presenter.API
{
    public static class DependenciesRegistration
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterDBContextAndRepos(services, configuration);

            RegisteredServices(services, configuration);

            RegisterAuthentication(services, configuration);

            RegisterMiddlewares(services);

            RegisterHttpClients(services, configuration);

            return services;
        }

        private static void RegisterHttpClients(IServiceCollection services, IConfiguration configuration)
        {
            // Configure named HttpClient for Supabase
            services.AddHttpClient("SupabaseClient", client =>
            {
                client.BaseAddress = new Uri(configuration["Supabase:Url"] ?? throw new ArgumentNullException("Supabase:Url"));
                client.DefaultRequestHeaders.Add("apikey", configuration["Supabase:Key"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Register IHttpClientFactory
            services.AddHttpClient();
        }


        private static void RegisterAuthentication(IServiceCollection services, IConfiguration configuration)
        {

            // Retrieve RSA keys from RSAKeysService
            // var rsaKeysService = services.BuildServiceProvider().GetService<IJwtRsaKeysService>();

            // // Get RSA keys
            // var validationKey = new RsaSecurityKey(rsaKeysService!.ValidationKey); // Public Key


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    {
                        var rsaKeysService = services.BuildServiceProvider().GetRequiredService<IJwtRsaKeysService>();
                        return new[] { new RsaSecurityKey(rsaKeysService!.ValidationKey) };
                    },

                    // ValidIssuer = configuration["Authentication:ValidIssuer2"],
                    ValidIssuer = configuration["Authentication:ValidIssuer"],
                    ValidAudience = configuration["Authentication:ValidAudience"],
                    // IssuerSigningKey = validationKey,
                    ClockSkew = TimeSpan.Zero
                };

                opts.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        try
                        {
                            // 2. Validate required claims
                            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            var sessionIdClaim = context.Principal?.FindFirst("SessionId")?.Value;

                            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(sessionIdClaim))
                            {
                                context.Fail("Missing user/session identifier");
                                return;
                            }

                            // 3. Parse GUIDs safely
                            if (!Guid.TryParse(userId, out var userIdGuid))
                            {
                                context.Fail("Invalid user ID format");
                                return;
                            }

                            if (!Guid.TryParse(sessionIdClaim, out var sessionIdGuid))
                            {
                                context.Fail("Invalid session ID format");
                                return;
                            }

                            // 4. Validate session
                            var sessionRepo = context.HttpContext.RequestServices
                                .GetRequiredService<ISessionRepository>();

                            var session = await sessionRepo.GetSessionByIdAndUserId(sessionIdGuid, userIdGuid);

                            if (session == null)
                            {
                                context.Fail("Invalid session");
                                return;
                            }

                            // 5. Validate refresh token
                            var refreshTokenRepo = context.HttpContext.RequestServices
                                .GetRequiredService<IRefresh_TokenRepository>();

                            var TokenRevoked = false;

                            var refreshToken = await refreshTokenRepo.GetTokenByUserSessionActive(session.User_Id, session.Id, TokenRevoked);
                            if (refreshToken == null)
                            {
                                context.Fail("Invalid refresh token");
                                return;
                            }

                            if (refreshToken.Token_Revoked)
                            {
                                context.Fail("Revoked token");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Validation error: {ex}");
                            context.Fail("Internal error");
                        }
                    }
                };
            });
        }

        private static void RegisterMiddlewares(IServiceCollection services)
        {
            services.AddTransient<GlobalExceptionMiddleware>();


        }

        private static void RegisteredServices(IServiceCollection services, IConfiguration configuration)
        {

            // Hosted Services work in the background
            services.AddHostedService<DatabaseConnectionCheckService>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IPaswdService, PaswdService>();

            services.AddScoped<IJwtRsaKeysService, JwtRsaKeysService>();

            services.AddScoped<ISessionService, SessionService>();

            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        }

        private static void RegisterDBContextAndRepos(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SupabaseContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("SessionConnection"),
                    npgsqlOptions =>
                    {
                        npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        npgsqlOptions.CommandTimeout(60);
                    }
                )
            );

            // Register Supabase Client to use supabase capabilities such as Realtime, Auth, Storage, etc.
            services.AddSingleton<Client>((provider) =>
            {
                var url = configuration["Supabase:Url"] ?? throw new ArgumentNullException(nameof(configuration), "Supabase URL cannot be null");
                var key = configuration["Supabase:Key"];

                return new Client(
                    url,
                    key,
                    new SupabaseOptions
                    {
                        AutoRefreshToken = false,
                        AutoConnectRealtime = true
                    });
            });

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IRefresh_TokenRepository, Refresh_TokenRepository>();

            services.AddScoped<ISessionRepository, SessioRepository>();

        }
    }
}