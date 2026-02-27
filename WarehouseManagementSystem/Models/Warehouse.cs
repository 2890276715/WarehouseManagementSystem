using System;
using System.Collections.Generic;

namespace WarehouseManagementSystem.Models;

public partial class Warehouse
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Location { get; set; }

    public int? Capacity { get; set; }

    public string? ContactPhone { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
}
