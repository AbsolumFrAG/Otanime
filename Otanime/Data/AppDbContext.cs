using Microsoft.EntityFrameworkCore;

namespace Otanime.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Admin Users with static password hashes
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    UserId = 1, 
                    FirstName = "Lou", 
                    LastName = "Admin", 
                    Email = "lou@admin.com", 
                    IsAdmin = true,
                    Password = "$2a$11$qEkezkoB72UlLo6MQ/m3tOrHm1nUX6m4EieXix9rlEFxMJCzn8zhW" //admin123
                },
                new User 
                { 
                    UserId = 2, 
                    FirstName = "Capucine", 
                    LastName = "Admin", 
                    Email = "capucine@admin.com", 
                    IsAdmin = true,
                    Password = "$2a$11$qEkezkoB72UlLo6MQ/m3tOrHm1nUX6m4EieXix9rlEFxMJCzn8zhW" //admin123
                }
            );

            // Seed Anime Figurines
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Name = "Naruto Uzumaki", Price = 79.99m, Description = "Figurine de Naruto Uzumaki en pose de combat", Stock = 10 },
                new Product { ProductId = 2, Name = "Sasuke Uchiha", Price = 89.99m, Description = "Figurine de Sasuke Uchiha avec Sharingan", Stock = 8 },
                new Product { ProductId = 3, Name = "Goku Super Saiyan", Price = 99.99m, Description = "Figurine de Goku en mode Super Saiyan", Stock = 12 },
                new Product { ProductId = 4, Name = "Vegeta", Price = 84.99m, Description = "Figurine de Vegeta en armure de combat", Stock = 9 },
                new Product { ProductId = 5, Name = "Luffy Gear 4", Price = 94.99m, Description = "Figurine de Monkey D. Luffy en Gear 4", Stock = 7 },
                new Product { ProductId = 6, Name = "Zoro", Price = 79.99m, Description = "Figurine de Roronoa Zoro avec trois sabres", Stock = 11 },
                new Product { ProductId = 7, Name = "Eren Yeager", Price = 69.99m, Description = "Figurine d'Eren Yeager en tenue de combat", Stock = 6 },
                new Product { ProductId = 8, Name = "Mikasa Ackerman", Price = 74.99m, Description = "Figurine de Mikasa Ackerman avec ses équipements", Stock = 8 },
                new Product { ProductId = 9, Name = "Light Yagami", Price = 64.99m, Description = "Figurine de Light Yagami avec Death Note", Stock = 5 },
                new Product { ProductId = 10, Name = "L", Price = 69.99m, Description = "Figurine de L, le détective", Stock = 7 },
                new Product { ProductId = 11, Name = "Tanjiro Kamado", Price = 89.99m, Description = "Figurine de Tanjiro avec sa hache", Stock = 10 },
                new Product { ProductId = 12, Name = "Nezuko Kamado", Price = 84.99m, Description = "Figurine de Nezuko en mode combat", Stock = 9 },
                new Product { ProductId = 13, Name = "Ichigo Kurosaki", Price = 79.99m, Description = "Figurine d'Ichigo avec Zangetsu", Stock = 8 },
                new Product { ProductId = 14, Name = "Kakashi Hatake", Price = 74.99m, Description = "Figurine de Kakashi en tenue ninja", Stock = 7 },
                new Product { ProductId = 15, Name = "Saitama", Price = 69.99m, Description = "Figurine de Saitama du One Punch Man", Stock = 6 },
                new Product { ProductId = 16, Name = "All Might", Price = 94.99m, Description = "Figurine de All Might en mode héroïque", Stock = 5 },
                new Product { ProductId = 17, Name = "Edward Elric", Price = 84.99m, Description = "Figurine d'Edward Elric en alchimiste", Stock = 9 },
                new Product { ProductId = 18, Name = "Levi Ackerman", Price = 89.99m, Description = "Figurine de Levi en tenue militaire", Stock = 8 },
                new Product { ProductId = 19, Name = "Sailor Moon", Price = 79.99m, Description = "Figurine de Sailor Moon en transformation", Stock = 7 },
                new Product { ProductId = 20, Name = "Gon Freecss", Price = 74.99m, Description = "Figurine de Gon de Hunter x Hunter", Stock = 6 },
                new Product { ProductId = 21, Name = "Lelouch Lamperouge", Price = 89.99m, Description = "Figurine de Lelouch de Code Geass", Stock = 5 },
                new Product { ProductId = 22, Name = "Spike Spiegel", Price = 79.99m, Description = "Figurine de Spike de Cowboy Bebop", Stock = 4 },
                new Product { ProductId = 23, Name = "Ryuko Matoi", Price = 84.99m, Description = "Figurine de Ryuko de Kill la Kill", Stock = 6 },
                new Product { ProductId = 24, Name = "Killua Zoldyck", Price = 74.99m, Description = "Figurine de Killua de Hunter x Hunter", Stock = 7 },
                new Product { ProductId = 25, Name = "Rin Tohsaka", Price = 69.99m, Description = "Figurine de Rin de Fate/Stay Night", Stock = 5 },
                new Product { ProductId = 26, Name = "Zero Two", Price = 94.99m, Description = "Figurine de Zero Two de Darling in the Franxx", Stock = 8 },
                new Product { ProductId = 27, Name = "Saber Alter", Price = 89.99m, Description = "Figurine de Saber Alter de Fate", Stock = 6 },
                new Product { ProductId = 28, Name = "Asuna Yuuki", Price = 79.99m, Description = "Figurine d'Asuna de Sword Art Online", Stock = 7 },
                new Product { ProductId = 29, Name = "Rem", Price = 84.99m, Description = "Figurine de Rem de Re:Zero", Stock = 5 },
                new Product { ProductId = 30, Name = "Esdeath", Price = 94.99m, Description = "Figurine d'Esdeath de Akame ga Kill", Stock = 4 }
            );
        }
    }
}