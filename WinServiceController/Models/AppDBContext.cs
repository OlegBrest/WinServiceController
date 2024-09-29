using Microsoft.EntityFrameworkCore;

namespace WinServiceController.Models
{
    /// <summary>
    /// Класс схемы БД
    /// </summary>
    public class AppDBContext : DbContext
    {
        //        public DbSet<Models.ServiceControllerValuesDB> Services { get; set; } = null;
        public DbSet<Models.TrackedItemsModel> TrackedItems { get; set; } = null;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Tracked.db")
                .EnableSensitiveDataLogging();
        }
    }
}
