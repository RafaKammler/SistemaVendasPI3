using MySql.Data.MySqlClient;

namespace ProjetoIntegradorVendas
{
    public static class Database
    {
        private static readonly string connectionString = "Server=localhost;Database=projetointegrador3;Uid=root;Pwd=admin;";

        public static MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(connectionString);
            return connection;
        }
    }
}