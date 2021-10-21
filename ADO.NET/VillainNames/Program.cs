using Microsoft.Data.SqlClient;
using System;


namespace VillainNames
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

                var queryCommand = @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount FROM Villains AS v
                                     JOIN MinionsVillains AS mv ON v.Id = mv.VillainId
                                     GROUP BY v.Id, v.Name
                                     HAVING COUNT(mv.VillainId) > 0
                                     ORDER BY COUNT(mv.VillainId)";

                var command = new SqlCommand(queryCommand, connection);

                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader[0]} - {reader[1]}");
                    }
                }



            }
        }
    }
}
