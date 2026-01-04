using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStore.User.Infrastructure.data;

public class AppDbContext: IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public DbSet<Domain.User> Users { get; set; } = null!;
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 
        // ВОТ САМАЯ ВАЖНАЯ ЧАСТЬ ДЛЯ МОДУЛЬНОГО АРХИТЕКТОРА:
        // Мы говорим EF: "Найди все классы IEntityTypeConfiguration в этой сборке (Infrastructure) и примени их".
        // Это позволяет вынести конфигурацию каждой таблицы в отдельный файл.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.Entity<Domain.User>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Маппинг Enum в строку
            entity.ToTable("Users"); // Явно задаем имя таблицы
            entity.Property(e => e.Role)
                .HasConversion<string>() 
                .HasMaxLength(20); // Рекомендуется ограничить длину
        });
    }
}