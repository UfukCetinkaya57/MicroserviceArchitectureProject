using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Servisleri konteynerde ekleyin.
builder.Services.AddControllers();

// Swagger/OpenAPI yap�land�rmas� hakk�nda bilgi edinmek i�in https://aka.ms/aspnetcore/swashbuckle adresini ziyaret edin.
builder.Services.AddEndpointsApiExplorer();

// T�m denetleyici y�ntemleri i�in yetkilendirme filtresi ekleyin.
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter());
});

// Swagger'� ekleyin.
builder.Services.AddSwaggerGen();

// JWT Bearer kimlik do�rulamas�n� yap�land�r�n.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // Kimlik sunucusunun URL'sini yap�land�r�n.
    options.Authority = builder.Configuration.GetSection("IdentityServerURL").Value;

    // API'ye eri�im i�in beklenen hedef kitleyi belirtin.
    options.Audience = "resource_photo_stock";

    // HTTPS gereklili�ini devre d��� b�rak�n (geli�tirme a�amas�nda yararl� olabilir, ancak �retim i�in �nerilmez).
    options.RequireHttpsMetadata = false;
});

var app = builder.Build();

// Statik dosyalar� sunmak i�in middleware ekleyin.
app.UseStaticFiles();

// HTTP istek boru hatt�n� yap�land�r�n.
if (app.Environment.IsDevelopment())
{
    // Swagger'� yaln�zca geli�tirme ortam�nda etkinle�tirin.
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Kimlik do�rulama ve yetkilendirme middleware'lerini ekleyin.
app.UseAuthentication();
app.UseAuthorization();

// Denetleyicileri haritalay�n.
app.MapControllers();

// Uygulamay� �al��t�r�n.
app.Run();
