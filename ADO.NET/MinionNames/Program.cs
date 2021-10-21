using Microsoft.Data.SqlClient;
using System;

namespace MinionNames
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

                int id = int.Parse(Console.ReadLine());

                var commandQuery = @"SELECT Name FROM Villains WHERE Id = @Id";
                SqlCommand command = CommandQuery(connection, id, commandQuery);

                var result = command.ExecuteScalar();

                if (result is null)
                {
                    Console.WriteLine($"No villain with ID {id} exists in the database.");
                }
                else
                {
                    Console.WriteLine($"Villain: {result}");

                    var queryCommand = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

                    SqlCommand query = CommandQuery(connection, id, queryCommand);

                    ReaderAndPrint(query);

                }

            }


        }

        private static void ReaderAndPrint(SqlCommand query)
        {
            SqlDataReader reader = query.ExecuteReader();

            using (reader)
            {
                if (reader is null)
                {
                    Console.WriteLine("(no minions)");
                }
                else
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["RowNum"]} {reader["Name"]} {reader["Age"]}");
                    }
                }
            }
        }

        private static SqlCommand CommandQuery(SqlConnection connection, int id, string commandQuery)
        {
            var command = new SqlCommand(commandQuery, connection);

            command.Parameters.AddWithValue($"@Id", id);
            return command;
        }
    }
}
