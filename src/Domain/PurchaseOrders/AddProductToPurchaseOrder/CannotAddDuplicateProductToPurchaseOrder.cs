using System;

namespace Domain
{
    public class CannotAddDuplicateProductToPurchaseOrder : IAddProductToPurchaseOrderResult
    {
        public CannotAddDuplicateProductToPurchaseOrder(
            Guid purchaseOrderId,
            Guid productId)
        {
            PurchaseOrderId = purchaseOrderId;
            ProductId = productId;
        }
        public Guid PurchaseOrderId { get; }
        public Guid ProductId { get; }
    }
}