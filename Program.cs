using rentalcar_backend.Method;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// CONFIGURATION FROM appsettings.json
// SOURCE: https://stackoverflow.com/a/69722959
ConfigurationManager configuration = builder.Configuration;

// fill our method with configuration
Token.Init(configuration);
//CRUD.Init(configuration);
Email.Init(configuration);

// CORS
// SOURCE:https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-6.0
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
            .AllowAnyOrigin()
            //.SetIsOriginAllowedToAllowWildcardSubdomains()
            //.WithOrigins("https://localhost:3000/", "http://localhost:3000/")
            .AllowAnyMethod()
            .AllowAnyHeader();
            //.AllowCredentials();
        });
});


string jwtKey = configuration["Jwt:Key"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer((options) =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // validate signature/credential
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            // validate issuer
            ValidateIssuer = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            // validate audience
            ValidateAudience = true,
            ValidAudience = configuration["Jwt:Audience"],
            // validate time
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    });

var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
