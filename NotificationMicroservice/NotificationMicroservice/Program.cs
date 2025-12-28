using Core.RedisDestributesSemaphore;
using MassTransit;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NotificationListener.Consumers;
using NotificationMicroservice.API.Consumers;
using NotificationMicroservice.Application.Interfaces;
using NotificationMicroservice.Application.Services;
using NotificationMicroservice.Domain.Interface;
using NotificationMicroservice.Infrastructure;
using NotificationMicroservice.Infrastructure.Repositories;
using StackExchange.Redis;

namespace NotificationMicroservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddOpenApi(); 
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

            // Register application services (scoped for per-request lifecycle)
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();

            // Register infrastructure services
            builder.Services.AddTransient<IEmailSender, MailKitEmailService>();

            builder.Services.AddTransient<IEmailNotificationRepository, EmailNotificationRepository>();
            builder.Services.AddDbContext<NotificationDbContext>((serviceProvider, options) =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddRedisDistributedSemaphore();

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<RegisterUserRequestedConsumer>();
                x.AddConsumer<LoginRequestedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq://localhost", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // Option A: explicit queues
                    cfg.ReceiveEndpoint("auth.register.requested", e =>
                    {
                        e.ConfigureConsumer<RegisterUserRequestedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("auth.login.requested", e =>
                    {
                        e.ConfigureConsumer<LoginRequestedConsumer>(context);
                    });
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification Service API V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.MapControllers();

            app.Run();
        }
    }
}
