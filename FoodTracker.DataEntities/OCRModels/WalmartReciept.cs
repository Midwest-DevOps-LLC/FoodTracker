using System;
using System.Collections.Generic;

namespace FoodTracker.DataEntities.OCRModels
{
    public class WalmartReciept
    {
        public long ItemCount { get; set; }
        public string TC { get; set; }
        public string Brand { get; set; }
        public string DateTime { get; set; }
        public double Total { get; set; }
        public string Card { get; set; }
        public double SubTotal { get; set; }
        public List<Purchase> Purchases { get; set; } = new List<Purchase>();

        public class Purchase
        {
            public string Product { get; set; }
            public string SKU { get; set; }
            public string Type { get; set; }
            public string Price { get; set; }
            public string TaxType { get; set; }

            public Purchase() { }

            public Purchase(DataEntities.Receipt.Purchase r)
            {
                this.Product = r.Product;
                this.SKU = r.SKU;
                this.Type = r.Type;
                this.Price = r.Price.ToString();
                this.TaxType = r.TaxType;
            }
        }

        public WalmartReciept() { }

        public WalmartReciept(DataEntities.Receipt r)
        {
            this.Brand = r.Brand;
            this.Card = r.Card;
            this.TC = r.TC;
            this.ItemCount = r.ItemCount;
            this.DateTime = r.DateOnReceipt.ToString("f");
            if (double.TryParse(r.SubTotal.ToString(), out double subTotal))
                this.SubTotal = subTotal;
            if (double.TryParse(r.Total.ToString(), out double total))
                this.Total = total;
        }
    }
}
