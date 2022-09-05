using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using OnlineMessanger.Helpers;
using OnlineMessanger.Models;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureKeyVault(new Uri(Environment.GetEnvironmentVariable("VAULT_NAME")!), new DefaultAzureCredential());

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddDbContext<MessangerDataContext>(options =>
    options.UseSqlServer(builder.Configuration["DefaultConnection"]),
    ServiceLifetime.Singleton);

builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<MessangerDataContext>()
                .AddDefaultTokenProviders();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE"),
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!))
        };
    });

_ = TokenCredentials.GetInstance(key: Environment.GetEnvironmentVariable("JWT_SECRET")!,
            issuer: Environment.GetEnvironmentVariable("JWT_VALID_ISSUER")!,
            audience: Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE")!);

_ = ConnectionStrings.GetInstance(Environment.GetEnvironmentVariable("DEFAULT_CONNECTION")!);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();
app.Use(async (context, next) =>
{
    string? token = context.Session.GetString("Token");
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + token);
    }
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
