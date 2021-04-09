using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class PurchaseOrderCreated
    {
        public PurchaseOrderCreated(
            Guid purchaseOrderId,
            PurchaseOrderStatus status,
            Guid vendorId,
            IEnumerable<PurchaseOrderLine> lines
        )
        {
            PurchaseOrderId = purchaseOrderId;
            Status = status;
            VendorId = vendorId;
            Lines = lines;
        }

        public Guid PurchaseOrderId { get; }
        public PurchaseOrderStatus Status { get; }
        public Guid VendorId { get; }
        public IEnumerable<PurchaseOrderLine> Lines { get; }
    }
}