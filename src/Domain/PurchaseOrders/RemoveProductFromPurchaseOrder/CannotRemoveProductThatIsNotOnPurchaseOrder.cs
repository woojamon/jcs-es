using System;

namespace Domain
{
    public class CannotRemoveProductThatIsNotOnPurchaseOrder : IRemoveProductFromPurchaseOrderResult
    {
        public CannotRemoveProductThatIsNotOnPurchaseOrder(
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