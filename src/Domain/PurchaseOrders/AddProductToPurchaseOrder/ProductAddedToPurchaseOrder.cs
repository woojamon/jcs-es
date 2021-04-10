using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class ProductAddedToPurchaseOrder : IAddProductToPurchaseOrderResult
    {
        public ProductAddedToPurchaseOrder(
            Guid purchaseOrderId,
            Guid productId,
            string measure,
            decimal quantity)
        {
            PurchaseOrderId = purchaseOrderId;
            ProductId = productId;
            Measure = measure;
            Quantity = quantity;
        }

        public Guid PurchaseOrderId { get; }
        public Guid ProductId { get; }
        public string Measure { get; }
        public decimal Quantity { get; }
    }
}