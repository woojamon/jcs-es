using System;

namespace Domain
{
    public class CreatePurchaseOrderLine
    {
        public CreatePurchaseOrderLine(
            Guid productId,
            string description,
            decimal quantity,
            string measure,
            decimal pricePerUnit) 
        {
            ProductId = productId;
            Description = description;
            Quantity = quantity;
            Measure = measure;
            PricePerUnit = pricePerUnit;
        }

        public Guid ProductId { get; }
        public string Description { get; }
        public decimal Quantity { get; }
        public string Measure { get; }
        public decimal PricePerUnit { get; }
    }
}