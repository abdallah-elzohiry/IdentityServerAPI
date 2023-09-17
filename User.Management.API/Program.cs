using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using User.Management.API.Models;
using User.Management.Service.Models;
using User.Management.Service.Services;

namespace User.Management.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configration = builder.Configuration;
            // Add services to the container.
            
            // For Entity Framework
            builder.Services.AddDbContext<ApplicatinDbContext>( options => options.UseSqlServer(configration.GetConnectionString("ConnStr")));

            // Foe Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicatinDbContext>()
            .AddDefaultTokenProviders();

            // Add Config for Required Email
            builder.Services.Configure<IdentityOptions>(opt => opt.SignIn.RequireConfirmedEmail = true);

            // Add Authentication 
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            //Add Email Configration
            var emailConfig = configration.GetSection("EmailConfigration").Get<EmailConfigration>();
            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}