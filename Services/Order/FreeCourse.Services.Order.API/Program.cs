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
    // Mesaj tüketicilerini ekle
    x.AddConsumer<CreateOrderMessageCommandConsumer>();
    x.AddConsumer<CourseNameChangedEventConsumer>();

    // RabbitMQ yapýlandýrmasý
    x.UsingRabbitMq((context, cfg) =>
    {
        // RabbitMQ sunucusuna baðlan
        cfg.Host(builder.Configuration["RabbitMQUrl"], "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        // Mesaj alma noktalarý
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

// MassTransit'i arka planda çalýþtýrmak için gerekli hizmeti ekle
builder.Services.AddMassTransitHostedService();

// Veritabaný baðlamý için SQL Server yapýlandýrmasý
builder.Services.AddDbContext<OrderDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), configure =>
    {
        configure.EnableRetryOnFailure(); // Hata durumunda tekrar deneme
        configure.MigrationsAssembly("FreeCourse.Services.Order.Infrastructure");
    });
});

// Yetkilendirme politikasý oluþtur
var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

// JWT güvenlik ayarlarýný yapýlandýr
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub"); // Varsayýlan "sub" iddia tipini kaldýr
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"]; // Kimlik sunucusu URL'si
    options.Audience = "resource_order"; // Beklenen izleyici
    options.RequireHttpsMetadata = false; // HTTPS zorunlu deðil
});

// HttpContextAccessor ekle
builder.Services.AddHttpContextAccessor();

// Kullanýcý kimliði servisini ekle
builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();

// MediatR'ý ekle
builder.Services.AddMediatR(typeof(CreateOrderCommandHandler).Assembly);

// Denetleyicileri ekle ve yetkilendirme filtresini ekle
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy)); // Tüm denetleyiciler için yetkilendirme
});

// Swagger/OpenAPI yapýlandýrmasý
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// HTTP istek boru hattýný yapýlandýr
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Swagger'ý geliþtirme modunda etkinleþtir
    app.UseSwaggerUI(); // Swagger kullanýcý arayüzünü etkinleþtir
}

// Veritabaný migrasyonunu uygula
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var orderDbContext = serviceProvider.GetRequiredService<OrderDbContext>();
    orderDbContext.Database.Migrate(); // Migrasyonlarý uygula
}

app.UseAuthentication(); // Kimlik doðrulamasýný etkinleþtir
app.UseAuthorization(); // Yetkilendirmeyi etkinleþtir

app.MapControllers(); // Denetleyicileri haritalandýr

app.Run(); // Uygulamayý çalýþtýr
