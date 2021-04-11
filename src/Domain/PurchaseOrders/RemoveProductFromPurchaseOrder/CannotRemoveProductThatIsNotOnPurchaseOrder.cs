using System;

namespace Domain
{
    public class CannotRemoveProductsFromPaidPurchaseOrder : IRemoveProductFromPurchaseOrderResult
    {
        public CannotRemoveProductsFromPaidPurchaseOrder(Guid purchaseOrderId)
        {
            PurchaseOrderId = purchaseOrderId;
        }
        public Guid PurchaseOrderId { get; }
    }
}