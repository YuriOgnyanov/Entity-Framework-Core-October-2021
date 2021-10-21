using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrintAllMinionName
{
    public class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=.;Database=MinionsDB;Integrated Security=true";
            var connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();

                var selectQuery = "SELECT Name FROM Minions";
                var command = new SqlCommand(selectQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                List<string> minionNames = new List<string>();

                using (reader)
                {
                    while (reader.Read())
                    {
                        minionNames.Add((string)reader["Name"]);
                    }

                    for (int i = 0; i < minionNames.Count / 2; i++)
                    {
                        Console.WriteLine(minionNames[i]);
                        Console.WriteLine(minionNames[minionNames.Count - 1 - i]);
                    }

                    if (minionNames.Count % 2 == 1)
                    {
                        Console.WriteLine(minionNames[minionNames.Count / 2]);
                    }
                }
            }
        }
    }
}
