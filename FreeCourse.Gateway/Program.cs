using FreeCourse.Gateway.DelegateHandlers; // Token exchange handler sýnýfý
using Ocelot.DependencyInjection; // Ocelot baðýmlýlýk enjeksiyonu
using Ocelot.Middleware; // Ocelot ara katman
using Ocelot.Values; // Ocelot deðerleri

var builder = WebApplication.CreateBuilder(args);

// Uygulama yapýlandýrmasýný JSON dosyasýndan ve ortam deðiþkenlerinden yükleme
builder.Configuration
    .AddJsonFile($"configuration.{builder.Environment.EnvironmentName}.json", optional: true) // Ortam bazlý yapýlandýrma dosyasý
    .AddEnvironmentVariables(); // Ortam deðiþkenlerini ekleme

// JWT Bearer kimlik doðrulama yapýlandýrmasý
builder.Services.AddAuthentication().AddJwtBearer("GatewayAuthenticationScheme", options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"]; // Identity Server URL'si
    options.Audience = "resource_gateway"; // Beklenen kaynak
    options.RequireHttpsMetadata = false; // HTTPS zorunluluðu yok
});

// Ocelot yapýlandýrmasýný ekleme ve TokenExhangeDelegateHandler'ý delegating handler olarak ekleme
builder.Services.AddOcelot(builder.Configuration).AddDelegatingHandler<TokenExhangeDelegateHandler>();

// TokenExhangeDelegateHandler için HttpClient ekleme
builder.Services.AddHttpClient<TokenExhangeDelegateHandler>();

var app = builder.Build(); // Uygulama inþasý

app.UseAuthorization(); // Yetkilendirmeyi uygulama akýþýna ekleme
await app.UseOcelot(); // Ocelot middleware'ini kullanma
app.Run(); // Uygulamayý baþlatma
