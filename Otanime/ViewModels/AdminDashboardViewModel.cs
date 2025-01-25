using Otanime.Models;

namespace Otanime.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalProducts { get; set; }
    public int ProductsOutOfStock { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int TotalUsers { get; set; }
    public List<Order> RecentOrders { get; set; } = [];
    public List<Product> LowStockProducts { get; set; } = [];
}