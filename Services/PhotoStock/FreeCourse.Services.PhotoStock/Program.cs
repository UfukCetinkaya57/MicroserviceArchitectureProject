using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Servisleri konteynerde ekleyin.
builder.Services.AddControllers();

// Swagger/OpenAPI yapýlandýrmasý hakkýnda bilgi edinmek için https://aka.ms/aspnetcore/swashbuckle adresini ziyaret edin.
builder.Services.AddEndpointsApiExplorer();

// Tüm denetleyici yöntemleri için yetkilendirme filtresi ekleyin.
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter());
});

// Swagger'ý ekleyin.
builder.Services.AddSwaggerGen();

// JWT Bearer kimlik doðrulamasýný yapýlandýrýn.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // Kimlik sunucusunun URL'sini yapýlandýrýn.
    options.Authority = builder.Configuration.GetSection("IdentityServerURL").Value;

    // API'ye eriþim için beklenen hedef kitleyi belirtin.
    options.Audience = "resource_photo_stock";

    // HTTPS gerekliliðini devre dýþý býrakýn (geliþtirme aþamasýnda yararlý olabilir, ancak üretim için önerilmez).
    options.RequireHttpsMetadata = false;
});

var app = builder.Build();

// Statik dosyalarý sunmak için middleware ekleyin.
app.UseStaticFiles();

// HTTP istek boru hattýný yapýlandýrýn.
if (app.Environment.IsDevelopment())
{
    // Swagger'ý yalnýzca geliþtirme ortamýnda etkinleþtirin.
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Kimlik doðrulama ve yetkilendirme middleware'lerini ekleyin.
app.UseAuthentication();
app.UseAuthorization();

// Denetleyicileri haritalayýn.
app.MapControllers();

// Uygulamayý çalýþtýrýn.
app.Run();
