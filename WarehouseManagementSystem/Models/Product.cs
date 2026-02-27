using System;
using System.Collections.Generic;

namespace WarehouseManagementSystem.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Barcode { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public string? Category { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastModified { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
}
