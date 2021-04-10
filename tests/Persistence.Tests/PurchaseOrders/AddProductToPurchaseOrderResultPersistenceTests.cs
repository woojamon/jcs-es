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
    public class AddProductToPurchaseOrderResultPersistenceTests
    { 
        [Fact]
        public void Save_ResultIsProductAddedToPurchaseOrder_PersistsExpectedDocument()
        {
            /// Arrange
            // Get a product added to purchase order event.
            var @event = new ProductAddedToPurchaseOrder(
                purchaseOrderId: Guid.NewGuid(),
                productId: Guid.NewGuid(),
                measure: "EA",
                quantity: 14.25M);

            // Get some next id.
            var nextId = Guid.NewGuid();

            // Get the expected document dictionary.
            var expected = new BsonDocument()
                .Add(new BsonElement("_id", BsonString.Create(nextId.ToString())))
                .Add(new BsonElement("_type", BsonString.Create(nameof(ProductAddedToPurchaseOrder))))
                .Add(new BsonElement(nameof(ProductAddedToPurchaseOrder.PurchaseOrderId), new BsonString(@event.PurchaseOrderId.ToString())))
                .Add(new BsonElement(nameof(ProductAddedToPurchaseOrder.ProductId), new BsonString(@event.ProductId.ToString())))
                .Add(new BsonElement(nameof(ProductAddedToPurchaseOrder.Measure), BsonString.Create(@event.Measure)))
                .Add(new BsonElement(nameof(ProductAddedToPurchaseOrder.Quantity), BsonDecimal128.Create(@event.Quantity)));

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
        public void Save_ResultIsCannotAddProductsToPaidPurchaseOrder_PersistsExpectedDocument()
        {
            /// Arrange
            // Get an instance of the event.
            var @event = new CannotAddProductsToPaidPurchaseOrder(
                purchaseOrderId: Guid.NewGuid());

            // Get some next id.
            var nextId = Guid.NewGuid();

            // Get the expected document dictionary.
            var expected = new BsonDocument()
                .Add(new BsonElement("_id", BsonString.Create(nextId.ToString())))
                .Add(new BsonElement("_type", BsonString.Create(nameof(CannotAddProductsToPaidPurchaseOrder))))
                .Add(new BsonElement(nameof(CannotAddProductsToPaidPurchaseOrder.PurchaseOrderId), new BsonString(@event.PurchaseOrderId.ToString())));

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
