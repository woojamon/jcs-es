using System;
using System.Linq;

namespace Domain
{
    public static partial class JC
    {
        public static IAddProductToPurchaseOrderResult AddProductToPurchaseOrder(
            Func<Guid, PurchaseOrderForAddProductTask> getPurchaseOrder,
            AddProductToPurchaseOrder command)
        {
            var purchaseOrder = getPurchaseOrder(command.PurchaseOrderId);
            if (purchaseOrder.Status == PurchaseOrderStatus.Paid)
                return new CannotAddProductsToPaidPurchaseOrder(
                    purchaseOrderId: purchaseOrder.PurchaseOrderId);

            else if (purchaseOrder.ProductIds.Contains(command.ProductId))
                return new CannotAddDuplicateProductToPurchaseOrder(
                    purchaseOrderId: purchaseOrder.PurchaseOrderId,
                    productId: command.ProductId);

            else
                return new ProductAddedToPurchaseOrder(
                    purchaseOrderId: purchaseOrder.PurchaseOrderId,
                    productId: command.ProductId,
                    measure: command.Measure,
                    quantity: command.Quantity);
        }
    }
}