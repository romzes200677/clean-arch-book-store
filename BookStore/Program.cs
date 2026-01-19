using System.Text;
using BookStore.Middleware;
using BookStore.User.Api;
using BookStore.User.Infrastructure;
using BookStore.User.Infrastructure.data;
using BookStore.User.Infrastructure.seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharedKernel;
using SharedKernel.Architecture;

var builder = WebApplication.CreateBuilder(args);
// Гарантированно загружаем сборку модуля User
// 1. Загружаем сборки (используем ваш список assemblies из прошлого шага)
var assemblies = ModuleLoader.LoadModuleAssemblies(AppContext.BaseDirectory);

// Явное добавление сборки с контроллерами (User.Api)
var userApiAssembly = typeof(AuthController).Assembly;
if (!assemblies.Contains(userApiAssembly))
{
    assemblies.Add(userApiAssembly);
}

// Ищем все типы, реализующие IModule
var moduleTypes = assemblies
    .SelectMany(a => a.GetTypes())
    .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

var modules = new List<IModule>();

foreach (var type in moduleTypes)
{
    // Создаем экземпляр модуля через активатор
    var module = (IModule)Activator.CreateInstance(type)!;
    module.ConfigureServices(builder.Services, builder.Configuration);
    modules.Add(module);
}


// 2. Регистрируем MVC и явно указываем части приложения
builder.Services.AddControllers()
    .ConfigureApplicationPartManager(apm =>
    {
        foreach (var assembly in assemblies)
        {
            // Это принудительная активация контроллеров в сборке
            if (!apm.ApplicationParts.Any(p => p.Name == assembly.GetName().Name))
            {
                apm.ApplicationParts.Add(new Microsoft.AspNetCore.Mvc.ApplicationParts.AssemblyPart(assembly));
            }
        }
    });

// 1. Контроллеры и Swagger
builder.Services.AddEndpointsApiExplorer(); // Нужно для корректной работы Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "User API", Version = "v1" });

    // 1. Определяем схему безопасности (Bearer)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите только ваш JWT токен. Пример: 12345abcdef"
    });

    // 2. Делаем авторизацию глобальной для всех эндпоинтов
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// dotnet add package MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies.ToArray()));



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://myfrontend.com") // Список разрешенных доменов
            .AllowAnyMethod()  // Разрешить GET, POST, PUT, DELETE и т.д.
            .AllowAnyHeader()  // Разрешить любые заголовки (Content-Type, Authorization)
            .AllowCredentials(); // Разрешить передачу Cookies или заголовков авторизации
    });
});
// 3. Identity Core
builder.Services.AddIdentityCore<AppUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

// Чтение через индексатор (строка)
var secretKey = builder.Configuration["JwtSettings:SecretKey"]; //читаем из secrets
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];


// Проверка на null
if (string.IsNullOrEmpty(secretKey)) 
    throw new Exception("JWT Secret Key is missing in configuration!");

var key = Encoding.UTF8.GetBytes(secretKey);
// 4. Аутентификация
builder.Services.AddAuthentication(options =>
    {
        // Указываем, что по умолчанию используем JWT Bearer
        options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
        options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
        options.DefaultScheme = IdentityConstants.BearerScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        
            // Должно СОВПАДАТЬ с тем, что вы пишете в методе GenerateJwtToken
            ValidIssuer = issuer, 
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    })
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();

// --- CONFIGURE ENDPOINTS ---
foreach (var module in modules)
{
    module.ConfigureEndpoints(app);
}

// Инициализация ролей
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await initializer.InitializeAsync();
}

// --- Настройка Middleware (Порядок важен!) ---

if (app.Environment.IsDevelopment())
{
    // Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Порядок этих двух методов критичен:
app.UseAuthentication(); // КТО пользователь?
app.UseAuthorization();  // ЧТО ему можно делать?

// 5. Маппинг маршрутов
app.MapControllers(); // Добавлено для работы ваших [ApiController]
app.MapGroup("/identity").MapIdentityApi<AppUser>(); // Стандартные эндпоинты Identity

app.Run();