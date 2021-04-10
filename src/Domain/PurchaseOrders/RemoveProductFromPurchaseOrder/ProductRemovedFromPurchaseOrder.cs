using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class ProductRemovedFromPurchaseOrder : IRemoveProductFromPurchaseOrderResult
    {
        public ProductRemovedFromPurchaseOrder(
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