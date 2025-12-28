using AuthMicroservice.API.Messaging.Sagas;
using AuthMicroservice.DAL;
using AuthMicroservice.DAL.Models;
using AuthMicroservice.Logic.Interfaces;
using AuthMicroservice.Logic.Services;
using ConnectionLib.ConnectionServices;
using ConnectionLib.ConnectionServices.Interfaces;
using Core.HttpLogic;
using Core.RedisDestributesSemaphore;
using Core.TraceIdLogic;
using Core.TraceLogic;
using MassTransit;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;

namespace AuthMicroservice.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Host.UseSerilog();

            builder.Services.TryAddTraceId();
            builder.Services.AddHttpRequestService();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
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

            builder.Services.AddIdentity<UserDal, RoleDal>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;

                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = JwtService.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = JwtService.AUDIENCE,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = JwtService.GetSymmetricSecurityKey()
                    };
                });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddTransient<IJwtService, JwtService>();
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddTransient<IProfileConnectionService, ProfileConnectionService>();

            builder.Services.AddRedisDistributedSemaphore();

            builder.Services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<CoordinatorStateMachine, CoordinatorState>()
                    .InMemoryRepository();
                x.AddSagaStateMachine<OrchestratorStateMachine, OrchestratorState>()
                    .InMemoryRepository();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", x =>
                    {
                        x.Username("guest");
                        x.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            var app = builder.Build();

            app.UseMiddleware<TraceMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
