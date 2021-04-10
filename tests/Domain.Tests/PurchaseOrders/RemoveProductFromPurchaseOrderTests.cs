using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Tests
{
    public class RemoveProductFromPurchaseOrderTests
    {
        [Fact]
        public void RemoveProductFromPurchaseOrder_POStatusIsPaid_ReturnsCannotRemoveProductsFromPaidPurchaseOrderResult()
        {
            /// Arrange
            // Get a command to remove a product from a purchase order.
            var command = new RemoveProductFromPurchaseOrder(
                purchaseOrderId: Guid.NewGuid(),
                productId: Guid.NewGuid()
            );

            // Get a purchase order aggregate to use  
            // when removing a product from a purchase order.
            var purchaseOrder = new PurchaseOrderForRemoveProductTask(
                purchaseOrderId: command.PurchaseOrderId,
                status: PurchaseOrderStatus.Paid,
                productIds: Enumerable.Empty<Guid>() // doesn't matter for this test
            );

            // Get a function that returns the purchase order aggregate
            // to use when removing a product from a purchase order. 
            Func<Guid, PurchaseOrderForRemoveProductTask> getPurchaseOrder = _ => purchaseOrder;

            // Get the event we expect the domain to return.
            var expected = new CannotRemoveProductsFromPaidPurchaseOrder(
                purchaseOrderId: command.PurchaseOrderId);

            /// Act
            var actual = Domain.RemoveProductFromPurchaseOrder(getPurchaseOrder, command);

            /// Assert
            CustomAssert.CoreValuesAreEqual(expected, actual);
        }

        [Fact]
        public void RemoveProductFromPurchaseOrder_PurchaseOrderStatusIsUnPaidAndPurchaseOrderHasProduct_ReturnsProductRemovedFromPurchaseOrderResult()
        {
            /// Arrange
            // Get a command to remove a product from a purchase order.
            var command = new RemoveProductFromPurchaseOrder(
                purchaseOrderId: Guid.NewGuid(),
                productId: Guid.NewGuid()
            );

            // Get a purchase order aggregate to use  
            // when removing a product from a purchase order,
            // and make sure that it has the product.
            var purchaseOrder = new PurchaseOrderForRemoveProductTask(
                purchaseOrderId: command.PurchaseOrderId,
                status: PurchaseOrderStatus.Unpaid,
                productIds: Enumerable.Empty<Guid>()
                    .Append(command.ProductId)
            );

            // Get a function that returns the purchase order aggregate
            // to use when removing a product from a purchase order. 
            Func<Guid, PurchaseOrderForRemoveProductTask> getPurchaseOrder = _ => purchaseOrder;

            // Get the event we expect the domain to return.
            var expected = new ProductRemovedFromPurchaseOrder(
                purchaseOrderId: command.PurchaseOrderId,
                productId: command.ProductId);

            /// Act
            var actual = Domain.RemoveProductFromPurchaseOrder(getPurchaseOrder, command);

            /// Assert
            CustomAssert.CoreValuesAreEqual(expected, actual);
        }
    }
}