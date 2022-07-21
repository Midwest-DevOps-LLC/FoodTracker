using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodTracker.DataEntities
{
    public class Receipt
    {
        public int? ID { get; set; }
        public string GUID { get; set; }
        public int ItemCount { get; set; }
        public string TC { get; set; }
        public string Brand { get; set; }
        public DateTime DateOnReceipt { get; set; }
        public decimal Total { get; set; }
        public string Card { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsVerified { get; set; }
        public string FileExtension { get; set; }
        public List<Purchase> Purchases { get; set; } = new List<Purchase>();

        public class Purchase
        {
            public int? ReceiptPurchaseID { get; set; }
            public int ReceiptID { get; set; }
            public string Product { get; set; }
            public string SKU { get; set; }
            public string Type { get; set; }
            public decimal Price { get; set; }
            public string TaxType { get; set; }

            public DataEntities.Receipt.Purchase ConvertMySQLToEntity(MySqlDataReader reader)
            {
                DataEntities.Receipt.Purchase p = new DataEntities.Receipt.Purchase();

                p.ReceiptPurchaseID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "ReceiptPurchaseID"));
                p.ReceiptID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "ReceiptID"));
                p.Product = DBUtilities.ReturnSafeString(reader, "Product");
                p.SKU = DBUtilities.ReturnSafeString(reader, "SKU");
                p.Type = DBUtilities.ReturnSafeString(reader, "ReceiptType");
                p.Price = DBUtilities.ReturnSafeDecimal(reader, "Price").GetValueOrDefault();
                p.TaxType = DBUtilities.ReturnSafeString(reader, "TaxType");

                return p;
            }
        }

        public DataEntities.Receipt ConvertMySQLToEntity(MySqlDataReader reader)
        {
            DataEntities.Receipt p = new DataEntities.Receipt();

            p.ID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "ReceiptID"));
            p.GUID = DBUtilities.ReturnSafeString(reader, "GUID");
            p.ItemCount = DBUtilities.ReturnSafeInt(reader, "ItemCount").GetValueOrDefault();
            p.TC = DBUtilities.ReturnSafeString(reader, "TC");
            p.Brand = DBUtilities.ReturnSafeString(reader, "Brand");
            p.DateOnReceipt = DBUtilities.ReturnSafeDateTime(reader, "DateOnReceipt").GetValueOrDefault();
            p.Total = DBUtilities.ReturnSafeDecimal(reader, "Total").GetValueOrDefault();
            p.Card = DBUtilities.ReturnSafeString(reader, "Card");
            p.SubTotal = DBUtilities.ReturnSafeDecimal(reader, "SubTotal").GetValueOrDefault();
            p.CreatedDate = DBUtilities.ReturnSafeDateTime(reader, "CreatedDate").GetValueOrDefault();
            p.IsVerified = DBUtilities.ReturnBoolean(reader, "IsVerified");
            p.FileExtension = DBUtilities.ReturnSafeString(reader, "FileExtension");

            return p;
        }
    }
}
