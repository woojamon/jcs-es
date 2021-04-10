using System;

namespace Domain
{
    public class CannotAddProductsToPaidPurchaseOrder : IAddProductToPurchaseOrderResult
    {
        public CannotAddProductsToPaidPurchaseOrder(Guid purchaseOrderId)
        {
            PurchaseOrderId = purchaseOrderId;
        }
        public Guid PurchaseOrderId { get; }
    }
}