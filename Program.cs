using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RiskManagement.Services;


var builder = WebApplication.CreateBuilder(args);

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");

var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Missing JWT secret in configuration: Jwt:Secret");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Missing JWT issuer in configuration: Jwt:Issuer");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Missing JWT audience in configuration: Jwt:Audience");

var jwtSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Paste only your JWT access token (without 'Bearer ').",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddDbContext<RiskManagementDbContext>(options =>

    options.UseMySql(

        defaultConnection,

        ServerVersion.AutoDetect(defaultConnection)

    ));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = jwtSigningKey,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    var anyAuthenticatedUserPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.DefaultPolicy = anyAuthenticatedUserPolicy;
    options.FallbackPolicy = anyAuthenticatedUserPolicy;

    options.AddPolicy("AnyAuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());

    options.AddPolicy("AdminOrSuperadmin", policy =>
        policy.RequireAuthenticatedUser().RequireRole("Admin", "Superadmin"));

    options.AddPolicy("SuperadminOnly", policy =>
        policy.RequireAuthenticatedUser().RequireRole("Superadmin"));
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AppSeedService>();


builder.Services.AddControllers();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seedService = scope.ServiceProvider.GetRequiredService<AppSeedService>();
    await seedService.SeedAsync();
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("FrontendCors");

var allowedOriginSet = new HashSet<string>(allowedOrigins, StringComparer.OrdinalIgnoreCase);
app.Use(async (context, next) =>
{
    var method = context.Request.Method;
    var isUnsafeMethod = !(HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method));

    if (!isUnsafeMethod)
    {
        await next();
        return;
    }

    var path = context.Request.Path.Value ?? string.Empty;
    var isAuthCookieEndpoint =
        path.Equals("/api/auth/refresh", StringComparison.OrdinalIgnoreCase) ||
        path.Equals("/api/auth/logout", StringComparison.OrdinalIgnoreCase);

    if (!isAuthCookieEndpoint)
    {
        await next();
        return;
    }

    var origin = context.Request.Headers.Origin.ToString();
    if (!string.IsNullOrWhiteSpace(origin))
    {
        if (!allowedOriginSet.Contains(origin))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("CSRF blocked: invalid Origin.");
            return;
        }

        await next();
        return;
    }

    var referer = context.Request.Headers.Referer.ToString();
    var refererAllowed = !string.IsNullOrWhiteSpace(referer) &&
                         allowedOrigins.Any(allowed =>
                             referer.StartsWith(allowed, StringComparison.OrdinalIgnoreCase));

    if (!refererAllowed)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("CSRF blocked: missing/invalid Referer.");
        return;
    }

    await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();