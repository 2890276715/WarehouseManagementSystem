using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Data;
using WarehouseManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseManagementSystem.Services
{
    public class DatabaseWarehouseService : IWarehouseService
    {
        private readonly WarehouseDbContext _context;

        public DatabaseWarehouseService(WarehouseDbContext context)
        {
            _context = context;
        }

        // 1. 产品管理
        public async Task<Product> AddProductAsync(Product product)
        {
            try
            {
                // 检查条码是否重复
                bool exists = await _context.Products
                    .AnyAsync(p => p.Barcode == product.Barcode);

                if (exists)
                {
                    throw new InvalidOperationException($"产品条码 {product.Barcode} 已存在");
                }

                product.CreatedDate = DateTime.Now;
                product.LastModified = DateTime.Now;
                product.IsActive = true;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("数据库操作失败", ex);
            }
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive == true)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive == true);
        }

        public async Task<Product> GetProductByBarcodeAsync(string barcode)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Barcode == barcode && p.IsActive == true);
        }

        public async Task<List<Product>> SearchProductsAsync(string keyword)
        {
            return await _context.Products
                .Where(p => p.IsActive == true &&
                           (p.Name.Contains(keyword) ||
                            p.Barcode.Contains(keyword) ||
                            p.Category.Contains(keyword)))
                .ToListAsync();
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null)
                    return false;

                // 更新字段
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Description = product.Description;
                existingProduct.Category = product.Category;
                existingProduct.LastModified = DateTime.Now;

                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return false;

                // 软删除：标记为不活跃
                product.IsActive = false;
                product.LastModified = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        // 2. 库存操作
        public async Task<bool> StockInAsync(int productId, int warehouseId, int quantity, string referenceNumber = "", string notes = "")
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 更新产品库存
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    return false;

                product.Quantity += quantity;
                product.LastModified = DateTime.Now;

                // 记录流水
                var transactionRecord = new InventoryTransaction
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    TransactionType = "IN",
                    Quantity = quantity,
                    ReferenceNumber = referenceNumber,
                    Notes = notes,
                    CreatedDate = DateTime.Now
                };

                _context.InventoryTransactions.Add(transactionRecord);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> StockOutAsync(int productId, int warehouseId, int quantity, string referenceNumber = "", string notes = "")
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null || product.Quantity < quantity)
                    return false;

                product.Quantity -= quantity;
                product.LastModified = DateTime.Now;

                var transactionRecord = new InventoryTransaction
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    TransactionType = "OUT",
                    Quantity = quantity,
                    ReferenceNumber = referenceNumber,
                    Notes = notes,
                    CreatedDate = DateTime.Now
                };

                _context.InventoryTransactions.Add(transactionRecord);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // 3. 报表查询
        public async Task<List<dynamic>> GetLowStockProductsAsync(int threshold = 10)
        {
            return await _context.Products
                .Where(p => p.Quantity < threshold && p.IsActive == true)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Barcode,
                    p.Quantity,
                    p.Price,
                    Status = p.Quantity == 0 ? "缺货" : "低库存"
                })
                .ToListAsync<dynamic>();
        }

        public async Task<List<InventoryTransaction>> GetRecentTransactionsAsync(int count = 20)
        {
            return await _context.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.Warehouse)
                .OrderByDescending(t => t.CreatedDate)
                .Take(count)
                .ToListAsync();
        }
    }
}