using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Services
{
    public interface IWarehouseService
    {
        // 产品管理
        Task<Product> AddProductAsync(Product product);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> GetProductByBarcodeAsync(string barcode);
        Task<List<Product>> SearchProductsAsync(string keyword);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);

        // 库存操作
        Task<bool> StockInAsync(int productId, int warehouseId, int quantity, string referenceNumber = "", string notes = "");
        Task<bool> StockOutAsync(int productId, int warehouseId, int quantity, string referenceNumber = "", string notes = "");

        // 报表查询
        Task<List<dynamic>> GetLowStockProductsAsync(int threshold = 10);
        Task<List<InventoryTransaction>> GetRecentTransactionsAsync(int count = 20);
    }
}