using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Tests
{
    public class CreatePurchaseOrderTests
    {
        [Fact]
        public void CreatePurchaseOrder_VendorExists_ReturnsPurchaseOrderCreatedEvent()
        {
            /// Arrange
            // Get a create purchase order command with the vendor. 
            // (By the time the command gets to the domain it's already been structurally validated.)
            var command = new CreatePurchaseOrder(
                vendorId: Guid.NewGuid(),
                lines: new List<CreatePurchaseOrderLine> {
                    new CreatePurchaseOrderLine(productId: Guid.NewGuid(), description: "2x4x8 Lumber", quantity: 354M, measure: "EA", pricePerUnit: 4.75M),
                    new CreatePurchaseOrderLine(productId: Guid.NewGuid(), description: "500 Nails Pack", quantity: 10M, measure: "EA", pricePerUnit: 24.99M)
                }
            );

            // Assume the vendor exists.
            Func<Guid,bool> getVendorExists = _ => true;

            // Get some id as the next id.
            var nextId = Guid.NewGuid();
           
            // Get the event we expect the domain to return.
            var expected = new PurchaseOrderCreated(
                purchaseOrderId: nextId,
                status: PurchaseOrderStatus.Unpaid,
                vendorId: command.VendorId,
                lines: command.Lines.Select(l => 
                    new PurchaseOrderLine(l.ProductId, l.Quantity, l.Measure, l.PricePerUnit))
            );

            /// Act
            var actual = Domain.CreatePurchaseOrder(getVendorExists, nextId, command);

            /// Assert
            CustomAssert.CoreValuesAreEqual(expected, actual);
        }

        [Fact]
        public void CreatePurchaseOrder_VendorDoesNotExist_ReturnsExpectedError()
        {
            /// Arrange
            // Get a create purchase order command with the vendor. 
            // (By the time the command gets to the domain it's already been structurally validated.)
            var command = new CreatePurchaseOrder(
                vendorId: Guid.NewGuid(),
                lines: new List<CreatePurchaseOrderLine> {
                    new CreatePurchaseOrderLine(productId: Guid.NewGuid(), description: "2x4x8 Lumber", quantity: 354M, measure: "EA", pricePerUnit: 4.75M),
                    new CreatePurchaseOrderLine(productId: Guid.NewGuid(), description: "500 Nails Pack", quantity: 10M, measure: "EA", pricePerUnit: 24.99M)
                }
            );

            // Assume the vendor does not exist.
            Func<Guid,bool> getVendorExists = _ => false;

            // Get some id as the next id.
            var nextId = Guid.NewGuid();
           
            // Get the error we expect the domain to return.
            var expected = new VendorDoesNotExist(command.VendorId);

            /// Act
            var actual = Domain.CreatePurchaseOrder(getVendorExists, nextId, command);

            /// Assert
            CustomAssert.CoreValuesAreEqual(expected, actual);
        }
    }
}