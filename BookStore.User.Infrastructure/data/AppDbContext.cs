using BookStore.User.Application.Interfaces;
using BookStore.User.Infrastructure.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookStore.User.Infrastructure.data;

public class AppDbContext: IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>,IUnitOfWork
{
    public DbSet<Domain.User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    private IDbContextTransaction? _currentTransaction;
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
        
        //User
        modelBuilder.Entity<Domain.User>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Маппинг Enum в строку
            entity.ToTable("Users"); // Явно задаем имя таблицы
            entity.Property(e => e.Role)
                .HasConversion<string>() 
                .HasMaxLength(20); // Рекомендуется ограничить длину
        });
        
        // Конфигурация RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(e => e.Id);
            
            // Индекс для быстрого поиска токена
            entity.HasIndex(e => e.Token).IsUnique();

            // Связь: при удалении пользователя удаляются и его токены
            entity.HasOne(rt => rt.User)
                .WithMany() 
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction ??= await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            if (_currentTransaction != null) await _currentTransaction.CommitAsync(cancellationToken);
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }
    }
}