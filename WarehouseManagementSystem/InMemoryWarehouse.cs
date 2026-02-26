using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagementSystem
{
    //服务类：专门负责在内存中管理产品集合。
    public class InMemoryWarehouse
    {
        // 1. 私有字段。这是我们真正的“货架”，一个Product对象的列表。
        private List<Product> _products = new List<Product>();

        // 2. 方法：添加产品 (Create)
        public void AddProduct(Product product) 
        {
            /// 简单的验证：防止添加空对象或ID重复（这里简化处理）
            if (product != null && !_products.Any(p => p.Id == product.Id))
            {
                _products.Add(product);
                Console.WriteLine($"已添加产品：{product.Name}");
            }
            else 
            {
                Console.WriteLine("添加产品失败：产品为空或ID已存在。");
            }
        }

        // 3. 方法：获取所有产品 (Read - All)
        public List<Product> GetAllProducts()
        {
            // 返回列表的副本，防止外部代码直接修改我们的内部货架。
            return new List<Product>(_products);
        }

        // 4. 方法：根据ID查找产品 (Read - By Id)
        //    这里使用了泛型集合的LINQ方法 FirstOrDefault
        public Product GetProductById(int id)
        {
            // 遍历_products，找到第一个Id匹配的产品，如果没找到就返回null。
            return _products.FirstOrDefault(p => p.Id == id);
        }

        // 5. 方法：根据名称搜索产品 (Read - By Name)
        public List<Product> SearchProductsByName(string keyword)
        {
            // Where是筛选，ToList()将结果转为新列表。
            // 这是LINQ的“方法语法”，非常直观。
            return _products
                    .Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)) // 忽略大小写包含关键词
                    .ToList();
        }

        // 6. 方法：更新产品库存 (Update)
        public bool UpdateProductStock(int productId, int newQuantity)
        {
            var product = GetProductById(productId);
            if (product != null)
            {
                product.Quantity = newQuantity;
                Console.WriteLine($"已更新产品【{product.Name}】库存为：{newQuantity}");
                return true; // 更新成功
            }
            Console.WriteLine($"更新失败：未找到ID为{productId}的产品。");
            return false; // 更新失败
        }

        // 7. 方法：删除产品 (Delete)
        public bool RemoveProduct(int productId)
        {
            var product = GetProductById(productId);
            if (product != null)
            {
                _products.Remove(product);
                Console.WriteLine($"已删除产品：{product.Name}");
                return true;
            }
            return false;
        }
    }
}
