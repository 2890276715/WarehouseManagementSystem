using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagementSystem
{
    //仓库实体类
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; } // 仓库名称
        public string Location { get; set; } // 仓库地址

        // 一个仓库可以存放很多产品，这个关系我们明天用数据库表示。
        // public List<Product> Products { get; set; } // 先注释掉，明天会用到

        public override string ToString()
        {
            return $"仓库[{Id}]: {Name} (位置: {Location})";
        }
    }
}
