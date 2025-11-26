using FluentValidation;
using GymWebApp.ApplicationCore.Common;
using GymWebApp.ApplicationCore.CQRS.GroupTraining;
using GymWebApp.ApplicationCore.Services;
using GymWebApp.ApplicationCore.Services.Interfaces;
using GymWebApp.Data;
using GymWebApp.Data.Entities;
using GymWebApp.Data.Repositories;
using GymWebApp.Data.Repositories.Interfaces;
using GymWebApp.WebAPI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Gym Web App...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/gym-app-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30));

    ConfigureServices(builder.Services, builder.Configuration);

    var app = builder.Build();

    ConfigurePipeline(app);

    await app.InitializeDatabaseAsync();

    Log.Information("Gym Web App started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularDevClient", builder =>
        {
            builder
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(GetGroupTrainingsFilteredQueryHandler).Assembly));

    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidatorBase<,>));

    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    services.AddScoped(typeof(IGroupTrainingRepository), typeof(GroupTrainingRepository));
    services.AddScoped(typeof(IIndividualTrainingRepository), typeof(IndividualTrainingRepository));
    services.AddScoped(typeof(IShiftRepository), typeof(ShiftRepository));

    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    services.AddScoped<IJwtTokenService, JwtTokenService>();

    var jwtKey = configuration["Jwt:Key"];
    var jwtIssuer = configuration["Jwt:Issuer"];
    
    if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
    {
        throw new InvalidOperationException("JWT configuration is missing. Please check appsettings.json");
    }

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

    services.Configure<IdentityOptions>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
    });

    services.AddAuthorization();

    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}

static void ConfigurePipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
        };
    });

    app.UseHttpsRedirection();
    app.UseCors("AllowAngularDevClient");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}
