using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// MVC & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
 c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
 var bearer = new OpenApiSecurityScheme
 {
 Name = "Authorization",
 Type = SecuritySchemeType.Http,
 Scheme = "bearer",
 BearerFormat = "JWT",
 In = ParameterLocation.Header,
 Description = "Paste the JWT token (no 'Bearer ' prefix)",
 Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
 };
 c.AddSecurityDefinition("Bearer", bearer);
 c.AddSecurityRequirement(new OpenApiSecurityRequirement { { bearer, Array.Empty<string>() } });
});


builder.Services.AddExceptionHandler<CustomExceptionHandler>();


var appAssembly = typeof(Application.Commands.CreateUserCommand).Assembly;
builder.Services.AddMediatR(cfg =>
{
 cfg.RegisterServicesFromAssembly(appAssembly);
 cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
 cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(appAssembly);

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
 opt.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

// JWT auth
var jwt = builder.Configuration.GetSection("JwtSettings");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

builder.Services
 .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
 options.MapInboundClaims = false; // keep standard claim names like "sub"
 options.TokenValidationParameters = new TokenValidationParameters
 {
 ValidateIssuer = true,
 ValidateAudience = true,
 ValidateIssuerSigningKey = true,
 ValidateLifetime = true,
 ValidIssuer = jwt["Issuer"],
 ValidAudience = jwt["Audience"],
 IssuerSigningKey = key,
 ClockSkew = TimeSpan.FromSeconds(30)
 };
 });

// Require auth by default (use [AllowAnonymous] where needed)
builder.Services.AddAuthorization(options =>
{
 options.FallbackPolicy = new AuthorizationPolicyBuilder()
 .RequireAuthenticatedUser()
 .Build();
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
 try
 {
 var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
 db.Database.Migrate();
 }
 catch (Exception ex)
 {
 var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
 logger.LogError(ex, "Database migration failed");
 throw;
 }
}

if (app.Environment.IsDevelopment())
{
 app.UseSwagger();
 app.UseSwaggerUI();
}

app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
