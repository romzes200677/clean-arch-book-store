using Microsoft.EntityFrameworkCore;

namespace BookStore.User.Infrastructure;

public class UsersDbContext : DbContext
{
    // 1. DbSet для твоей сущности (Агрегата)
    public DbSet<Domain.User> Users { get; set; } = null!;

    // 2. Конструктор, принимающий options
    public UsersDbContext(DbContextOptions<UsersDbContext> options) 
        : base(options)
    {
    }

    // 3. Настройка модели (Fluent API)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ВОТ САМАЯ ВАЖНАЯ ЧАСТЬ ДЛЯ МОДУЛЬНОГО АРХИТЕКТОРА:
        // Мы говорим EF: "Найди все классы IEntityTypeConfiguration в этой сборке (Infrastructure) и примени их".
        // Это позволяет вынести конфигурацию каждой таблицы в отдельный файл.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
    }
}