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
    public class RemoveProductFromPurchaseOrderResultPersistenceTests
    { 
        [Fact]
        public void Save_ResultIsProductRemovedFromPurchaseOrder_PersistsExpectedDocument()
        {
            /// Arrange
            // Get a product Removed to purchase order event.
            var @event = new ProductRemovedFromPurchaseOrder(
                purchaseOrderId: Guid.NewGuid(),
                productId: Guid.NewGuid());

            // Get some next id.
            var nextId = Guid.NewGuid();

            // Get the expected document dictionary.
            var expected = new BsonDocument()
                .Add(new BsonElement("_id", BsonString.Create(nextId.ToString())))
                .Add(new BsonElement("_type", BsonString.Create(nameof(ProductRemovedFromPurchaseOrder))))
                .Add(new BsonElement(nameof(ProductRemovedFromPurchaseOrder.PurchaseOrderId), new BsonString(@event.PurchaseOrderId.ToString())))
                .Add(new BsonElement(nameof(ProductRemovedFromPurchaseOrder.ProductId), new BsonString(@event.ProductId.ToString())));

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
        public void Save_ResultIsCannotRemoveProductsToPaidPurchaseOrder_PersistsExpectedDocument()
        {
            /// Arrange
            // Get an instance of the event.
            var @event = new CannotRemoveProductsFromPaidPurchaseOrder(
                purchaseOrderId: Guid.NewGuid());

            // Get some next id.
            var nextId = Guid.NewGuid();

            // Get the expected document dictionary.
            var expected = new BsonDocument()
                .Add(new BsonElement("_id", BsonString.Create(nextId.ToString())))
                .Add(new BsonElement("_type", BsonString.Create(nameof(CannotRemoveProductsFromPaidPurchaseOrder))))
                .Add(new BsonElement(nameof(CannotRemoveProductsFromPaidPurchaseOrder.PurchaseOrderId), new BsonString(@event.PurchaseOrderId.ToString())));

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
