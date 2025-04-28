using Transport_Management;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Transport_Management.Models;
using Transport_Management.Helpers.DbContexts;
using Transport_Management.Helpers.Middlewares;
using Transport_Management.Interface;
using Transport_Management.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Transport_Management.Helpers.Filters;
using Microsoft.Extensions.Configuration;

internal class Program
{
    private static void Main(string[] args)
    {


        var builder = WebApplication.CreateBuilder(args);

        // Connection string for your database
         string connectionString = @"Server=172.16.15.17;Database=testDB;user=sa;password=Rndsoft@123;TrustServerCertificate=True;MultipleActiveResultSets=true;";

        // Register your dbContext with the dependency injection container
       // builder.Services.AddDbContext<dbContext>(options => options.UseSqlServer(connectionString));
        builder.Services.AddDbContext<dbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddSingleton<ApiResponseRepository>();
        builder.Services.AddScoped<authRepository>();
        builder.Services.AddScoped<vehicleRepository>();
        builder.Services.AddScoped<companyRepository>();
        builder.Services.AddScoped<SessionAuthorizeAttribute>();


        // Register Repositories and its implementation
        builder.Services.AddScoped<IApiResponseRepository, ApiResponseRepository>();
        builder.Services.AddScoped<IauthRepository, authRepository>();
        builder.Services.AddScoped<IvehicleRepository, vehicleRepository>();
        builder.Services.AddScoped<IcompanyRepository, companyRepository>();
        builder.Services.AddScoped<IrentalRepository, rentalRepository>();
        builder.Services.AddScoped<IinvoiceRepository, invoiceRepository>();

        //  Middleware Implementation
        builder.Services.AddScoped<globalExceptionHandlingMiddleware>();

        // Add services to the container
        builder.Services.AddControllersWithViews();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        });

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(240        ); // Set session timeout
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();

        app.UseAuthorization();

        // Use the middleware
        app.UseMiddleware<globalExceptionHandlingMiddleware>();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Auth}/{action=Login}/{id?}");

        app.Run();
    }
}

