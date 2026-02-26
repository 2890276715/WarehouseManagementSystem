using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagementSystem
{
    //产品实体类
    public class Product
    {
        // 2. 属性 (Properties)。这是C#中定义类特征的推荐方式。
        //    get; set; 表示这个属性可以被读取和赋值。
        public int Id { get; set; }          // 产品唯一编号
        public string Name { get; set; }     // 产品名称
        public string Barcode { get; set; }  // 产品条码（未来用于扫码）
        public decimal Price { get; set; }   // 产品单价
        public int Quantity { get; set; }    // 产品库存数量

        // 3. 方法 (Method)。类能执行的操作。void 表示这个方法不返回值。 这个方法用于减少库存。
        public void ReduceStock(int amount)
        {
            if (amount <= Quantity) // 检查库存是否充足
            {
                Quantity -= amount; // 减少库存
                Console.WriteLine($"产品【{Name}】出库{amount}件，剩余库存{Quantity}。");
            }
            else
            {
                Console.WriteLine($"错误：产品【{Name}】库存不足。当前库存{Quantity}，尝试出库{amount}。");
            }
        }

        // 4. 重写 ToString 方法。当需要将对象转为字符串显示时（如Console.Write），会调用此方法。
        public override string ToString()
        {
            return $"ID: {Id}, 品名: {Name}, 条码: {Barcode}, 单价: ¥{Price}, 库存: {Quantity}";
        }
    }
}
