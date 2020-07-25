using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TCP_Web_Server_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = new TcpListener(new IPEndPoint(IPAddress.IPv6Any, 80));
            listener.Server.DualMode = true;
            listener.Start();

            while (true)
            {
                Console.WriteLine("Waiting for a connection");
                TcpClient client = listener.AcceptTcpClient();

                StreamReader sr = new StreamReader(client.GetStream());
                StreamWriter sw = new StreamWriter(client.GetStream());
                try
                {
                    string request = sr.ReadLine();       
                    Console.WriteLine(request);
                    string[] tokens = request.Split(' ');
                    string page = tokens[1];
                    string methode = tokens[0];
                    string name = "";
                    string username = "";
                    string email = "";
                    string age = "";
                    string message = "";

                    if (page == "/")
                    {
                        page = "/Clan.html";
                    }
                    if (methode == "POST")
                    {
                        FormHandler(name, username, email, age, message);
                        page = "/Clan_Beitreten.html";
                    }
                    // Finde die Datei.
                    StreamReader file = new StreamReader("../../web" + page);
                    sw.WriteLine("HTTP/1.0 200 OK\n");

                    // Sende die Datei.
                    string data = file.ReadLine();
                    while (data != null)
                    {
                        sw.WriteLine(data);
                        sw.Flush();
                        data = file.ReadLine();
                    }
                }
                catch (Exception e)
                {
                    // Finde die Datei.
                    StreamReader file = new StreamReader("../../web/Error404.html");
                    sw.WriteLine("HTTP/1.0 404 OK\n");

                    // Sende die Datei.
                    string data = file.ReadLine();
                    while (data != null)
                    {
                        sw.WriteLine(data);
                        sw.Flush();
                        data = file.ReadLine();
                    }
                    sw.Flush();

                    Console.WriteLine("Error: " + e);
                    DateTime localDate = DateTime.Now;
                    string time = localDate.ToString();

                    time = time.Replace('.', '-');
                    time = time.Replace(' ', '_');
                    time = time.Replace(':', '-');
                    StreamWriter errorLog = new StreamWriter(@"../../Logs/ErrorLog " + time + ".txt");
                    errorLog.Write(e);
                    errorLog.Close();
                }
                client.Close();

                void FormHandler(string name, string username, string email, string age, string message)
                {
                    string test;

                    for (int i = 0; i < 17; i++) {
                        test = sr.ReadLine();
                        Console.WriteLine(test);

                        if (i == 16) {
                            name = test;
                            for (int e = 0; e < 4; e++) {
                                test = sr.ReadLine();
                                Console.WriteLine(test);
                            }
                            username = test;
                            for (int e = 0; e < 4; e++) {
                                test = sr.ReadLine();
                                Console.WriteLine(test);
                            }
                            email = test;
                            for (int e = 0; e < 4; e++) {
                                test = sr.ReadLine();
                                Console.WriteLine(test);
                            }
                            age = test;
                            for (int e = 0; e < 4; e++) {
                                test = sr.ReadLine();
                                Console.WriteLine(test);
                            }
                            message = test;
                        }
                    }
                    new Thread(() =>
                    {
                        SQLInsert(name, username, email, age, message);
                    }).Start();
                }

                void SQLInsert(string name, string username, string email, string age, string message)
                {
                    Console.WriteLine("////////////////////////////////");
                    Console.WriteLine("///");
                    Console.WriteLine("Getting Connection");
                    MySqlConnection conn = DBUtils.GetDBConnection();

                    try
                    {
                        Console.WriteLine("///");
                        Console.WriteLine("Openning Connection ...");

                        conn.Open();

                        Console.WriteLine("///");
                        Console.WriteLine("Connection Successful!");

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("///");
                        Console.WriteLine("Error: " + e.Message);
                        Console.WriteLine("///");
                        Console.WriteLine("////////////////////////////////");

                    }

                    string sql = "Insert into basedigitsclan.bewerbungen (Vorname, Username, Email, Age, Nachricht) " + " values ('" + name + "','" + username + "','" + email + "','" + age + "','" + message + "');";

                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("///");
                    Console.WriteLine("Command Successful!");
                    Console.WriteLine("///");
                    Console.WriteLine("////////////////////////////////");

                    conn.Close();
                    return;

                }
            }

        }
    }
}
