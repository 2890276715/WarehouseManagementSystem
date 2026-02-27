using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using WarehouseManagementSystem.Data;
using WarehouseManagementSystem.Services;

namespace WarehouseManagementSystem
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Day 2: 数据库集成与EF Core实战 ===");
            Console.WriteLine();

            try
            {
                // 1. 配置依赖注入
                var services = new ServiceCollection();

                // 添加配置
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                services.AddSingleton<IConfiguration>(configuration);

                // 添加DbContext
                services.AddDbContext<WarehouseDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("WarehouseDb")));

                // 添加数据库仓储服务
                services.AddScoped<IWarehouseService, DatabaseWarehouseService>();

                // 构建服务提供者
                var serviceProvider = services.BuildServiceProvider();

                // 2. 测试数据库连接
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

                    Console.WriteLine("正在测试数据库连接...");
                    bool canConnect = await dbContext.Database.CanConnectAsync();

                    if (canConnect)
                    {
                        Console.WriteLine("✅ 数据库连接成功！");
                        Console.WriteLine($"数据库名称: {dbContext.Database.GetDbConnection().Database}");

                        // 获取数据库统计信息
                        var productCount = await dbContext.Products.CountAsync();
                        var warehouseCount = await dbContext.Warehouses.CountAsync();
                        var transactionCount = await dbContext.InventoryTransactions.CountAsync();

                        Console.WriteLine($"📊 当前数据统计:");
                        Console.WriteLine($"   产品数: {productCount}");
                        Console.WriteLine($"   仓库数: {warehouseCount}");
                        Console.WriteLine($"   流水记录: {transactionCount}");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("❌ 数据库连接失败！");
                        return;
                    }
                }

                // 3. 测试数据库仓储服务
                Console.WriteLine("=== 测试数据库仓储服务 ===");
                Console.WriteLine();

                using (var scope = serviceProvider.CreateScope())
                {
                    var dbService = scope.ServiceProvider.GetRequiredService<DatabaseWarehouseService>();

                    // 测试1：获取所有产品
                    Console.WriteLine("1. 获取所有产品列表:");
                    var allProducts = await dbService.GetAllProductsAsync();
                    foreach (var product in allProducts)
                    {
                        Console.WriteLine($"   - {product.Id}: {product.Name} (库存: {product.Quantity}, 价格: ¥{product.Price})");
                    }
                    Console.WriteLine();

                    // 测试2：搜索产品
                    Console.WriteLine("2. 搜索包含'可乐'的产品:");
                    var searchResults = await dbService.SearchProductsAsync("可乐");
                    foreach (var product in searchResults)
                    {
                        Console.WriteLine($"   - 找到: {product.Name} (条码: {product.Barcode})");
                    }
                    Console.WriteLine();

                    // 测试3：获取低库存产品
                    Console.WriteLine("3. 检查低库存产品(阈值<20):");
                    var lowStockProducts = await dbService.GetLowStockProductsAsync(20);
                    foreach (var product in lowStockProducts)
                    {
                        Console.WriteLine($"   - ⚠️ {product.Name} 库存仅剩 {product.Quantity}");
                    }
                    Console.WriteLine();

                    // 测试4：模拟入库操作
                    Console.WriteLine("4. 模拟产品入库操作:");
                    Console.Write("   请输入要入库的产品ID (按Enter使用默认1): ");
                    string input = Console.ReadLine();
                    int productId = string.IsNullOrEmpty(input) ? 1 : int.Parse(input);

                    Console.Write("   请输入入库数量 (按Enter使用默认10): ");
                    input = Console.ReadLine();
                    int quantity = string.IsNullOrEmpty(input) ? 10 : int.Parse(input);

                    bool success = await dbService.StockInAsync(productId, 1, quantity, "TEST-IN-001", "测试入库");
                    if (success)
                    {
                        Console.WriteLine("   ✅ 入库成功！");
                        // 显示更新后的库存
                        var updatedProduct = await dbService.GetProductByIdAsync(productId);
                        Console.WriteLine($"   当前库存: {updatedProduct.Quantity}");
                    }
                    else
                    {
                        Console.WriteLine("   ❌ 入库失败！");
                    }
                    Console.WriteLine();

                    // 测试5：模拟出库操作
                    Console.WriteLine("5. 模拟产品出库操作:");
                    Console.Write("   请输入要出库的产品ID (按Enter使用默认1): ");
                    input = Console.ReadLine();
                    productId = string.IsNullOrEmpty(input) ? 1 : int.Parse(input);

                    Console.Write("   请输入出库数量 (按Enter使用默认5): ");
                    input = Console.ReadLine();
                    quantity = string.IsNullOrEmpty(input) ? 5 : int.Parse(input);

                    success = await dbService.StockOutAsync(productId, 1, quantity, "TEST-OUT-001", "测试出库");
                    if (success)
                    {
                        Console.WriteLine("   ✅ 出库成功！");
                        var updatedProduct = await dbService.GetProductByIdAsync(productId);
                        Console.WriteLine($"   当前库存: {updatedProduct.Quantity}");
                    }
                    else
                    {
                        Console.WriteLine("   ❌ 出库失败！库存可能不足。");
                    }
                    Console.WriteLine();

                    // 测试6：查看最近流水记录
                    Console.WriteLine("6. 最近5条库存流水记录:");
                    var recentTransactions = await dbService.GetRecentTransactionsAsync(5);
                    foreach (var transaction in recentTransactions)
                    {
                        string type = transaction.TransactionType == "IN" ? "📥 入库" : "📤 出库";
                        Console.WriteLine($"   {type} - {transaction.Product?.Name} ×{transaction.Quantity} ({transaction.CreatedDate:HH:mm:ss})");
                    }
                    Console.WriteLine();

                    // 测试7：添加新产品
                    Console.WriteLine("7. 测试添加新产品:");
                    var newProduct = new Models.Product
                    {
                        Name = "农夫山泉 550ml",
                        Barcode = "6921168509256",
                        Price = 2.00m,
                        Quantity = 200,
                        Category = "饮料",
                        Description = "天然饮用水"
                    };

                    try
                    {
                        var addedProduct = await dbService.AddProductAsync(newProduct);
                        Console.WriteLine($"   ✅ 新产品添加成功！ID: {addedProduct.Id}");

                        // 验证添加
                        var fetchedProduct = await dbService.GetProductByBarcodeAsync("6921168509256");
                        Console.WriteLine($"   验证: {fetchedProduct.Name} 库存: {fetchedProduct.Quantity}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   ❌ 添加失败: {ex.Message}");
                    }
                    Console.WriteLine();

                    // 测试8：删除产品（软删除）
                    Console.WriteLine("8. 测试删除产品:");
                    Console.Write("   请输入要删除的产品ID (按Enter跳过): ");
                    input = Console.ReadLine();

                    if (!string.IsNullOrEmpty(input))
                    {
                        productId = int.Parse(input);
                        success = await dbService.DeleteProductAsync(productId);
                        Console.WriteLine(success ? "   ✅ 产品已标记为删除" : "   ❌ 删除失败，产品可能不存在");
                    }

                    Console.WriteLine();
                }

                Console.WriteLine("=== 数据库测试完成 ===");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 发生错误: {ex.Message}");
                Console.WriteLine($"详细信息: {ex}");
            }

            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }
    }
}