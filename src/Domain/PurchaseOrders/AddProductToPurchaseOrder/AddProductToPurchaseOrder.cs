
using System;
using System.Collections.Generic;

namespace Domain
{
    public class AddProductToPurchaseOrder
    {
        public AddProductToPurchaseOrder(
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