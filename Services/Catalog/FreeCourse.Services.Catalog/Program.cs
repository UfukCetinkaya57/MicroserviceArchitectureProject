using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Services.Catalog.Settings;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration.GetSection("IdentityServerURL").Value;
    options.Audience = "resource_catalog";
    options.RequireHttpsMetadata = false;
});

builder.Services.AddMassTransit(x =>
{
    //default port: 5672
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQUrl"], "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });
    });
});
builder.Services.AddMassTransitHostedService();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter());
});

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddSingleton<IDatabaseSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program)); 
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var categoryService = serviceProvider.GetRequiredService<ICategoryService>();
    if (!categoryService.GetAllAsync().Result.Data.Any())
    {
        categoryService.CreateAsync(new CategoryDto { Name = "Asp .Net Core Kursu" }).Wait();
        categoryService.CreateAsync(new CategoryDto { Name = "Asp .Net Core API Kursu" }).Wait();
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
