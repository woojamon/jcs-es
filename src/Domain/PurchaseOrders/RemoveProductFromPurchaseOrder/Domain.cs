using System;
using System.Linq;

namespace Domain
{
    public static partial class Domain
    {
        public static IRemoveProductFromPurchaseOrderResult RemoveProductFromPurchaseOrder(
            Func<Guid, PurchaseOrderForRemoveProductTask> getPurchaseOrder,
            RemoveProductFromPurchaseOrder command
        )
        {
            var purchaseOrder = getPurchaseOrder(command.PurchaseOrderId);
            if (purchaseOrder.Status == PurchaseOrderStatus.Paid) 
                return new CannotRemoveProductsFromPaidPurchaseOrder(purchaseOrder.PurchaseOrderId);
            else
                return new ProductRemovedFromPurchaseOrder(
                    purchaseOrderId: purchaseOrder.PurchaseOrderId,
                    productId: command.ProductId);
        }
    }
}