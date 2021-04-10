using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.PurchaseOrders.ReadModels
{
    public class PurchaseOrder
    {
        public PurchaseOrder(
            Guid purchaseOrderId,
            Guid vendorId,
            IEnumerable<PurchaseOrderLine> lines)
        {
            PurchaseOrderId = purchaseOrderId;
            VendorId = vendorId;
            Lines = lines;
            Total = lines.Sum(line => line.Quantity * line.PricePerUnit);
        }

        public Guid PurchaseOrderId { get; }
        public Guid VendorId { get; }
        public IEnumerable<PurchaseOrderLine> Lines { get; }
        public decimal Total { get; }
    }
}
