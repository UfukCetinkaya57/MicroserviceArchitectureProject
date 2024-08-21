using FreeCourse.Gateway.DelegateHandlers; // Token exchange handler s�n�f�
using Ocelot.DependencyInjection; // Ocelot ba��ml�l�k enjeksiyonu
using Ocelot.Middleware; // Ocelot ara katman
using Ocelot.Values; // Ocelot de�erleri

var builder = WebApplication.CreateBuilder(args);

// Uygulama yap�land�rmas�n� JSON dosyas�ndan ve ortam de�i�kenlerinden y�kleme
builder.Configuration
    .AddJsonFile($"configuration.{builder.Environment.EnvironmentName}.json", optional: true) // Ortam bazl� yap�land�rma dosyas�
    .AddEnvironmentVariables(); // Ortam de�i�kenlerini ekleme

// JWT Bearer kimlik do�rulama yap�land�rmas�
builder.Services.AddAuthentication().AddJwtBearer("GatewayAuthenticationScheme", options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"]; // Identity Server URL'si
    options.Audience = "resource_gateway"; // Beklenen kaynak
    options.RequireHttpsMetadata = false; // HTTPS zorunlulu�u yok
});

// Ocelot yap�land�rmas�n� ekleme ve TokenExhangeDelegateHandler'� delegating handler olarak ekleme
builder.Services.AddOcelot(builder.Configuration).AddDelegatingHandler<TokenExhangeDelegateHandler>();

// TokenExhangeDelegateHandler i�in HttpClient ekleme
builder.Services.AddHttpClient<TokenExhangeDelegateHandler>();

var app = builder.Build(); // Uygulama in�as�

app.UseAuthorization(); // Yetkilendirmeyi uygulama ak���na ekleme
await app.UseOcelot(); // Ocelot middleware'ini kullanma
app.Run(); // Uygulamay� ba�latma
