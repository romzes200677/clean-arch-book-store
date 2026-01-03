using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharedKernel.Architecture;
using User.Infrastructure;
using User.Infrastructure.data;


var builder = WebApplication.CreateBuilder(args);
var assemblies = AppDomain.CurrentDomain.GetAssemblies(); // Или загружаем из папки Plugins

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

// 1. Контроллеры и Swagger
builder.Services.AddControllers(); // Добавлено для поддержки [ApiController]
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

builder.Services.AddOpenApi();
// dotnet add package MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
// 2. База данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("IdentityInMemoryDb"));
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
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

// 4. Аутентификация
builder.Services.AddAuthentication(IdentityConstants.BearerScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorization();

var app = builder.Build();

// --- CONFIGURE ENDPOINTS ---
foreach (var module in modules)
{
    module.ConfigureEndpoints(app);
}

// --- Настройка Middleware (Порядок важен!) ---

if (app.Environment.IsDevelopment())
{
    // Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Порядок этих двух методов критичен:
app.UseAuthentication(); // КТО пользователь?
app.UseAuthorization();  // ЧТО ему можно делать?

// 5. Маппинг маршрутов
app.MapControllers(); // Добавлено для работы ваших [ApiController]
app.MapGroup("/identity").MapIdentityApi<AppUser>(); // Стандартные эндпоинты Identity

app.Run();