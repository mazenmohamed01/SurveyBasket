using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Context;
using System.Reflection;
using System.Text;

namespace SurveyBasket.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddControllers();

        // Add CORS policy
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder => builder.AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
            );
        });

        services.AddAuthenticationConfiguration(configuration);


        var ConnectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


        // Add DbContext with SQL Server
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(ConnectionString));


        services
            .AddSwaggerServices()
            .AddMapsterConfiguration()
            .AddFluentValidationConfiguration();

        //add services
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService, ResultService>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
    private static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
    {

        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        return services;
    }
    private static IServiceCollection AddFluentValidationConfiguration(this IServiceCollection services)
    {

        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services,
       IConfiguration configuration )
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddSingleton<IJwtProvider, JwtProvider>();


        //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
            

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)) // Replace with your actual secret key
            };
        });

        services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequiredLength = 8;
            //options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });

        return services;
    }

}
