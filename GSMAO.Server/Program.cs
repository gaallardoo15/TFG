using AutoMapper;
using GSMAO.Server.Controllers;
using GSMAO.Server.Database;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar las variables de entorno
// 1. Leer .env
DotNetEnv.Env.Load(".env");
// 2. Según el CLIENT, cargar un appsettings especial
var clientValue = Environment.GetEnvironmentVariable("CLIENT")
                  ?? throw new InvalidOperationException("No se ha definido la variable de entorno CLIENT");
builder.Configuration.AddJsonFile($"appsettings.{clientValue}.json", optional: false);


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("LogsGSMAOHitachi/log-.txt", rollingInterval: RollingInterval.Day)
    .Filter.ByExcluding(logEvent =>
        logEvent.Properties.ContainsKey("ResponseBody") // Filtra logs que contengan ResponseBody
    )
    .CreateLogger();
Log.Information("Current working directory: {WorkingDirectory}", Environment.CurrentDirectory);
builder.Host.UseSerilog();


// Add DB connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mysqlOptions =>
        {
            mysqlOptions.DisableBackslashEscaping();
            mysqlOptions.CommandTimeout(8); // Tiempo de espera en segundos
        }
    );
});

// Configure AutoMapper for map Tables to DTOs or DTOs to Tables.
builder.Services.AddAutoMapper(typeof(Program).Assembly);
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<AutoMapperProfile>();
});

// Configure Entity Framework Initializar for seeding
builder.Services.AddScoped<IDefaultDbContextInitializer, ApplicationDbContextInitializer>();

// Add Identity
builder.Services.AddIdentity<Usuario, Rol>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<UsersDAO, UsersDAO>();
builder.Services.AddScoped<RolesDAO, RolesDAO>();
builder.Services.AddScoped<EstadosUsuariosDAO, EstadosUsuariosDAO>();
builder.Services.AddScoped<EstadosActivosDAO, EstadosActivosDAO>();
builder.Services.AddScoped<EmpresaDAO, EmpresaDAO>();
builder.Services.AddScoped<PlantaDAO, PlantaDAO>();
builder.Services.AddScoped<LocalizacionDAO, LocalizacionDAO>();
builder.Services.AddScoped<ActivoDAO, ActivoDAO>();
builder.Services.AddScoped<ComponenteDAO, ComponenteDAO>();
builder.Services.AddScoped<ActivoComponenteDAO, ActivoComponenteDAO>();
builder.Services.AddScoped<CriticidadDAO, CriticidadDAO>();
builder.Services.AddScoped<CentroCosteDAO, CentroCosteDAO>();
builder.Services.AddScoped<MecanismoDeFalloDAO, MecanismoDeFalloDAO>();
builder.Services.AddScoped<IncidenciaDAO, IncidenciaDAO>();
builder.Services.AddScoped<IncidenciasOrdenesDAO, IncidenciasOrdenesDAO>();
builder.Services.AddScoped<ResolucionDAO, ResolucionDAO>();
builder.Services.AddScoped<OrdenesDAO, OrdenesDAO>();
builder.Services.AddScoped<EstadosOrdenesDAO, EstadosOrdenesDAO>();
builder.Services.AddScoped<TiposOrdenesDAO, TiposOrdenesDAO>();
builder.Services.AddScoped<HistorialCambiosOrdenDAO, HistorialCambiosOrdenDAO>();
builder.Services.AddSingleton<JwtService, JwtService>();
builder.Services.AddScoped<ValidateData, ValidateData>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddTransient<GeneradorExcel>();
builder.Services.AddTransient<IMyEmailSender, EmailSender>();
builder.Services.Configure<EmailSenderOptions>(builder.Configuration.GetSection("email"));
//builder.Services.AddScoped<ImportService>();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All
                                        | HttpLoggingFields.RequestMethod
                                        | HttpLoggingFields.RequestBody
                                        | HttpLoggingFields.RequestPath
                                        | HttpLoggingFields.ResponseStatusCode
                                        | HttpLoggingFields.ResponseBody;
    logging.RequestHeaders.Add("Authorization");
    logging.CombineLogs = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .SetIsOriginAllowed(origin =>
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                if (Uri.TryCreate(origin, UriKind.Absolute, out Uri uri))
                {
                    return uri.Host == "localhost" ||
                           uri.Host == "hitachimigracion.gsmao.es" ||
                           uri.Host == "migracionprueba.gsmao.es";
                }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                return false;
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "GSMAO", Version = "V1" });

    // Filtrar controladores para mostrar en Swagger
    //c.DocInclusionPredicate((docName, apiDesc) =>
    //{
    //    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

    //    var controller = methodInfo.DeclaringType;
    //    return controller != null && controller == typeof(SoporteController);
    //});

    // Define el esquema de seguridad para usar JWT.
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                      <br/> Enter 'Bearer' [space] and then your token in the text input below.
                      <br/>nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Use CORS policy
app.UseCors("AllowSpecificOrigin");

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseHttpLogging();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpLogging();

// Configuracion de las rutas de nivel superior
app.MapControllers();

// Si tienes rutas adicionales, las puedes configurar aqui
// Ejemplo: app.MapGet("/", () => "Hello World!");

app.MapFallbackToFile("/index.html");

using (var scope = app.Services.CreateScope())
{
    // Create a scope to obtain services via dependency injection
    var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<UsersDAO>>();
    var stateDAO = scope.ServiceProvider.GetRequiredService<EstadosUsuariosDAO>();

    if (dbContext != null)
    {
        // Asegurate de que Login se inicialice correctamente con ApplicationDbContext
        var userDAO = new UsersDAO(dbContext, logger, userManager, stateDAO); // Crear una instancia de Login

        // Apply any pending migrations to the database
        await dbContext.Database.MigrateAsync();
    }

    // Get the application's environment
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

    if (env.IsDevelopment())
    {
        // If we are in the development environment, seed the database with initial data
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDefaultDbContextInitializer>();
        await dbInitializer.SeedAsync();
    }
}

app.Run();
