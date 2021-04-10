using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using Persistence;
using Telerik.JustMock;
using Xunit;

namespace Persistence.Tests.PurchaseOrders
{
    public class CreatePurchaseOrderResultPersistenceTests
    {
        [Theory]
        [InlineData(PurchaseOrderStatus.Paid)]
        [InlineData(PurchaseOrderStatus.Unpaid)]
        [InlineData(PurchaseOrderStatus.Canceled)]
        public void Save_ResultIsPurchaseOrderCreated_PersistsExpectedDocument(PurchaseOrderStatus status)
        {
            /// Arrange
            // Get a purchase order created event.
            var @event = new PurchaseOrderCreated(
                purchaseOrderId: Guid.NewGuid(),
                status: status,
                vendorId: Guid.NewGuid(),
                lines: Enumerable.Empty<PurchaseOrderLine>()
                    .Append(new PurchaseOrderLine(
                        productId: Guid.NewGuid(),
                        quantity: 14.25M,
                        measure: "EA",
                        pricePerUnit: 2.24M))
                    .Append(new PurchaseOrderLine(
                        productId: Guid.NewGuid(),
                        quantity: 5.5M,
                        measure: "FT",
                        pricePerUnit: 0.75M)));

            // Get some next id.
            var nextId = Guid.NewGuid();

            // Get the expected document dictionary.
            var expected = new BsonDocument()
                .Add(new BsonElement("_id", BsonString.Create(nextId.ToString())))
                .Add(new BsonElement("_type", BsonString.Create(nameof(PurchaseOrderCreated))))
                .Add(new BsonElement(nameof(PurchaseOrderCreated.PurchaseOrderId), new BsonString(@event.PurchaseOrderId.ToString())))
                .Add(new BsonElement(nameof(PurchaseOrderCreated.Status), new BsonString(Enum.GetName<PurchaseOrderStatus>(@event.Status))))
                .Add(new BsonElement(nameof(PurchaseOrderCreated.VendorId), new BsonString(@event.VendorId.ToString())))
                .Add(new BsonElement(nameof(PurchaseOrderCreated.Lines), new BsonArray()
                    .Add(new BsonDocument()
                        .Add(new BsonElement(nameof(PurchaseOrderLine.ProductId), BsonString.Create(@event.Lines.ElementAt(0).ProductId.ToString())))
                        .Add(new BsonElement(nameof(PurchaseOrderLine.Quantity), BsonDecimal128.Create(@event.Lines.ElementAt(0).Quantity)))
                        .Add(new BsonElement(nameof(PurchaseOrderLine.Measure), BsonString.Create(@event.Lines.ElementAt(0).Measure)))
                        .Add(new BsonElement(nameof(PurchaseOrderLine.PricePerUnit), BsonDecimal128.Create(@event.Lines.ElementAt(0).PricePerUnit))))
                    .Add(new BsonDocument()
                       .Add(new BsonElement(nameof(PurchaseOrderLine.ProductId), BsonString.Create(@event.Lines.ElementAt(1).ProductId.ToString())))
                       .Add(new BsonElement(nameof(PurchaseOrderLine.Quantity), BsonDecimal128.Create(@event.Lines.ElementAt(1).Quantity)))
                       .Add(new BsonElement(nameof(PurchaseOrderLine.Measure), BsonString.Create(@event.Lines.ElementAt(1).Measure)))
                       .Add(new BsonElement(nameof(PurchaseOrderLine.PricePerUnit), BsonDecimal128.Create(@event.Lines.ElementAt(1).PricePerUnit))))));

            // Mock up a mongo client for the mongo db event store.
            BsonDocument actual = null;
            var collection = Mock.Create<IMongoCollection<BsonDocument>>();
            Mock.Arrange(() => collection.InsertOne(Arg.IsAny<BsonDocument>(), Arg.IsAny<InsertOneOptions>(), Arg.IsAny<CancellationToken>()))
                .DoInstead((BsonDocument document) => actual = document);
            var database = Mock.Create<IMongoDatabase>();
            Mock.Arrange(() => database.GetCollection<BsonDocument>(Arg.AnyString, Arg.IsAny<MongoCollectionSettings>()))
                .Returns(collection);
            var client = Mock.Create<IMongoClient>();
            Mock.Arrange(() => client.GetDatabase(Arg.AnyString, Arg.IsAny<MongoDatabaseSettings>()))
                .Returns(database);

            /// Act
            // Save the event.
            MongoDBEventStore.Save(client, nextId, @event);

            /// Assert
            // Verify that the actual document matches the expected document.
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Save_ResultIsVendorDoesNotExist_PersistsExpectedDocument()
        {
            /// Arrange
            // Get a purchase order created event.
            var @event = new VendorDoesNotExist(
                vendorId: Guid.NewGuid());

            // Get some next id.
            var nextId = Guid.NewGuid();

            // Get the expected document dictionary.
            var expected = new BsonDocument()
                .Add(new BsonElement("_id", BsonString.Create(nextId.ToString())))
                .Add(new BsonElement("_type", BsonString.Create(nameof(VendorDoesNotExist))))
                .Add(new BsonElement(nameof(VendorDoesNotExist.VendorId), new BsonString(@event.VendorId.ToString())));

            // Mock up a mongo client for the mongo db event store.
            BsonDocument actual = null;
            var collection = Mock.Create<IMongoCollection<BsonDocument>>();
            Mock.Arrange(() => collection.InsertOne(Arg.IsAny<BsonDocument>(), Arg.IsAny<InsertOneOptions>(), Arg.IsAny<CancellationToken>()))
                .DoInstead((BsonDocument document) => actual = document);
            var database = Mock.Create<IMongoDatabase>();
            Mock.Arrange(() => database.GetCollection<BsonDocument>(Arg.AnyString, Arg.IsAny<MongoCollectionSettings>()))
                .Returns(collection);
            var client = Mock.Create<IMongoClient>();
            Mock.Arrange(() => client.GetDatabase(Arg.AnyString, Arg.IsAny<MongoDatabaseSettings>()))
                .Returns(database);

            /// Act
            // Save the event.
            MongoDBEventStore.Save(client, nextId, @event);

            /// Assert
            // Verify that the actual document matches the expected document.
            Assert.Equal(expected, actual);
        }
    }
}
