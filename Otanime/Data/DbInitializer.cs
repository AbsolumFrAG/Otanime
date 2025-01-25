using Microsoft.AspNetCore.Identity;
using Otanime.Models;

namespace Otanime.Data;

public class DbInitializer
{
    public async Task Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.EnsureCreatedAsync();

        // Création des rôles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        // Création de l'admin
        var adminUser = await userManager.FindByEmailAsync("admin@otanime.com");
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = "admin@otanime.com",
                Email = "admin@otanime.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "AdminPassword123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Ajout des figurines si la table est vide
        if (!context.Products.Any())
        {
            var figurines = new List<Product>
            {
                // Figurines Naruto
                new()
                { 
                    Name = "Naruto Uzumaki - Mode Sage", 
                    Price = 89.99m, 
                    Description = "Figurine détaillée de Naruto en mode sage avec effets spéciaux", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20220425/12629/98168/large/affba8a8d19383a6084695f0f2bb80a6.jpg",
                    Category = "Naruto",
                    StockQuantity = 15
                },
                new()
                { 
                    Name = "Sasuke Uchiha - Rinnegan", 
                    Price = 99.99m, 
                    Description = "Édition limitée avec épée Kusanagi", 
                    ImageUrl = "https://www.shin-sekai.fr/69128-large_default/boruto-naruto-next-generations-figurine-uchiha-sasuke-kizuna-relation-figuarts-zero.jpg",
                    Category = "Naruto",
                    InStock = false
                },

                // Figurines One Piece
                new()
                { 
                    Name = "Monkey D. Luffy - Gear Fifth", 
                    Price = 149.99m, 
                    Description = "Figurine 30cm avec base thématique", 
                    ImageUrl = "https://www.sugoishop.fr/20225-large_default/one-piece-figurine-luffy-gear-5-ichiban-kuji-four-new-emperors-lot-c.jpg",
                    Category = "One Piece",
                    StockQuantity = 8
                },
                new()
                { 
                    Name = "Roronoa Zoro - 3 Épées", 
                    Price = 129.99m, 
                    Description = "Posture de combat avec effets de tranchants", 
                    ImageUrl = "https://www.sugoishop.fr/20288-large_default/one-piece-figurine-zoro-roronoa-king-of-artist-wano-kuni-ii.jpg",
                    Category = "One Piece"
                },

                // Figurines Demon Slayer
                new()
                { 
                    Name = "Tanjiro Kamado - Masque de chasseur", 
                    Price = 79.99m, 
                    Description = "Avec effet d'eau nichirine", 
                    ImageUrl = "https://www.shin-sekai.fr/52768-large_default/demon-slayer-kimetsu-no-yaiba-figurine-tanjiro-kamado-vol-19-special-color-ver.jpg",
                    Category = "Demon Slayer"
                },
                new()
                { 
                    Name = "Nezuko - Forme démoniaque", 
                    Price = 89.99m, 
                    Description = "Édition spéciale avec boîte bambou", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20230803/14775/119190/large/113d794ff4cc8f64a93966ce030f5314.jpg",
                    Category = "Demon Slayer"
                },

                // Figurines My Hero Academia
                new()
                { 
                    Name = "Izuku Midoriya - One For All 100%", 
                    Price = 109.99m, 
                    Description = "Effets lumineux inclus", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20231117/15235/123515/large/173b3abf235b1c192e6d15a4e0bfc715.jpg",
                    Category = "My Hero Academia"
                },
                new()
                { 
                    Name = "Katsuki Bakugo - Explosion", 
                    Price = 99.99m, 
                    Description = "Posture dynamique avec effets pyrotechniques", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/36394/rubgyqehLY8ZUpGB5HaPk3t1FK97CQRn.jpg",
                    Category = "My Hero Academia"
                },

                // Figurines Attack on Titan
                new()
                { 
                    Name = "Eren Yeager - Titan Assaillant", 
                    Price = 199.99m, 
                    Description = "Figurine géante 45cm", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20140924/4623/30566/large/7cf76eb2aa28192e3aa274c7dcbc32b1.jpg",
                    Category = "Attack on Titan"
                },
                new()
                { 
                    Name = "Levi Ackerman - Équipement 3DMG", 
                    Price = 159.99m, 
                    Description = "Avec base mur en 3D", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20231018/15102/122418/large/0d149dc3c13c1c5eb73387207ed35820.jpg",
                    Category = "Attack on Titan"
                },

                // Figurines Jujutsu Kaisen
                new()
                { 
                    Name = "Gojo Satoru - Domaine Infini", 
                    Price = 179.99m, 
                    Description = "Effets translucides spéciaux", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/54549/1PHxmCfck03URVGZ84eqh5uBFn9TKSM6.jpg",
                    Category = "Jujutsu Kaisen"
                },
                new()
                { 
                    Name = "Itadori Yuji - Black Flash", 
                    Price = 89.99m, 
                    Description = "Édition limitée sang sukuna", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20210518/11205/84757/large/1bb96c43fd652b39b552b88e927f52ea.jpg",
                    Category = "Jujutsu Kaisen"
                },

                // Figurines Sword Art Online
                new()
                { 
                    Name = "Kirito - Dual Wield", 
                    Price = 129.99m, 
                    Description = "Avec épées luminescentes", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/61229/7FbVDKjZSBUP5WXekQ0R6zT8udtnGvLh.jpg",
                    Category = "Sword Art Online"
                },
                new()
                { 
                    Name = "Asuna - Lame Flamboyante", 
                    Price = 119.99m, 
                    Description = "Costume de fée de l'éclair", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20211027/11947/91516/large/20c3bf61008389ae1ff05a4ea03781f5.jpg",
                    Category = "Sword Art Online"
                },

                // Figurines supplémentaires (continuer jusqu'à 30)
                new()
                { 
                    Name = "Gon Freecss - Jajanken", 
                    Price = 89.99m, 
                    Description = "Figurine Hunter x Hunter", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20221213/13670/108519/large/bfa503c0635e7936ec59bd98e0dc3b82.jpg",
                    Category = "Autres"
                },
                new()
                { 
                    Name = "Kakashi Hatake - Chidori", 
                    Price = 99.99m, 
                    Description = "Éclair lumineux intégré", 
                    ImageUrl = "https://www.mangatori.fr/2588022-large_default/bandai-ban66660-4-naruto-shippuden-figuarts-zero-extra-battle-kakash.jpg",
                    Category = "Naruto"
                },
                new()
                { 
                    Name = "Light Yagami - Death Note", 
                    Price = 69.99m, 
                    Description = "Avec carnet miniature", 
                    ImageUrl = "https://www.clubotaku.fr/wp-content/uploads/2021/11/figurine-death-note-light-yagami-25-cm-1.jpg",
                    Category = "Death Note"
                },
                new()
                { 
                    Name = "Edward Elric - Alchimie", 
                    Price = 109.99m, 
                    Description = "Avec bras automail", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20200907/10021/74003/large/c574630081238669e330f18e3d0fb8ee.jpg",
                    Category = "Fullmetal Alchemist"
                },
                new()
                { 
                    Name = "Goku Ultra Instinct", 
                    Price = 299.99m, 
                    Description = "Figurine 50cm édition collector", 
                    ImageUrl = "https://goku-shop.fr/wp-content/uploads/2024/12/product-image-1310181915_jpg_1.jpg.webp",
                    Category = "Dragon Ball"
                },
                new()
                { 
                    Name = "Vegeta Blue Evolution", 
                    Price = 279.99m, 
                    Description = "Avec aura énergétique", 
                    ImageUrl = "https://www.shin-sekai.fr/13179-large_default/dragon-ball-z-figurine-vegeta-ssj-blue-chosenshiretsuden.jpg",
                    Category = "Dragon Ball"
                },
                new()
                { 
                    Name = "L Lawliet - Detective", 
                    Price = 59.99m, 
                    Description = "Posture assise avec sucrerie", 
                    ImageUrl = "https://www.abystyle.com/9769275-large_default/death-note-figurine-l.jpg",
                    Category = "Death Note"
                },
                new()
                { 
                    Name = "Saber - Excalibur", 
                    Price = 199.99m, 
                    Description = "Figurine Fate/stay night", 
                    ImageUrl = "https://www.shin-sekai.fr/50535-large_default/fate-grand-order-figurine-saber-white-dress-renewal-ver-.jpg",
                    Category = "Fate"
                },
                new()
                { 
                    Name = "Rem - Demon Maid",
                    Price = 149.99m, 
                    Description = "Édition anniversaire Re:Zero", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/56932/TqnGJku7Hp6hCj8Kw0SiZzN4BfWtrAUb.jpg",
                    Category = "Re:Zero"
                },
                new()
                { 
                    Name = "Zero Two - Darling", 
                    Price = 129.99m, 
                    Description = "Costume de pilote détaillé", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/1136873/i39my7bzeXrj6n8p0wvVfxYDPL1BcqgJ.jpg",
                    Category = "Darling in the Franxx"
                },
                new()
                { 
                    Name = "Mikasa Ackerman - Écharpe", 
                    Price = 89.99m, 
                    Description = "Avec effets de cape dynamiques", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20140421/4388/28678/large/a7b90a77d415512a772e1770d79ca16f.jpg",
                    Category = "Attack on Titan"
                },
                new()
                { 
                    Name = "All Might - United States of Smash", 
                    Price = 189.99m, 
                    Description = "Effet de fumée inclus", 
                    ImageUrl = "https://chibi-akihabara.com/79906/my-hero-academia-figurine-all-might-grandista-reproduction-.jpg",
                    Category = "My Hero Academia"
                },
                new()
                { 
                    Name = "Killua Zoldyck - Godspeed", 
                    Price = 109.99m, 
                    Description = "Avec effets électriques", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20230724/14716/118728/large/7fb5d575654970da9fa4d95fcaf58e23.jpg",
                    Category = "Hunter x Hunter"
                },
                new()
                { 
                    Name = "Erza Scarlet - Armure Céleste", 
                    Price = 139.99m, 
                    Description = "Avec 4 armes interchangeables", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/61181/DsBLYvnQpqw8xFVRAdyMtrJaZXHPTiS5.jpg",
                    Category = "Fairy Tail"
                },
                new()
                { 
                    Name = "Meliodas - Dragon Handle", 
                    Price = 159.99m, 
                    Description = "Figurine Seven Deadly Sins", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20210728/11567/87834/large/ed709e03fcd59bdb799cf951bf8eca38.jpg",
                    Category = "Seven Deadly Sins"
                },
                new()
                { 
                    Name = "Rimuru Tempest - Forme Slime", 
                    Price = 79.99m, 
                    Description = "Édition translucide glow-in-the-dark", 
                    ImageUrl = "https://www.goodsmile.com/gsc-webrevo-sdk-storage-prd/product/image/product/20230906/14921/120681/large/4667c5dbdd7ee3827be05f9315f7c8b3.jpg",
                    Category = "Tensei Shitara Slime Datta Ken"
                }
            };

            await context.Products.AddRangeAsync(figurines);
            await context.SaveChangesAsync();
        }
    }
}