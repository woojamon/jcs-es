
using System;
using System.Collections.Generic;

namespace Domain
{
    public class CreatePurchaseOrder
    {
        public CreatePurchaseOrder(
            Guid vendorId,
            IEnumerable<CreatePurchaseOrderLine> lines) 
        {
            VendorId = vendorId;
            Lines = lines;
        }

        public Guid VendorId { get; }
        public IEnumerable<CreatePurchaseOrderLine> Lines { get; }
    }
}