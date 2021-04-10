using System;

namespace Domain
{
    public class CreatePurchaseOrderLine
    {
        public CreatePurchaseOrderLine(
            Guid productId,
            decimal quantity,
            string measure,
            decimal pricePerUnit) 
        {
            ProductId = productId;
            Quantity = quantity;
            Measure = measure;
            PricePerUnit = pricePerUnit;
        }

        public Guid ProductId { get; }
        public decimal Quantity { get; }
        public string Measure { get; }
        public decimal PricePerUnit { get; }
    }
}