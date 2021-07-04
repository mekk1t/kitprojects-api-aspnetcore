using KitProjects.MasterChef.WebApplication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;

namespace KitProjects.Api.AspNetCore.Extensions
{
    /// <summary>
    /// Класс расширений конфигурации API.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Добавляет Swagger-документацию для API версии 1. Включает XML-документацию для сборки веб-приложения.
        /// </summary>
        /// <param name="services">Коллекция сервисов DI ASP.NET Core.</param>
        /// <param name="title">Название API.</param>
        /// <param name="setupAction">Настройка генерации Swagger.</param>
        public static void AddSwaggerV1(this IServiceCollection services, string title, Action<SwaggerGenOptions> setupAction = null)
        {
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = "v1" });
                var xmlDocPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                config.IncludeXmlComments(xmlDocPath);
                setupAction?.Invoke(config);
            });
        }

        /// <summary>
        /// Добавляет основные сервисы для API.
        /// </summary>
        /// <param name="services">Коллекция сервисов DI ASP.NET Core.</param>
        /// <param name="controllersSetup">Настройка контроллеров.</param>
        /// <param name="jsonSetup">Настройка сериализации JSON.</param>
        /// <param name="serializeEnumsAsStrings">Сериализовать <see cref="Enum"/> как <see cref="string"/>?</param>
        public static void AddApiCore(
            this IServiceCollection services,
            bool serializeEnumsAsStrings,
            Action<MvcOptions> controllersSetup = null,
            Action<JsonOptions> jsonSetup = null)
        {
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new LowercaseControllerTokenTransformer()));
                controllersSetup?.Invoke(options);
            }).AddJsonOptions(options =>
            {
                if (serializeEnumsAsStrings)
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                jsonSetup?.Invoke(options);
            });
        }

        /// <summary>
        /// Добавляет документацию Swagger, доступную по адресу API /swagger.
        /// </summary>
        /// <param name="app">Пайплайн ASP.NET Core.</param>
        /// <param name="apiName">Название API.</param>
        /// <param name="uiSetup">Настройки интерфейса Swagger.</param>
        public static void UseSwaggerDocumentation(this IApplicationBuilder app, string apiName, Action<SwaggerUIOptions> uiSetup = null)
        {
            app.UseSwagger().UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", $"{apiName} V1");
                config.RoutePrefix = "swagger";
                config.DocumentTitle = $"{apiName} API";
                uiSetup?.Invoke(config);
            });
        }
    }
}
