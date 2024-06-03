using DataAccessPackage;
using DataAcessRepository;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using TestSimetricaConsulting.Filter;
using TestSimetricaConsulting.Service;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Agrega el servicio de versionado de la API
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// Agrega el servicio de proveedor de descripción de versiones de la API
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestSimetricaConsulting API", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Configuración del esquema de seguridad
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        In = ParameterLocation.Header
    };

    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.OperationFilter<RemoveAuthorizationFilter>();
    c.OperationFilter<SecurityRequirementsOperationFilter>(); 

});

// Configuración del DbContext
builder.Services.AddDbContext<OracleDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDbContext")));

// Repositorios y servicios
builder.Services.AddScoped<IRepository<Assignment>, Repository<Assignment>>();
builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddSingleton<IAssignmentService>(_ => new AssignmentService(builder.Configuration.GetConnectionString("OracleDbContext")));

// Configuración de autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
    var signingKey = new SymmetricSecurityKey(key);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = signingKey
    };
});

// Registro del servicio de autenticación
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();


app.UseSwagger();
app.UseSwaggerUI(c => 
{
    foreach (var description in provider.ApiVersionDescriptions) 
    {
        c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        //c.RoutePrefix = "api/documentation"; // Establece el prefijo de ruta para Swagger
        c.DefaultModelsExpandDepth(2);
        c.DocExpansion(DocExpansion.None);
        c.DisplayRequestDuration();
    }
});

// Habilitar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
