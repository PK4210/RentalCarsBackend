using Microsoft.EntityFrameworkCore;
using RentalCars.Data;

var builder = WebApplication.CreateBuilder(args);

// Registrar AppDbContext con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar controladores
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Información del documento de Swagger
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "RentalCars API",
        Version = "v1",
        Description = "API para la gestión de alquiler de vehículos"
    });

    // Ejemplo personalizado para el modelo User (opcional)
    c.MapType<RentalCars.Models.User>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Example = new Microsoft.OpenApi.Any.OpenApiObject
        {
            ["userId"] = new Microsoft.OpenApi.Any.OpenApiInteger(1),
            ["username"] = new Microsoft.OpenApi.Any.OpenApiString("adminuser"),
            ["password"] = new Microsoft.OpenApi.Any.OpenApiString("securepassword"),
            ["email"] = new Microsoft.OpenApi.Any.OpenApiString("admin@example.com"),
            ["userType"] = new Microsoft.OpenApi.Any.OpenApiBoolean(true),
            ["isDeleted"] = new Microsoft.OpenApi.Any.OpenApiBoolean(false),
            ["createdAt"] = new Microsoft.OpenApi.Any.OpenApiDateTime(DateTime.Now)
        }
    });
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:3000") // Dominio del frontend
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials(); // Permite cookies y credenciales
    });
});

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aplicar la política de CORS antes de MapControllers
app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();
app.MapControllers(); // Mapea los controladores
app.Run();
