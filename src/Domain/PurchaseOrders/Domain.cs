using System;
using System.Linq;

namespace Domain
{
    public static partial class Domain
    {
        public static PurchaseOrderCreated CreatePurchaseOrder(
            Func<Guid,bool> getVendorExists,
            Guid nextId,
            CreatePurchaseOrder command
        )
        {
            return new PurchaseOrderCreated(
                purchaseOrderId: nextId,
                status: PurchaseOrderStatus.Unpaid,
                vendorId: command.VendorId,
                lines: command.Lines.Select(l => 
                    new PurchaseOrderLine(l.ProductId, l.Description, l.Quantity, l.Measure, l.PricePerUnit)));
        }
    }
}