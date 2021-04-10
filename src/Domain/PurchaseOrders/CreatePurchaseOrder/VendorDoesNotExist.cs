using System;

namespace Domain
{
    public class VendorDoesNotExist : ICreatePurchaseOrderResult
    {
        public VendorDoesNotExist(Guid vendorId)
        {
            VendorId = vendorId;
        }
        public Guid VendorId { get; init; }
    }
}