
using System;
using System.Collections.Generic;

namespace Domain
{
    public class RemoveProductFromPurchaseOrder
    {
        public RemoveProductFromPurchaseOrder(
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