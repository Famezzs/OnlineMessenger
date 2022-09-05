using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineMessenger.Helpers.Constants;
using OnlineMessenger.Models;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureKeyVault(
    new Uri(Environment.GetEnvironmentVariable("VAULT_NAME")!),
    new DefaultAzureCredential()
    );

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddDbContext<MessengerDataContext>(options =>
    options.UseSqlServer(builder.Configuration["DefaultConnection"]),
    ServiceLifetime.Singleton);

builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<MessengerDataContext>()
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
            ValidAudience = builder.Configuration["ValidAudience"],
            ValidIssuer = builder.Configuration["ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Secret"]))
        };
    });

_ = TokenCredentials.GetInstance(
            key: builder.Configuration["Secret"],
            issuer: builder.Configuration["ValidIssuer"],
            audience: builder.Configuration["ValidAudience"]
            );

_ = ConnectionStrings.GetInstance(builder.Configuration["DefaultConnection"]);

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
