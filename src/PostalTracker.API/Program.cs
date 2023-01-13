using MassTransit;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PostalTracker.API.Services;
using PostalTracker.Contracts.Events;
using PostalTracker.System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<PostalService>();

builder.Services.AddControllers(config => config.AllowEmptyInputInBodyModelBinding = true)
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
    });

builder.Services.AddMassTransit(mt =>
{
    mt.SetKebabCaseEndpointNameFormatter();
    mt.UsingRabbitMq((context, cfg) =>
    {
        cfg.AutoStart = true;

        var redditMqSection = builder.Configuration.GetSection("RabbitMq");
        cfg.Host(redditMqSection.GetValue<string>("HostAddress"),
            redditMqSection.GetValue<ushort>("Port"),
            redditMqSection.GetValue<string>("VirtualHost"), h =>
            {
                h.Username(redditMqSection.GetValue<string>("Username"));
                h.Password(redditMqSection.GetValue<string>("Password"));
            });
        cfg.ConfigureEndpoints(context);
    });

    mt.AddRequestClient<PostalCreate>();
    mt.AddRequestClient<PostalStatusRequest>();
});
builder.Services.AddMassTransitHostedService();

var commentsFileName = typeof(Program).Assembly.GetName().Name + ".xml";
var commentFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, commentsFileName);

builder.Services.AddSwaggerGen(setup =>
{
    setup.IncludeXmlComments(commentFile);
    setup.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "Postal tracker API" });
});
builder.Services.AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.MapControllers();
app.Run();