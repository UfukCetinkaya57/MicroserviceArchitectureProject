using FreeCourse.Services.Discount.Services;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();

var requrieAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
//Claimle ilgili bir kod  new AuthorizationPolicyBuilder().RequireClaim("scope", "discount_read");
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration.GetSection("IdentityServerURL").Value;
    options.Audience = "resource_discount";
    options.RequireHttpsMetadata = false;
});
builder.Services.AddControllers(
    opt =>
    {
        opt.Filters.Add(new AuthorizeFilter(requrieAuthorizePolicy));
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
