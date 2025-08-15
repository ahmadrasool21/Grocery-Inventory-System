// using System.Text;
// using FluentValidation;
// using FluentValidation.AspNetCore;
// using GroceryInventory.Api.Auth;
// using GroceryInventory.Api.Middleware;
// using GroceryInventory.Infrastructure;
// using GroceryInventory.Infrastructure.Persistence;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.OpenApi.Models;

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddEndpointsApiExplorer();

// // Swagger + Bearer auth
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "Grocery Inventory API", Version = "v1" });
//     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         In = ParameterLocation.Header,
//         Name = "Authorization",
//         Type = SecuritySchemeType.Http,
//         Scheme = "bearer",
//         BearerFormat = "JWT",
//         Description = "JWT Authorization header using the Bearer scheme."
//     });
//     c.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
//             },
//             Array.Empty<string>()
//         }
//     });
// });

// builder.Services.AddControllers();

// // FluentValidation
// builder.Services.AddFluentValidationAutoValidation();
// // Validators are in the Application assembly
// builder.Services.AddValidatorsFromAssemblyContaining<GroceryInventory.Application.Validation.ProductDtoValidator>();

// // JWT settings
// var jwtSection = builder.Configuration.GetSection("Jwt");
// builder.Services.Configure<JwtSettings>(jwtSection);
// var jwtSettings = jwtSection.Get<JwtSettings>()!;

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = jwtSettings.Issuer,
//             ValidAudience = jwtSettings.Audience,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
//             ClockSkew = TimeSpan.Zero
//         };
//     });

// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("Admin", p => p.RequireRole("Admin"));
//     options.AddPolicy("ClerkOrAdmin", p => p.RequireRole("Clerk", "Admin"));
// });

// var cs = builder.Configuration.GetConnectionString("DefaultConnection");
// var useSqlite = builder.Configuration.GetValue<bool>("UseSqlite");

// builder.Services.AddInfrastructure(cs, useSqlite);

// var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseGlobalExceptionHandler();

// app.UseAuthentication();
// app.UseAuthorization();


// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     if (!app.Environment.IsEnvironment("Testing"))
//     {
//         db.Database.Migrate();
//     }
// }

// app.MapControllers();
// app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// app.Run();

// public partial class Program { }


using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using GroceryInventory.Api.Auth;
using GroceryInventory.Api.Middleware;
using GroceryInventory.Infrastructure;
using GroceryInventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

// Swagger + Bearer auth
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Grocery Inventory API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
// Validators are in the Application assembly
builder.Services.AddValidatorsFromAssemblyContaining<GroceryInventory.Application.Validation.ProductDtoValidator>();

// JWT settings
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>()!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", p => p.RequireRole("Admin"));
    options.AddPolicy("ClerkOrAdmin", p => p.RequireRole("Clerk", "Admin"));
});

var cs = builder.Configuration.GetConnectionString("DefaultConnection");
var useSqlite = builder.Configuration.GetValue<bool>("UseSqlite");

builder.Services.AddInfrastructure(cs, useSqlite);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

// ✅ FIX: Declare and use scope properly
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!app.Environment.IsEnvironment("Testing"))
    {
        db.Database.Migrate();
    }
}

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program { }
