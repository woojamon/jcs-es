using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Domain;
using MongoDB.Bson;
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
            var bson = (@event switch
            {
                PurchaseOrderCreated e => Map(e),
                VendorDoesNotExist e => Map(e),
                ProductAddedToPurchaseOrder e => Map(e),
                CannotAddProductsToPaidPurchaseOrder e => Map(e),
                ProductRemovedFromPurchaseOrder e => Map(e),
                CannotRemoveProductsFromPaidPurchaseOrder e => Map(e),
                _ => throw new NotImplementedException()
            });

            bson.InsertAt(0, new BsonElement("_id", BsonString.Create(nextId.ToString())));

            client
                .GetDatabase("jcs")
                .GetCollection<BsonDocument>("events")
                .InsertOne(bson);
        }

        static BsonDocument Map(PurchaseOrderCreated @event)
        {
            return new BsonDocument()
                .Add(new BsonElement("_type", BsonString.Create(nameof(PurchaseOrderCreated))))
                .Add(new BsonElement(nameof(PurchaseOrderCreated.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())))
                .Add(new BsonElement(nameof(PurchaseOrderCreated.Status), BsonString.Create(Enum.GetName<PurchaseOrderStatus>(@event.Status))))
                .Add(new BsonElement(nameof(PurchaseOrderCreated.VendorId), BsonString.Create(@event.VendorId.ToString())))
                .Add(new BsonElement(nameof(PurchaseOrderCreated.Lines), @event.Lines.Aggregate(
                    seed: new BsonArray(),
                    func: (lines, line) => lines.Add(
                        new BsonDocument()
                            .Add(nameof(PurchaseOrderLine.ProductId), BsonString.Create(line.ProductId.ToString()))
                            .Add(nameof(PurchaseOrderLine.Quantity), BsonDecimal128.Create(line.Quantity))
                            .Add(nameof(PurchaseOrderLine.Measure), BsonString.Create(line.Measure))
                            .Add(nameof(PurchaseOrderLine.PricePerUnit), BsonDecimal128.Create(line.PricePerUnit))))));
        }

        public static PurchaseOrderForAddProductTask GetPurchaseOrderForAddProductTask(IMongoClient client, Guid purchaseOrderId)
        {
            // The query is basically
            // select *
            // from jcs.events
            // where _type in ('PurchaseOrderCreated','ProductAddedToPurchaseOrder','ProductRemovedFromPurchaseOrder')
            // and _event.PurchaseOrderId = purchaseOrderId
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
                            field: new StringFieldDefinition<BsonDocument, string>("PurchaseOrderId"),
                            value: purchaseOrderId.ToString())))
                .ToList()

                // This is the code that rehydrates the aggregate.
                .Aggregate(
                    seed: null,
                    func: (PurchaseOrderForAddProductTask acc, BsonDocument doc) =>
                        doc.GetValue("_type").ToString() switch
                        {
                            // If the event is PurchaseOrderCreated then just grab the desired information
                            nameof(PurchaseOrderCreated) =>
                                new PurchaseOrderForAddProductTask(
                                    purchaseOrderId: new Guid(doc.GetValue(nameof(PurchaseOrderCreated.PurchaseOrderId)).ToString()),
                                    status: Enum.Parse<PurchaseOrderStatus>(doc.GetValue(nameof(PurchaseOrderCreated.Status)).ToString()),
                                    productIds: doc.GetValue(nameof(PurchaseOrderCreated.Lines)).AsBsonArray.Select(line =>
                                        new Guid(line.AsBsonDocument.GetValue(nameof(PurchaseOrderLine.ProductId)).ToString())).ToList()),

                            // If the event is ProductAddedToPurchaseOrder then just add the ProductId to the collection
                            nameof(ProductAddedToPurchaseOrder) =>
                                new PurchaseOrderForAddProductTask(
                                    purchaseOrderId: acc.PurchaseOrderId,
                                    status: acc.Status,
                                    productIds: acc.ProductIds.Union(new[] { new Guid(doc.GetValue(nameof(ProductAddedToPurchaseOrder.ProductId)).ToString()) }).ToList()),

                            // If the event is ProductRemovedFromPurchaseOrder then just remove the ProductId to the collection
                            nameof(ProductRemovedFromPurchaseOrder) => new PurchaseOrderForAddProductTask(
                                purchaseOrderId: acc.PurchaseOrderId,
                                status: acc.Status,
                                productIds: acc.ProductIds.Except(new[] { new Guid(doc.GetValue(nameof(ProductAddedToPurchaseOrder.ProductId)).ToString()) }).ToList()),
                            _ => throw new NotImplementedException()
                        });  
        }

        public static PurchaseOrderForRemoveProductTask GetPurchaseOrderForRemoveProductTask(IMongoClient client, Guid purchaseOrderId)
        {
            // The query is basically
            // select *
            // from jcs.events
            // where _type in ('PurchaseOrderCreated','ProductAddedToPurchaseOrder','ProductRemovedFromPurchaseOrder')
            // and _event.PurchaseOrderId = purchaseOrderId
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
                            field: new StringFieldDefinition<BsonDocument, string>("PurchaseOrderId"),
                            value: purchaseOrderId.ToString())))
                .ToList()

                // This is the code that rehydrates the aggregate.
                .Aggregate(
                    seed: null,
                    func: (PurchaseOrderForRemoveProductTask acc, BsonDocument doc) =>
                        doc.GetValue("_type").ToString() switch
                        {
                            // If the event is PurchaseOrderCreated then just grab the desired information
                            nameof(PurchaseOrderCreated) =>
                                new PurchaseOrderForRemoveProductTask(
                                    purchaseOrderId: new Guid(doc.GetValue(nameof(PurchaseOrderCreated.PurchaseOrderId)).ToString()),
                                    status: Enum.Parse<PurchaseOrderStatus>(doc.GetValue(nameof(PurchaseOrderCreated.Status)).ToString()),
                                    productIds: doc.GetValue(nameof(PurchaseOrderCreated.Lines)).AsBsonArray.Select(line =>
                                        new Guid(line.AsBsonDocument.GetValue(nameof(PurchaseOrderLine.ProductId)).ToString())).ToList()),


                            // If the event is ProductAddedToPurchaseOrder then just add the ProductId to the collection
                            nameof(ProductAddedToPurchaseOrder) =>
                                new PurchaseOrderForRemoveProductTask(
                                    purchaseOrderId: acc.PurchaseOrderId,
                                    status: acc.Status,
                                    productIds: acc.ProductIds.Union(new[] { new Guid(doc.GetValue(nameof(ProductAddedToPurchaseOrder.ProductId)).ToString()) }).ToList()),

                            // If the event is ProductRemovedFromPurchaseOrder then just remove the ProductId to the collection
                            nameof(ProductRemovedFromPurchaseOrder) =>
                                new PurchaseOrderForRemoveProductTask(
                                    purchaseOrderId: acc.PurchaseOrderId,
                                    status: acc.Status,
                                    productIds: acc.ProductIds.Except(new[] { new Guid(doc.GetValue(nameof(ProductAddedToPurchaseOrder.ProductId)).ToString()) }).ToList()),
                            _ => throw new NotImplementedException()
                        });
        }

        static BsonDocument Map(VendorDoesNotExist @event)
        {
            return new BsonDocument()
                .Add(new BsonElement("_type", BsonString.Create(nameof(VendorDoesNotExist))))
                .Add(new BsonElement(nameof(VendorDoesNotExist.VendorId), BsonString.Create(@event.VendorId.ToString())));
        }

        static BsonDocument Map(ProductAddedToPurchaseOrder @event)
        {
            return new BsonDocument()
                .Add(new BsonElement("_type", BsonString.Create(nameof(ProductAddedToPurchaseOrder))))
                .Add(new BsonElement(nameof(ProductAddedToPurchaseOrder.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())))
                .Add(new BsonElement(nameof(ProductAddedToPurchaseOrder.ProductId), BsonString.Create(@event.ProductId.ToString())))
                .Add(nameof(ProductAddedToPurchaseOrder.Measure), BsonString.Create(@event.Measure))
                .Add(nameof(ProductAddedToPurchaseOrder.Quantity), BsonDecimal128.Create(@event.Quantity));
        }

        static BsonDocument Map(AddProductToPurchaseOrder @event)
        {
            return new BsonDocument()
                .Add(new BsonElement("_type", BsonString.Create(nameof(AddProductToPurchaseOrder))))
                .Add(new BsonElement(nameof(AddProductToPurchaseOrder.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())))
                .Add(new BsonElement(nameof(AddProductToPurchaseOrder.ProductId), BsonString.Create(@event.ProductId.ToString())))
                .Add(nameof(AddProductToPurchaseOrder.Measure), BsonString.Create(@event.Measure))
                .Add(nameof(AddProductToPurchaseOrder.Quantity), BsonDecimal128.Create(@event.Quantity));
        }

        static BsonDocument Map(CannotAddProductsToPaidPurchaseOrder @event)
        {
            return new BsonDocument()
                .Add(new BsonElement("_type", BsonString.Create(nameof(CannotAddProductsToPaidPurchaseOrder))))
                .Add(new BsonElement(nameof(CannotAddProductsToPaidPurchaseOrder.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())));
        }

        static BsonDocument Map(ProductRemovedFromPurchaseOrder @event)
        {
            return new BsonDocument()
                .Add(new BsonElement("_type", BsonString.Create(nameof(ProductRemovedFromPurchaseOrder))))
                .Add(new BsonElement(nameof(ProductRemovedFromPurchaseOrder.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())))
                .Add(new BsonElement(nameof(ProductRemovedFromPurchaseOrder.ProductId), BsonString.Create(@event.ProductId.ToString())));
        }

        static BsonDocument Map(RemoveProductFromPurchaseOrder @event)
        {
            return new BsonDocument()
                .Add(new BsonElement("_type", BsonString.Create(nameof(RemoveProductFromPurchaseOrder))))
                .Add(new BsonElement(nameof(RemoveProductFromPurchaseOrder.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())))
                .Add(new BsonElement(nameof(RemoveProductFromPurchaseOrder.ProductId), BsonString.Create(@event.ProductId.ToString())));
        }

        static BsonDocument Map(CannotRemoveProductsFromPaidPurchaseOrder @event)
        {
            return new BsonDocument()
                .Add(new BsonElement("_type", BsonString.Create(nameof(CannotRemoveProductsFromPaidPurchaseOrder))))
                .Add(new BsonElement(nameof(CannotRemoveProductsFromPaidPurchaseOrder.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())));
        }
    }
}
