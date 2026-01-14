using System.Text;
using ChatService.Auth;
using ChatService.Data;
using ChatService.Hubs;
using ChatService.Services;
using ChatService.Models.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

/// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// EF Core + SQL Server
// EF Core + SQL Server or In-Memory based on config
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    var useInMemory = builder.Configuration.GetValue<bool>("Database:UseInMemory");
    if (useInMemory)
    {
        var inMemoryName = builder.Configuration.GetValue<string>("Database:InMemoryName") ?? "ChatServiceInMemory";
        opt.UseInMemoryDatabase(inMemoryName);
    }
    else
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Password hasher
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<User>,
    Microsoft.AspNetCore.Identity.PasswordHasher<User>>();

// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddSingleton<JwtTokenService>();

// Controllers + SignalR
builder.Services.AddControllers();
builder.Services.AddSignalR();

// CORS for front-end
const string CorsPolicy = "AllowVueApp";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",      // Vite dev server
                "http://localhost:5174",      // Alternative port
                "http://localhost:5175"      // Alternative port
                )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// JWT Auth
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ClockSkew = TimeSpan.Zero
        };
        // SignalR token handling from query string
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                    context.Token = accessToken;
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception?.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully. User: " +
                    $"{context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();