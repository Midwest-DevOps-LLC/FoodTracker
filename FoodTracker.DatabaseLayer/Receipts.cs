using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodTracker.DatabaseLayer
{
    public class Receipts : DBManager
    {
        #region BoringStuff

        string ConnectionString { get; set; }

        MySqlConnection sqlConnection { get; set; }

        public Receipts(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public Receipts(MySqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        internal MySqlConnection GetConnection()
        {
            if (sqlConnection == null)
            {
                return new MySqlConnection(this.ConnectionString);
            }
            else
            {
                return this.sqlConnection;
            }
        }

        #endregion

        public List<DataEntities.Receipt> GetAllReceipts()
        {
            List<DataEntities.Receipt> p = new List<DataEntities.Receipt>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM receipt;", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Add(new DataEntities.Receipt().ConvertMySQLToEntity(reader));
                    }
                }

                //foreach (var product in p)
                //{
                //    product.productPricings = GetProductPricings(product.ProductID.Value);
                //    product.productPictures = GetProductPictures(product.ProductID.Value);
                //    product.productChangelogs = GetProductChangeLogs(product.ProductID.Value);
                //}
            }

            return p;
        }

        public DataEntities.Receipt GetReceiptByGUID(string GUID)
        {
            DataEntities.Receipt person = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM receipt Where GUID = @GUID;", conn);

                cmd.Parameters.AddWithValue("@GUID", GUID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        person = new DataEntities.Receipt().ConvertMySQLToEntity(reader);
                    }
                }

                //person.productPricings = GetProductPricings(person.ProductID.Value);
                //person.productPictures = GetProductPictures(person.ProductID.Value);
                //person.productChangelogs = GetProductChangeLogs(person.ProductID.Value);
            }

            return person;
        }

        public long? SaveReceipt(DataEntities.Receipt p)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                if (p.ID == null)
                {
                    sql = @"INSERT INTO `receipt` (`ReceiptID`, `GUID`, `ItemCount`, `TC`, `Brand`, `DateOnReceipt`, `Total`, `Card`, `SubTotal`, `CreatedDate`, `IsVerified`, `FileExtension`) VALUES (NULL, @GUID, @ItemCount, @TC, @Brand, @DateOnReceipt, @Total, @Card, @SubTotal, CURRENT_TIMESTAMP, @IsVerified, @FileExtension);
                            SELECT LAST_INSERT_ID();";
                }
                else
                {
                    sql = @"UPDATE `receipt` SET GUID = @GUID, ItemCount = @ItemCount, TC = @TC, Brand = @Brand, DateOnReceipt = @DateOnReceipt, Total = @Total, Card = @Card, SubTotal = @SubTotal, CreatedDate = @CreatedDate, IsVerified = @IsVerified, FileExtension = @FileExtension WHERE ReceiptID = @ID;";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ID", p.ID);
                cmd.Parameters.AddWithValue("@GUID", p.GUID);
                cmd.Parameters.AddWithValue("@ItemCount", p.ItemCount);
                cmd.Parameters.AddWithValue("@TC", p.TC);
                cmd.Parameters.AddWithValue("@Brand", p.Brand);
                cmd.Parameters.AddWithValue("@DateOnReceipt", p.DateOnReceipt);
                cmd.Parameters.AddWithValue("@Total", p.Total);
                cmd.Parameters.AddWithValue("@Card", p.Card);
                cmd.Parameters.AddWithValue("@SubTotal", p.SubTotal);
                cmd.Parameters.AddWithValue("@CreatedDate", p.CreatedDate);
                cmd.Parameters.AddWithValue("@IsVerified", p.IsVerified);
                cmd.Parameters.AddWithValue("@FileExtension", p.FileExtension);

                if (p.ID == null) //Get new id number
                {
                    return cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                }
                else //Return id if worked
                {
                    cmd.ExecuteScalar();

                    return p.ID;
                }
            }
        }

        #region ReceiptPurchase
        public List<DataEntities.Receipt.Purchase> GetAllReceiptPurchases()
        {
            List<DataEntities.Receipt.Purchase> p = new List<DataEntities.Receipt.Purchase>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM receiptpurchase;", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Add(new DataEntities.Receipt.Purchase().ConvertMySQLToEntity(reader));
                    }
                }

                //foreach (var product in p)
                //{
                //    product.productPricings = GetProductPricings(product.ProductID.Value);
                //    product.productPictures = GetProductPictures(product.ProductID.Value);
                //    product.productChangelogs = GetProductChangeLogs(product.ProductID.Value);
                //}
            }

            return p;
        }

        public List<DataEntities.Receipt.Purchase> GetAllReceiptPurchasesByReceiptID(int receiptID)
        {
            List<DataEntities.Receipt.Purchase> p = new List<DataEntities.Receipt.Purchase>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM receiptpurchase WHERE receiptID = @ID;", conn);
                cmd.Parameters.AddWithValue("@ID", receiptID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Add(new DataEntities.Receipt.Purchase().ConvertMySQLToEntity(reader));
                    }
                }

                //foreach (var product in p)
                //{
                //    product.productPricings = GetProductPricings(product.ProductID.Value);
                //    product.productPictures = GetProductPictures(product.ProductID.Value);
                //    product.productChangelogs = GetProductChangeLogs(product.ProductID.Value);
                //}
            }

            return p;
        }

        public DataEntities.Receipt.Purchase GetReceiptPurchasesByID(string id)
        {
            DataEntities.Receipt.Purchase person = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM receiptpurchase Where ReceiptPurchaseID = @ID;", conn);

                cmd.Parameters.AddWithValue("@ID", id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        person = new DataEntities.Receipt.Purchase().ConvertMySQLToEntity(reader);
                    }
                }

                //person.productPricings = GetProductPricings(person.ProductID.Value);
                //person.productPictures = GetProductPictures(person.ProductID.Value);
                //person.productChangelogs = GetProductChangeLogs(person.ProductID.Value);
            }

            return person;
        }

        public long? SaveReceiptPurchase(DataEntities.Receipt.Purchase p)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                if (p.ReceiptPurchaseID == null)
                {
                    sql = @"INSERT INTO `receiptpurchase` (`ReceiptPurchaseID`, `ReceiptID`, `Product`, `SKU`, `ReceiptType`, `Price`, `TaxType`) VALUES (NULL, @ReceiptID, @Product, @SKU, @ReceiptType, @Price, @TaxType);
                            SELECT LAST_INSERT_ID();";
                }
                else
                {
                    sql = @"UPDATE `receiptpurchase` SET ReceiptID = @ReceiptID, Product = @Product, SKU = @SKU, ReceiptType = @ReceiptType, Price = @Price, TaxType = @TaxType WHERE ReceiptPurchaseID = @ID;";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ID", p.ReceiptPurchaseID);
                cmd.Parameters.AddWithValue("@ReceiptID", p.ReceiptID);
                cmd.Parameters.AddWithValue("@Product", p.Product);
                cmd.Parameters.AddWithValue("@SKU", p.SKU);
                cmd.Parameters.AddWithValue("@ReceiptType", p.Type);
                cmd.Parameters.AddWithValue("@Price", p.Price);
                cmd.Parameters.AddWithValue("@TaxType", p.TaxType);

                if (p.ReceiptPurchaseID == null) //Get new id number
                {
                    return cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                }
                else //Return id if worked
                {
                    cmd.ExecuteScalar();

                    return p.ReceiptPurchaseID;
                }
            }
        }
        #endregion
    }
}
