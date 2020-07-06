using System;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace dbConnection
{
    public class ConnectionCreator
    {
        private static string connInfo = "server=studmysql01.fhict.local;" + "database=dbi403931;" + "user=dbi403931;" + "password=kalina03;" + "connect timeout =30;";
        private static MySqlConnection c = null;
        public static MySqlConnection LoadConnection()
        {
            if (c is null)
            {
                c = new MySqlConnection(connInfo);
                return c;
            }
            else
            {
                return c;
            }
        }
    }
}