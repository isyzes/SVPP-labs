using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Lab_8.Models
{
    public class ServicesDbContext : DbContext
    {
        public ServicesDbContext() : base("DefaultConnection")
        {
            // Инициализатор базы данных
            Database.SetInitializer(new ServicesDbInitializer());
        }

        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Отключаем каскадное удаление
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // Настройки таблицы
            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Service>()
                .Property(s => s.RegistrationDate)
                .HasColumnType("datetime");
        }
    }

    // Инициализатор базы данных
    public class ServicesDbInitializer : CreateDatabaseIfNotExists<ServicesDbContext>
    {
        protected override void Seed(ServicesDbContext context)
        {
            base.Seed(context);

            // Добавляем начальные данные
            var services = new[]
            {
                new Service
                {
                    Name = "Ремонт холодильников",
                    Description = "Качественный ремонт холодильников всех марок",
                    Category = "Ремонт бытовой техники",
                    Price = 1500.00m,
                    Provider = "ООО 'ХолодСервис'",
                    Address = "ул. Ленина, 10",
                    Phone = "+7 (495) 123-45-67",
                    RegistrationDate = DateTime.Now.AddDays(-30),
                    IsActive = true
                },
                new Service
                {
                    Name = "Химчистка мебели",
                    Description = "Профессиональная химчистка диванов, кресел, матрасов",
                    Category = "Уборка и чистка",
                    Price = 3000.00m,
                    Provider = "Клининговая компания 'Чистота'",
                    Address = "пр. Мира, 25",
                    Phone = "+7 (495) 234-56-78",
                    RegistrationDate = DateTime.Now.AddDays(-15),
                    IsActive = true
                },
                new Service
                {
                    Name = "Установка кондиционеров",
                    Description = "Установка и обслуживание кондиционеров",
                    Category = "Установка техники",
                    Price = 5000.00m,
                    Provider = "ИП Петров А.С.",
                    Address = "ул. Гагарина, 15",
                    Phone = "+7 (495) 345-67-89",
                    RegistrationDate = DateTime.Now.AddDays(-7),
                    IsActive = true
                },
                new Service
                {
                    Name = "Ремонт стиральных машин",
                    Description = "Ремонт всех марок стиральных машин, замена деталей",
                    Category = "Ремонт бытовой техники",
                    Price = 2000.00m,
                    Provider = "ООО 'МастерТехник'",
                    Address = "ул. Пушкина, 5",
                    Phone = "+7 (495) 456-78-90",
                    RegistrationDate = DateTime.Now.AddDays(-20),
                    IsActive = true
                },
                new Service
                {
                    Name = "Услуги сантехника",
                    Description = "Установка, ремонт и замена сантехнического оборудования",
                    Category = "Ремонт квартир",
                    Price = 2500.00m,
                    Provider = "ИП Сидоров В.П.",
                    Address = "ул. Чехова, 12",
                    Phone = "+7 (495) 567-89-01",
                    RegistrationDate = DateTime.Now.AddDays(-10),
                    IsActive = true
                }
            };

            context.Services.AddRange(services);
            context.SaveChanges();
        }
    }
}
