using MySql.Data.MySqlClient;
using System;

namespace FoodTracker.BusinessLogicLayer
{
    public class BLLManager
    {
        internal string ConnectionString { get; set; }

        internal MySqlConnection sqlConnection { get; set; }

        public MySqlConnection GetConnection()
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

        public void Dispose()
        {
            if (sqlConnection != null)
                sqlConnection.Dispose();
        }
    }
}
