using System;
using System.Collections.Generic;
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
                .GetCollection<BsonDocument>("eventstream")
                .InsertOne(bson);
        }

        static BsonDocument Map(PurchaseOrderCreated @event)
        {
            return new BsonDocument()
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

        static BsonDocument Map(VendorDoesNotExist @event)
        {
            return new BsonDocument()
                .Add(new BsonElement(nameof(VendorDoesNotExist.VendorId), BsonString.Create(@event.VendorId.ToString())));
        }

        static BsonDocument Map(ProductAddedToPurchaseOrder @event)
        {
            return new BsonDocument()
                .Add(new BsonElement(nameof(ProductAddedToPurchaseOrder.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())))
                .Add(new BsonElement(nameof(ProductAddedToPurchaseOrder.ProductId), BsonString.Create(@event.ProductId.ToString())))
                .Add(nameof(ProductAddedToPurchaseOrder.Measure), BsonString.Create(@event.Measure))
                .Add(nameof(ProductAddedToPurchaseOrder.Quantity), BsonDecimal128.Create(@event.Quantity));
        }

        static BsonDocument Map(AddProductToPurchaseOrder @event)
        {
            return new BsonDocument()
                .Add(new BsonElement(nameof(AddProductToPurchaseOrder.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())))
                .Add(new BsonElement(nameof(AddProductToPurchaseOrder.ProductId), BsonString.Create(@event.ProductId.ToString())))
                .Add(nameof(AddProductToPurchaseOrder.Measure), BsonString.Create(@event.Measure))
                .Add(nameof(AddProductToPurchaseOrder.Quantity), BsonDecimal128.Create(@event.Quantity));
        }

        static BsonDocument Map(CannotAddProductsToPaidPurchaseOrder @event)
        {
            return new BsonDocument()
                .Add(new BsonElement(nameof(CannotAddProductsToPaidPurchaseOrder.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())));
        }

        static BsonDocument Map(ProductRemovedFromPurchaseOrder @event)
        {
            return new BsonDocument()
                .Add(new BsonElement(nameof(ProductRemovedFromPurchaseOrder.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())))
                .Add(new BsonElement(nameof(ProductRemovedFromPurchaseOrder.ProductId), BsonString.Create(@event.ProductId.ToString())));
        }

        static BsonDocument Map(RemoveProductFromPurchaseOrder @event)
        {
            return new BsonDocument()
                .Add(new BsonElement(nameof(RemoveProductFromPurchaseOrder.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())))
                .Add(new BsonElement(nameof(RemoveProductFromPurchaseOrder.ProductId), BsonString.Create(@event.ProductId.ToString())));
        }

        static BsonDocument Map(CannotRemoveProductsFromPaidPurchaseOrder @event)
        {
            return new BsonDocument()
                .Add(new BsonElement(nameof(CannotRemoveProductsFromPaidPurchaseOrder.PurchaseOrderId), BsonString.Create(@event.PurchaseOrderId.ToString())));
        }
    }
}
