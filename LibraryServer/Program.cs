using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using SimpleTCP;



namespace LibraryServer
{
    class Program
    {
        static void Main()
        {
            Console.Title = "Server";
            var port = 2323;

            SimpleTcpServer server = new();
            Server server_logic = new();

            bool isDatabaseConnected = server_logic.DbStart();

            while (!isDatabaseConnected)
            {
                isDatabaseConnected = server_logic.DbStart();
                Thread.Sleep(1000);
                Alert.Error("Could not connect to MySQL database, retrying...");
            }

            if (isDatabaseConnected)
                Alert.Successs("Connected to MySQL database");    

            try
            {
                server.Start(port);
                Alert.Successs("Server started on port " + port);
            }
            catch
            {
                Alert.Error("Could not start the server");
            }

            if (server.IsStarted)
            {
                server.Delimiter = 0x13;
                server.DelimiterDataReceived += (sender, msg) => server_logic.ReadAndReply(msg);
            }

            while (true) 
                Console.ReadLine();
        }
    }
}
