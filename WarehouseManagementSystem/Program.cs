using System;
using System.Collections.Generic;

namespace WarehouseManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 仓储系统核心实体测试 ===");
            Console.WriteLine();

            // 1. 实例化（创建）一个Product对象
            Product myFirstProduct = new Product(); // new 关键字就是“创建”
            myFirstProduct.Id = 1;
            myFirstProduct.Name = "可口可乐 330ml 罐装";
            myFirstProduct.Barcode = "6954767415688";
            myFirstProduct.Price = 2.5m; // m 表示decimal类型
            myFirstProduct.Quantity = 100;

            // 2. 调用ToString方法并打印
            Console.WriteLine("创建的第一个产品：");
            Console.WriteLine(myFirstProduct.ToString()); // 显式调用
            Console.WriteLine(myFirstProduct); // 隐式调用，效果相同
            Console.WriteLine();

            // 3. 调用方法，操作对象
            Console.WriteLine("尝试出库操作：");
            myFirstProduct.ReduceStock(10);  // 正常出库
            myFirstProduct.ReduceStock(200); // 触发库存不足错误
            Console.WriteLine();

            // 4. 使用对象初始化器，更简洁地创建对象
            Product secondProduct = new Product
            {
                Id = 2,
                Name = "康师傅红烧牛肉面",
                Barcode = "6920152401013",
                Price = 4.0m,
                Quantity = 50
            };
            Console.WriteLine("创建的第二个产品：");
            Console.WriteLine(secondProduct);
            Console.WriteLine();

            // 5. 创建仓库对象
            Warehouse mainWarehouse = new Warehouse { Id = 1, Name = "总仓", Location = "北京" };
            Console.WriteLine("创建的仓库：");
            Console.WriteLine(mainWarehouse);

            Console.WriteLine();
            Console.WriteLine("=== 上午任务完成！按任意键退出。 ===");

            Console.WriteLine("\n=== 下午任务：内存仓库管理系统测试 ===");
            Console.WriteLine();

            // 1. 创建内存仓库
            InMemoryWarehouse myWarehouse = new InMemoryWarehouse();

            // 2. 添加上午创建的产品
            myWarehouse.AddProduct(myFirstProduct);
            myWarehouse.AddProduct(secondProduct);

            // 3. 再添加一些新产品
            myWarehouse.AddProduct(new Product { Id = 3, Name = "iPhone 16 Pro", Barcode = "888888", Price = 8999m, Quantity = 10 });
            myWarehouse.AddProduct(new Product { Id = 4, Name = "华为 MateBook", Barcode = "999999", Price = 6999m, Quantity = 5 });

            Console.WriteLine("\n--- 打印所有产品 ---");
            foreach (var product in myWarehouse.GetAllProducts())
            {
                Console.WriteLine($"  - {product}");
            }

            Console.WriteLine("\n--- 搜索名称包含‘可乐’的产品 ---");
            var searchResults = myWarehouse.SearchProductsByName("可乐");
            foreach (var p in searchResults)
            {   
                Console.WriteLine($"  - 找到：{p.Name}");
            }

            Console.WriteLine("\n--- 更新ID为2的产品的库存 ---");
            myWarehouse.UpdateProductStock(2, 999); // 把泡面库存改成999

            Console.WriteLine("\n--- 尝试删除一个产品，再打印所有产品 ---");
            myWarehouse.RemoveProduct(1); // 删除可乐
            Console.WriteLine("当前库存清单：");
            foreach (var product in myWarehouse.GetAllProducts())
            {
                Console.WriteLine($"  - {product}");
            }
            Console.ReadKey(); // 等待用户按键，防止窗口一闪而过
        }
    }
}
