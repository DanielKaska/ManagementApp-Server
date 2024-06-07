using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleTCP;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic;
using MySqlConnector;
using System.Security.AccessControl;
using System.Data;
using System.Security.Cryptography;

namespace LibraryServer
{
    public class Server
    {
        public MySqlConnection? conn;

        public bool DbStart()
        {
            bool result;
            MySqlConnection connection = new("Server=127.0.0.1;Database=Library;User Id=root;Password=;");

            try
            {
                connection.Open();
                if (connection.Ping())
                {
                    result = true;
                    conn = connection;
                }
                else
                {
                    result = false;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public void ReadAndReply(Message message)
        {
            string[]? data = JsonSerializer.Deserialize<string[]>(message.MessageString);

            if (data?[0] == "login") 
                Client.Login(message, conn);

            if (data?[0] == "get")
                Client.GetData(message, conn);

            if (data?[0] == "add")
                Client.Add(message, conn);

        }

    }
}
