using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodTracker.BusinessLogicLayer
{
    public class Receipts : BLLManager, IDisposable
    {

        public Receipts(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public Receipts(MySqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public List<DataEntities.Receipt> GetAllReceipts()
        {
            try
            {
                DatabaseLayer.Receipts userDLL = new DatabaseLayer.Receipts(GetConnection());

                return userDLL.GetAllReceipts();
            }
            catch (Exception e)
            {
                MDO.Utility.Standard.LogHandler.SaveException(e);
            }

            return new List<DataEntities.Receipt>();
        }

        public DataEntities.Receipt GetReceiptByGUID(string GUID)
        {
            try
            {
                DatabaseLayer.Receipts userDLL = new DatabaseLayer.Receipts(GetConnection());

                return userDLL.GetReceiptByGUID(GUID);
            }
            catch (Exception e)
            {
                MDO.Utility.Standard.LogHandler.SaveException(e);
            }

            return null;
        }

        public long? SaveReceipt(DataEntities.Receipt product)
        {
            try
            {
                DatabaseLayer.Receipts userDLL = new DatabaseLayer.Receipts(GetConnection());

                return userDLL.SaveReceipt(product);
            }
            catch (Exception e)
            {
                MDO.Utility.Standard.LogHandler.SaveException(e);
            }

            return null;
        }

        #region ReceiptPurchase

        public List<DataEntities.Receipt.Purchase> GetAllReceiptPurchases()
        {
            try
            {
                DatabaseLayer.Receipts userDLL = new DatabaseLayer.Receipts(GetConnection());

                return userDLL.GetAllReceiptPurchases();
            }
            catch (Exception e)
            {
                MDO.Utility.Standard.LogHandler.SaveException(e);
            }

            return new List<DataEntities.Receipt.Purchase>();
        }

        public List<DataEntities.Receipt.Purchase> GetAllReceiptPurchasesByReceiptID(int receiptID)
        {
            try
            {
                DatabaseLayer.Receipts userDLL = new DatabaseLayer.Receipts(GetConnection());

                return userDLL.GetAllReceiptPurchasesByReceiptID(receiptID);
            }
            catch (Exception e)
            {
                MDO.Utility.Standard.LogHandler.SaveException(e);
            }

            return new List<DataEntities.Receipt.Purchase>();
        }

        public DataEntities.Receipt.Purchase GetReceiptPurchasesByID(string receiptPurchaseID)
        {
            try
            {
                DatabaseLayer.Receipts userDLL = new DatabaseLayer.Receipts(GetConnection());

                return userDLL.GetReceiptPurchasesByID(receiptPurchaseID);
            }
            catch (Exception e)
            {
                MDO.Utility.Standard.LogHandler.SaveException(e);
            }

            return null;
        }

        public long? SaveReceiptPurchase(DataEntities.Receipt.Purchase product)
        {
            try
            {
                DatabaseLayer.Receipts userDLL = new DatabaseLayer.Receipts(GetConnection());

                return userDLL.SaveReceiptPurchase(product);
            }
            catch (Exception e)
            {
                MDO.Utility.Standard.LogHandler.SaveException(e);
            }

            return null;
        }

        #endregion
    }
}
