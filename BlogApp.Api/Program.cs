using BlogApp.API.Helpers;
using BlogApp.API.Hubs;
using BlogApp.BL;
using BlogApp.BL.Profiles;
using BlogApp.BL.Services.Interfaces;
using BlogApp.Core.Entities;
using BlogApp.DAL;
using BlogApp.DAL.Contexts;
using BlogApp.DAL.Repositories.Implements;
using BlogApp.DAL.Repositories.Interfaces;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace BlogApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
); ;
<<<<<<< HEAD:BlogApp.Api/Program.cs

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",  
                    policy =>
                    {
                        policy.WithOrigins(
                            "http://127.0.0.1:5500",    // Live Server VS Code
                            "null",                      // file:/// protocol
                            "http://localhost:5173",     // Vite default port
                            "https://localhost:5173"     // Vite HTTPS
                        )
                              .AllowAnyHeader()          // Allow any headers (including Authorization)
                              .AllowAnyMethod()           // Allow any methods (GET, POST, PUT, DELETE)
                              .AllowCredentials();        // Allow credentials for authenticated requests
                    });
            });
=======
>>>>>>> parent of 77b7f0a (update):BlogApp/Program.cs
            builder.Services.AddFluentValidation(opt=>
                {
                    opt.RegisterValidatorsFromAssemblyContaining<CategoryService>();
                });
                
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
               opt.AddSecurityRequirement(new OpenApiSecurityRequirement
               {
                   {
                       new OpenApiSecurityScheme
                       {
                           Reference = new OpenApiReference
                           {
                               Type=ReferenceType.SecurityScheme,
                               Id="Bearer"
                           }
                       },
                       new string[]{}
                   }
               });
               
               // Include XML comments from controller methods
               var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
               var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
               if (File.Exists(xmlPath))
               {
                   opt.IncludeXmlComments(xmlPath);
               }
               });
            builder.Services.AddDbContext<AppDbContext>(opt=> {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
                });
            builder.Services.AddIdentity<AppUser,IdentityRole>(opt=>
            {
                opt.Password.RequireNonAlphanumeric=false;

            }).AddDefaultTokenProviders().AddEntityFrameworkStores <AppDbContext>();

            builder.Services.AddRepositories();
            builder.Services.AddServices();
            builder.Services.AddSignalR();

           builder.Services.AddAuthentication(
                opt =>
                {       
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
                ).AddJwtBearer(
                opt => 
                {
                    opt.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        LifetimeValidator =(_,expires,token,_) => token!=null? DateTime.UtcNow < expires : false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecurityKey"]))

                    };
                }
                );
            builder.Services.AddAuthorization();
            builder.Services.AddAutoMapper(typeof(CategoryMappingProfile).Assembly);
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5000); // HTTP
                options.ListenAnyIP(5001, listenOptions =>
                {
                    listenOptions.UseHttps(); // HTTPS
                });
            });

            var app = builder.Build();
            // Configure the HTTP request pipeline.
      
                app.UseSwagger();
                app.UseSwaggerUI();
            
            app.UseSwaggerUI(c =>
            {
                c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCustomExceptionHandler();

            app.MapControllers();
            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}
