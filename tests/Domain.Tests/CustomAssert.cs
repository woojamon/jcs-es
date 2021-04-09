using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Domain.Tests
{
    static class CustomAssert
    {
        public static void CoreValuesAreEqual(ICreatePurchaseOrderResult expected, ICreatePurchaseOrderResult actual)
        {
            if (expected is PurchaseOrderCreated e1 && actual is PurchaseOrderCreated a1)
                CoreValuesAreEqual(e1,a1);
            else if (expected is VendorDoesNotExist e2 && actual is VendorDoesNotExist a2)
                CoreValuesAreEqual(e2,a2);
            else 
                throw new NotImplementedException();
        }

        public static void CoreValuesAreEqual(PurchaseOrderCreated expected, PurchaseOrderCreated actual)
        {
            Assert.Equal(expected.PurchaseOrderId, actual.PurchaseOrderId);
            Assert.Equal(expected.Status, actual.Status);
            Assert.Equal(expected.VendorId, actual.VendorId);
            CoreValuesAreEqual(expected.Lines, actual.Lines);
        }

        public static void CoreValuesAreEqual(IEnumerable<PurchaseOrderLine> expected, IEnumerable<PurchaseOrderLine> actual)
        {
            var expectedList = expected.ToList();
            var actualList = actual.ToList();
            Assert.Equal(expectedList.Count(), actualList.Count());
            for (int i = 0; i < expectedList.Count(); i++)
            {
                Assert.Equal(expectedList[i].ProductId, actualList[i].ProductId);
                Assert.Equal(expectedList[i].Description, actualList[i].Description);
                Assert.Equal(expectedList[i].Quantity, actualList[i].Quantity);
                Assert.Equal(expectedList[i].Measure, actualList[i].Measure);
                Assert.Equal(expectedList[i].PricePerUnit, actualList[i].PricePerUnit);
            }
        }

        public static void CoreValuesAreEqual(VendorDoesNotExist expected, VendorDoesNotExist actual)
        {
            Assert.Equal(expected.VendorId, actual.VendorId);
        }
    }
}