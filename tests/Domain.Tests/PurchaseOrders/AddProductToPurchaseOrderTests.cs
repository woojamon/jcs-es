using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Tests
{
    public class AddProductToPurchaseOrderTests
    {
        [Fact]
        public void AddProductToPurchaseOrder_POStatusIsPaid_ReturnsCannotAddProductsToPaidPurchaseOrderResult()
        {
            /// Arrange
            // Get a command to Add a product To a purchase order.
            var command = new AddProductToPurchaseOrder(
                purchaseOrderId: Guid.NewGuid(),
                productId: Guid.NewGuid(),
                measure: "EA",
                quantity: 14.5M
            );

            // Get a purchase order aggregate to use  
            // when removing a product To a purchase order.
            var purchaseOrder = new PurchaseOrderForAddProductTask(
                purchaseOrderId: command.PurchaseOrderId,
                status: PurchaseOrderStatus.Paid,
                productIds: Enumerable.Empty<Guid>() // doesn't matter for this test
            );

            // Get a function that returns the purchase order aggregate
            // to use when removing a product To a purchase order. 
            Func<Guid, PurchaseOrderForAddProductTask> getPurchaseOrder = _ => purchaseOrder;

            // Get the event we expect the domain to return.
            var expected = new CannotAddProductsToPaidPurchaseOrder(
                purchaseOrderId: command.PurchaseOrderId);

            /// Act
            var actual = Domain.AddProductToPurchaseOrder(getPurchaseOrder, command);

            /// Assert
            CustomAssert.CoreValuesAreEqual(expected, actual);
        }

        [Fact]
        public void AddProductToPurchaseOrder_PurchaseOrderStatusIsUnPaidAndPurchaseOrderHasProduct_ReturnsProductAddedToPurchaseOrderResult()
        {
            /// Arrange
            // Get a command to Add a product To a purchase order.
            var command = new AddProductToPurchaseOrder(
                purchaseOrderId: Guid.NewGuid(),
                productId: Guid.NewGuid(),
                measure: "EA",
                quantity: 3.56M
            );

            // Get a purchase order aggregate to use  
            // when removing a product To a purchase order,
            // and make sure that it has the product.
            var purchaseOrder = new PurchaseOrderForAddProductTask(
                purchaseOrderId: command.PurchaseOrderId,
                status: PurchaseOrderStatus.Unpaid,
                productIds: Enumerable.Empty<Guid>()
                    .Append(command.ProductId)
            );

            // Get a function that returns the purchase order aggregate
            // to use when removing a product To a purchase order. 
            Func<Guid, PurchaseOrderForAddProductTask> getPurchaseOrder = _ => purchaseOrder;

            // Get the event we expect the domain to return.
            var expected = new ProductAddedToPurchaseOrder(
                purchaseOrderId: command.PurchaseOrderId,
                productId: command.ProductId,
                measure: command.Measure,
                quantity: command.Quantity);

            /// Act
            var actual = Domain.AddProductToPurchaseOrder(getPurchaseOrder, command);

            /// Assert
            CustomAssert.CoreValuesAreEqual(expected, actual);
        }
    }
}