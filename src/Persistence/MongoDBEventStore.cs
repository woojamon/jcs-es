using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using Domain;
using Domain.PurchaseOrders.ReadModels;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Persistence
{
    public class MongoDBEventStore
    {
        public MongoDBEventStore()
        {

        }

        public static void Save(IMongoClient client, Guid nextId, object @event)
        {
            var json = @event switch
            {
                PurchaseOrderCreated e => Event.Serialize(e, nextId),//Map(e),
                VendorDoesNotExist e => Event.Serialize(e, nextId),
                ProductAddedToPurchaseOrder e => Event.Serialize(e, nextId),
                CannotAddProductsToPaidPurchaseOrder e => Event.Serialize(e, nextId),
                ProductRemovedFromPurchaseOrder e => Event.Serialize(e, nextId),
                CannotRemoveProductsFromPaidPurchaseOrder e => Event.Serialize(e, nextId),
                _ => throw new NotImplementedException()
            };

            using (var reader = new JsonReader(json))
            {
                var context = BsonDeserializationContext.CreateRoot(reader);
                BsonDocument doc = BsonDocumentSerializer.Instance.Deserialize(context);

                client
                    .GetDatabase("jcs")
                    .GetCollection<BsonDocument>("events")
                    .InsertOne(doc);
            }
        }

        public class Event<A>
        {
            public string _id { get; set; }
            public string _type { get; set; } = typeof(A).Name;
            public A _event { get; set; }
        }
        public static class Event
        {
            public static string Serialize<A>(A a, Guid id) =>
                 JsonSerializer.Serialize(new Event<A>
                 {
                     _id = id.ToString(),
                     _type = typeof(A).Name,
                     _event = a
                 });
        }

        public static PurchaseOrderForAddProductTask GetPurchaseOrderForAddProductTask(IMongoClient client, Guid purchaseOrderId)
        {
            return client
                .GetDatabase("jcs")
                .GetCollection<BsonDocument>("events")
                .Find(new FilterDefinitionBuilder<BsonDocument>()
                    .And(new FilterDefinitionBuilder<BsonDocument>()
                        .In<string>(
                            field: new StringFieldDefinition<BsonDocument, string>("_type"),
                            values: new[] {
                                nameof(PurchaseOrderCreated),
                                nameof(ProductAddedToPurchaseOrder),
                                nameof(ProductRemovedFromPurchaseOrder)}),
                        new FilterDefinitionBuilder<BsonDocument>()
                        .Eq<string>(
                            field: new StringFieldDefinition<BsonDocument, string>("_event.PurchaseOrderId"),
                            value: purchaseOrderId.ToString())))
                .ToList()
                .Aggregate(
                    seed: null,
                    func: (PurchaseOrderForAddProductTask po, BsonDocument doc) =>
                        doc.GetValue("_type").ToString() switch
                        {
                            nameof(PurchaseOrderCreated) => MapA(JsonSerializer.Deserialize<PurchaseOrderCreated>(doc.GetValue("_event").ToJson())),
                            nameof(ProductAddedToPurchaseOrder) => MapA(po, JsonSerializer.Deserialize<ProductAddedToPurchaseOrder>(doc.GetValue("_event").ToJson())),
                            nameof(ProductRemovedFromPurchaseOrder) => MapA(po, JsonSerializer.Deserialize<ProductRemovedFromPurchaseOrder>(doc.GetValue("_event").ToJson())),
                            _ => throw new NotImplementedException()
                        });
        }

        static PurchaseOrderForAddProductTask MapA(PurchaseOrderCreated e) =>
            new PurchaseOrderForAddProductTask(
                purchaseOrderId: e.PurchaseOrderId,
                status: e.Status,
                productIds: e.Lines.Select(l => l.ProductId).ToList());

        static PurchaseOrderForAddProductTask MapA(PurchaseOrderForAddProductTask o, ProductAddedToPurchaseOrder e) =>
            new PurchaseOrderForAddProductTask(
                purchaseOrderId: o.PurchaseOrderId,
                status: o.Status,
                productIds: o.ProductIds.Union(new[] { e.ProductId }).ToList());

        static PurchaseOrderForAddProductTask MapA(PurchaseOrderForAddProductTask o, ProductRemovedFromPurchaseOrder e) =>
            new PurchaseOrderForAddProductTask(
                purchaseOrderId: o.PurchaseOrderId,
                status: o.Status,
                productIds: o.ProductIds.Except(new[] { e.ProductId }).ToList());

        static PurchaseOrderForRemoveProductTask MapR(PurchaseOrderCreated e) =>
            new PurchaseOrderForRemoveProductTask(
                purchaseOrderId: e.PurchaseOrderId,
                status: e.Status,
                productIds: e.Lines.Select(l => l.ProductId).ToList());

        static PurchaseOrderForRemoveProductTask MapR(PurchaseOrderForRemoveProductTask o, ProductAddedToPurchaseOrder e) =>
            new PurchaseOrderForRemoveProductTask(
                purchaseOrderId: o.PurchaseOrderId,
                status: o.Status,
                productIds: o.ProductIds.Union(new[] { e.ProductId }).ToList());

        static PurchaseOrderForRemoveProductTask MapR(PurchaseOrderForRemoveProductTask o, ProductRemovedFromPurchaseOrder e) =>
            new PurchaseOrderForRemoveProductTask(
                purchaseOrderId: o.PurchaseOrderId,
                status: o.Status,
                productIds: o.ProductIds.Except(new[] { e.ProductId }).ToList());

        public static PurchaseOrderForRemoveProductTask GetPurchaseOrderForRemoveProductTask(IMongoClient client, Guid purchaseOrderId)
        {
            return client
                .GetDatabase("jcs")
                .GetCollection<BsonDocument>("events")
                .Find(new FilterDefinitionBuilder<BsonDocument>()
                    .And(new FilterDefinitionBuilder<BsonDocument>()
                        .In<string>(
                            field: new StringFieldDefinition<BsonDocument, string>("_type"),
                            values: new[] {
                                nameof(PurchaseOrderCreated),
                                nameof(ProductAddedToPurchaseOrder),
                                nameof(ProductRemovedFromPurchaseOrder)}),
                        new FilterDefinitionBuilder<BsonDocument>()
                        .Eq<string>(
                            field: new StringFieldDefinition<BsonDocument, string>("_event.PurchaseOrderId"),
                            value: purchaseOrderId.ToString())))
                .ToList()
                .Aggregate(
                    seed: null,
                    func: (PurchaseOrderForRemoveProductTask po, BsonDocument doc) =>
                        doc.GetValue("_type").ToString() switch
                        {
                            nameof(PurchaseOrderCreated) => MapR(JsonSerializer.Deserialize<PurchaseOrderCreated>(doc.GetValue("_event").ToJson())),
                            nameof(ProductAddedToPurchaseOrder) => MapR(po, JsonSerializer.Deserialize<ProductAddedToPurchaseOrder>(doc.GetValue("_event").ToJson())),
                            nameof(ProductRemovedFromPurchaseOrder) => MapR(po, JsonSerializer.Deserialize<ProductRemovedFromPurchaseOrder>(doc.GetValue("_event").ToJson())),
                            _ => throw new NotImplementedException()
                        });
        }
    }
}
