using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Otanime.Migrations
{
    /// <inheritdoc />
    public partial class AddSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Description", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, "Figurine de Naruto Uzumaki en pose de combat", "Naruto Uzumaki", 79.99m, 10 },
                    { 2, "Figurine de Sasuke Uchiha avec Sharingan", "Sasuke Uchiha", 89.99m, 8 },
                    { 3, "Figurine de Goku en mode Super Saiyan", "Goku Super Saiyan", 99.99m, 12 },
                    { 4, "Figurine de Vegeta en armure de combat", "Vegeta", 84.99m, 9 },
                    { 5, "Figurine de Monkey D. Luffy en Gear 4", "Luffy Gear 4", 94.99m, 7 },
                    { 6, "Figurine de Roronoa Zoro avec trois sabres", "Zoro", 79.99m, 11 },
                    { 7, "Figurine d'Eren Yeager en tenue de combat", "Eren Yeager", 69.99m, 6 },
                    { 8, "Figurine de Mikasa Ackerman avec ses équipements", "Mikasa Ackerman", 74.99m, 8 },
                    { 9, "Figurine de Light Yagami avec Death Note", "Light Yagami", 64.99m, 5 },
                    { 10, "Figurine de L, le détective", "L", 69.99m, 7 },
                    { 11, "Figurine de Tanjiro avec sa hache", "Tanjiro Kamado", 89.99m, 10 },
                    { 12, "Figurine de Nezuko en mode combat", "Nezuko Kamado", 84.99m, 9 },
                    { 13, "Figurine d'Ichigo avec Zangetsu", "Ichigo Kurosaki", 79.99m, 8 },
                    { 14, "Figurine de Kakashi en tenue ninja", "Kakashi Hatake", 74.99m, 7 },
                    { 15, "Figurine de Saitama du One Punch Man", "Saitama", 69.99m, 6 },
                    { 16, "Figurine de All Might en mode héroïque", "All Might", 94.99m, 5 },
                    { 17, "Figurine d'Edward Elric en alchimiste", "Edward Elric", 84.99m, 9 },
                    { 18, "Figurine de Levi en tenue militaire", "Levi Ackerman", 89.99m, 8 },
                    { 19, "Figurine de Sailor Moon en transformation", "Sailor Moon", 79.99m, 7 },
                    { 20, "Figurine de Gon de Hunter x Hunter", "Gon Freecss", 74.99m, 6 },
                    { 21, "Figurine de Lelouch de Code Geass", "Lelouch Lamperouge", 89.99m, 5 },
                    { 22, "Figurine de Spike de Cowboy Bebop", "Spike Spiegel", 79.99m, 4 },
                    { 23, "Figurine de Ryuko de Kill la Kill", "Ryuko Matoi", 84.99m, 6 },
                    { 24, "Figurine de Killua de Hunter x Hunter", "Killua Zoldyck", 74.99m, 7 },
                    { 25, "Figurine de Rin de Fate/Stay Night", "Rin Tohsaka", 69.99m, 5 },
                    { 26, "Figurine de Zero Two de Darling in the Franxx", "Zero Two", 94.99m, 8 },
                    { 27, "Figurine de Saber Alter de Fate", "Saber Alter", 89.99m, 6 },
                    { 28, "Figurine d'Asuna de Sword Art Online", "Asuna Yuuki", 79.99m, 7 },
                    { 29, "Figurine de Rem de Re:Zero", "Rem", 84.99m, 5 },
                    { 30, "Figurine d'Esdeath de Akame ga Kill", "Esdeath", 94.99m, 4 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "City", "Country", "Email", "FirstName", "IsAdmin", "LastName", "Password", "Phone", "PostalCode" },
                values: new object[,]
                {
                    { 1, null, null, null, "lou@admin.com", "Lou", true, "Admin", "$2a$11$qEkezkoB72UlLo6MQ/m3tOrHm1nUX6m4EieXix9rlEFxMJCzn8zhW", null, null },
                    { 2, null, null, null, "capucine@admin.com", "Capucine", true, "Admin", "$2a$11$qEkezkoB72UlLo6MQ/m3tOrHm1nUX6m4EieXix9rlEFxMJCzn8zhW", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);
        }
    }
}
