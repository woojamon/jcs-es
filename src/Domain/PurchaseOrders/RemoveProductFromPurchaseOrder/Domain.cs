using System;
using System.Linq;

namespace Domain
{
    public static partial class JC
    {
        public static IRemoveProductFromPurchaseOrderResult RemoveProductFromPurchaseOrder(
            Func<Guid, PurchaseOrderForRemoveProductTask> getPurchaseOrder,
            RemoveProductFromPurchaseOrder command
        )
        {
            var purchaseOrder = getPurchaseOrder(command.PurchaseOrderId);
            if (purchaseOrder.Status == PurchaseOrderStatus.Paid)
                return new CannotRemoveProductsFromPaidPurchaseOrder(
                    purchaseOrderId: purchaseOrder.PurchaseOrderId);

            else if (!purchaseOrder.ProductIds.Contains(command.ProductId))
                return new CannotRemoveProductThatIsNotOnPurchaseOrder(
                    purchaseOrderId: purchaseOrder.PurchaseOrderId,
                    productId: command.ProductId);

            else
                return new ProductRemovedFromPurchaseOrder(
                    purchaseOrderId: purchaseOrder.PurchaseOrderId,
                    productId: command.ProductId);
        }
    }
}