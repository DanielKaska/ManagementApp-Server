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
    public class Client
    {
        static string[] Deserialize(Message message)
        {
            return JsonSerializer.Deserialize<string[]>(message.MessageString);
        }

        public static void Login(Message message, MySqlConnection conn)
        {
            string[] data = Deserialize(message);

            string commandText = "SELECT login, password from employees WHERE login = @login and password = @password";

            using var command = new MySqlCommand(commandText, conn);

            command.Parameters.AddWithValue("@login", data?[1]);
            command.Parameters.AddWithValue("@password", data?[2]);

            using var reader = command.ExecuteReader();
            command.Cancel();

            if (reader.Read())
                message.Reply("logged");
        }

        public static void GetData(Message message, MySqlConnection conn)
        {
            string[]? data = Deserialize(message);
            string commandText = "SELECT * FROM customers where id = @id or name = @id or second_name = @id";
            using var command = new MySqlCommand(commandText, conn);

            command.Parameters.AddWithValue("@id", data?[1]);

            using MySqlDataReader reader = command.ExecuteReader();
            reader.Read();

            List<string> dataToSend = new()
            {
                "clientInfo"
            };

            try
            {
                for (int i = 0; i < reader.FieldCount; i++)
                    dataToSend.Add(reader[i].ToString());
            }
            catch
            {
                dataToSend.Add("Nie znaleziono podanego klienta");
            }
            message.Reply(JsonSerializer.Serialize<List<string>>(dataToSend));
            reader.Close();
        }

        public static void Add(Message message, MySqlConnection conn)
        {
            string[]? data = Deserialize(message);
            string commandText = "INSERT INTO customers(name, second_name) VALUES (@name, @surname)";
            using var command = new MySqlCommand(commandText, conn);

            command.Parameters.AddWithValue("@name", data?[1]);
            command.Parameters.AddWithValue("@surname", data?[2]);

            try
            {
                command.ExecuteNonQuery();
                message.Reply("clientAdded");
            }
            catch
            {
                message.Reply("addingClientFailed");
            }
            
        }

    }
}
