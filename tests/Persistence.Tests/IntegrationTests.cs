using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Persistence.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void InsertOne_BsonDocument_ExpectedDocumentInserted()
        {
            /// Arrange
            // Get an id for the document.
            var id = Guid.NewGuid().ToString();

            // Get some expected bson document with the id.
            var expected = new BsonDocument()
                .Add(new BsonElement("_id", BsonString.Create(id)))
                .Add(new BsonElement("hello", BsonString.Create("world")));

            // Get a collection.
            var collection = new MongoClient("mongodb://localhost:27017")
            .GetDatabase("jcs")
            .GetCollection<BsonDocument>("events");

            /// Act
            // Insert the document into the collection.
            collection.InsertOne(expected);

            // Get the document back.
            var actual = collection.Find(
                new FilterDefinitionBuilder<BsonDocument>().Eq<string>(
                    new StringFieldDefinition<BsonDocument, string>("_id"), id))
                .Single();

            /// Assert
            // Verify that the actual document matches the expected document.
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void loadtest()
        {
            // Get a MongoClient.
            var client = new MongoClient("mongodb://localhost:27017");

            Func<Guid, bool> getVendorExists = _ => true;
            Func<Guid, PurchaseOrderForAddProductTask> getPurchaseOrderForAddProductTask = purchaseOrderId => MongoDBEventStore.GetPurchaseOrderForAddProductTask(client, purchaseOrderId);
            Func<Guid, PurchaseOrderForRemoveProductTask> getPurchaseOrderForRemoveProductTask = purchaseOrderId => MongoDBEventStore.GetPurchaseOrderForRemoveProductTask(client, purchaseOrderId);
            Func<Guid> nextId = Guid.NewGuid;

            var create = new CreatePurchaseOrder(
                vendorId: nextId(),
                lines: new List<CreatePurchaseOrderLine>
                {
                    new CreatePurchaseOrderLine(
                        productId: nextId(),
                        quantity: 14.5M,
                        measure: "FT",
                        pricePerUnit: 2.8M),
                    new CreatePurchaseOrderLine(
                        productId: nextId(),
                        quantity: 1.5M,
                        measure: "YD",
                        pricePerUnit: 235.82M)
                 });


            Guid purchaseOrderId = nextId();

            var createResult = JC.CreatePurchaseOrder(
                getVendorExists: getVendorExists,
                nextId: purchaseOrderId,
                command: create);

            MongoDBEventStore.Save(client, nextId(), createResult);

            var expected = getPurchaseOrderForAddProductTask(purchaseOrderId);

            var productId = Guid.NewGuid();

            // Put 10000 events in the store for this purchase order.
            for (int i = 0; i < 5000; i++)
            {
                var addResult = new ProductAddedToPurchaseOrder(
                        purchaseOrderId: purchaseOrderId,
                        productId: productId,
                        measure: "EA",
                        quantity: i);

                MongoDBEventStore.Save(client, nextId(), addResult);

                var removeResult = new ProductRemovedFromPurchaseOrder(
                        purchaseOrderId: purchaseOrderId,
                        productId: productId);

                MongoDBEventStore.Save(client, nextId(), removeResult);
            }
                
            // Now for the time trial
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var actual = getPurchaseOrderForAddProductTask(purchaseOrderId);
            stopwatch.Stop();

            CustomAssert.CoreValuesAreEqual(expected, actual);
            Assert.Equal(0, stopwatch.ElapsedMilliseconds);
        }
    }

    static class CustomAssert
    {
        public static void CoreValuesAreEqual(PurchaseOrderForAddProductTask expected, PurchaseOrderForAddProductTask actual)
        {
            Assert.Equal(expected.PurchaseOrderId, actual.PurchaseOrderId);
            Assert.Equal(expected.Status, actual.Status);
            Assert.Equal(expected.ProductIds.Count(), actual.ProductIds.Count());
            for (int i = 0; i < expected.ProductIds.Count(); i++)
                Assert.Equal(expected.ProductIds.ElementAt(i), actual.ProductIds.ElementAt(i));
        }

    }
}
