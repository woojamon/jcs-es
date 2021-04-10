
using System;
using System.Collections.Generic;

namespace Domain
{
    public class PurchaseOrderForAddProductTask
    {
        public PurchaseOrderForAddProductTask(
            Guid purchaseOrderId,
            PurchaseOrderStatus status,
            IEnumerable<Guid> productIds)
        {
            PurchaseOrderId = purchaseOrderId;
            Status = status;
            ProductIds = productIds;
        }

        public Guid PurchaseOrderId { get; }
        public PurchaseOrderStatus Status { get; }
        public IEnumerable<Guid> ProductIds { get; }
    }
}