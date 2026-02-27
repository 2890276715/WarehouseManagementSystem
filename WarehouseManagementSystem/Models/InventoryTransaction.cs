using System;
using System.Collections.Generic;

namespace WarehouseManagementSystem.Models;

public partial class InventoryTransaction
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public string TransactionType { get; set; } = null!;

    public int Quantity { get; set; }

    public string? ReferenceNumber { get; set; }

    public string? Notes { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
