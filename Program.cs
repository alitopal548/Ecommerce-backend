using Microsoft.AspNetCore.Cors;
using ECommerce.Helpers;
using ECommerce.Services;
using ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ECommerce.Business;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEmailService, SmtpEmailService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()   // Her yerden gelen istekleri kabul et
            .AllowAnyMethod()   // GET, POST, PUT, DELETE vb. her metodu kabul et
            .AllowAnyHeader();  // Her header’a izin ver
    });
});

builder.Services.AddHttpContextAccessor();


var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Token'ın belirli bir "issuer"dan gelip gelmediğini kontrol et
            ValidateAudience = true, // Token'ın doğru "audience"a ait olup olmadığını kontrol et
            ValidateLifetime = true, // Token'ın süresi dolmuş mu kontrol et
            ValidateIssuerSigningKey = true, // Token'ın imzası doğru mu kontrol et

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["Key"]!))

        };
    });


builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // JWT Bearer token'ı Swagger'a tanıtıyoruz
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization", // Header'da "Authorization" alanını kullan
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Lütfen 'Bearer' ile başlayan JWT token'ınızı girin. Örn: Bearer eyJhbGciOiJIUzI1NiIs..."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<OrderServices>();
builder.Services.AddScoped<AuthBusiness>();
builder.Services.AddScoped<CategoryBusiness>();
builder.Services.AddScoped<RegistrationBusiness>();
builder.Services.AddScoped<UsersBusiness>();
builder.Services.AddScoped<ProductBusiness>();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
