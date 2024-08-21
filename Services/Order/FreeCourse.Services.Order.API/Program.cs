using FreeCourse.Services.Order.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MediatR;
using FreeCourse.Services.Order.Application.Handlers;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using MassTransit;
using FreeCourse.Services.Order.Application.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Servisleri konteyner'e ekleme
builder.Services.AddMassTransit(x =>
{
    // Mesaj t�keticilerini ekle
    x.AddConsumer<CreateOrderMessageCommandConsumer>();
    x.AddConsumer<CourseNameChangedEventConsumer>();

    // RabbitMQ yap�land�rmas�
    x.UsingRabbitMq((context, cfg) =>
    {
        // RabbitMQ sunucusuna ba�lan
        cfg.Host(builder.Configuration["RabbitMQUrl"], "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        // Mesaj alma noktalar�
        cfg.ReceiveEndpoint("create-order-service", e =>
        {
            e.ConfigureConsumer<CreateOrderMessageCommandConsumer>(context);
        });
        cfg.ReceiveEndpoint("course-name-changed-event-order-service", e =>
        {
            e.ConfigureConsumer<CourseNameChangedEventConsumer>(context);
        });
    });
});

// MassTransit'i arka planda �al��t�rmak i�in gerekli hizmeti ekle
builder.Services.AddMassTransitHostedService();

// Veritaban� ba�lam� i�in SQL Server yap�land�rmas�
builder.Services.AddDbContext<OrderDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), configure =>
    {
        configure.EnableRetryOnFailure(); // Hata durumunda tekrar deneme
        configure.MigrationsAssembly("FreeCourse.Services.Order.Infrastructure");
    });
});

// Yetkilendirme politikas� olu�tur
var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

// JWT g�venlik ayarlar�n� yap�land�r
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub"); // Varsay�lan "sub" iddia tipini kald�r
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"]; // Kimlik sunucusu URL'si
    options.Audience = "resource_order"; // Beklenen izleyici
    options.RequireHttpsMetadata = false; // HTTPS zorunlu de�il
});

// HttpContextAccessor ekle
builder.Services.AddHttpContextAccessor();

// Kullan�c� kimli�i servisini ekle
builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();

// MediatR'� ekle
builder.Services.AddMediatR(typeof(CreateOrderCommandHandler).Assembly);

// Denetleyicileri ekle ve yetkilendirme filtresini ekle
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy)); // T�m denetleyiciler i�in yetkilendirme
});

// Swagger/OpenAPI yap�land�rmas�
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// HTTP istek boru hatt�n� yap�land�r
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Swagger'� geli�tirme modunda etkinle�tir
    app.UseSwaggerUI(); // Swagger kullan�c� aray�z�n� etkinle�tir
}

// Veritaban� migrasyonunu uygula
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var orderDbContext = serviceProvider.GetRequiredService<OrderDbContext>();
    orderDbContext.Database.Migrate(); // Migrasyonlar� uygula
}

app.UseAuthentication(); // Kimlik do�rulamas�n� etkinle�tir
app.UseAuthorization(); // Yetkilendirmeyi etkinle�tir

app.MapControllers(); // Denetleyicileri haritaland�r

app.Run(); // Uygulamay� �al��t�r
