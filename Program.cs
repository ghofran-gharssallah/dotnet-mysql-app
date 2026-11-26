using MySqlConnector;

string host   = Environment.GetEnvironmentVariable("DB_HOST") ?? "127.0.0.1";
string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "myDB";
string user   = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
string pwd    = Environment.GetEnvironmentVariable("DB_PWD")  ?? "123";

string connStr = $"Server={host};Port=3306;Database={dbName};User ID={user};Password={pwd};SslMode=None;";
Console.WriteLine($"Connexion a MySQL sur {host}/{dbName} ...");

MySqlConnection? conn = null;
for (int essai = 1; essai <= 30; essai++)
{
    try
    {
        conn = new MySqlConnection(connStr);
        conn.Open();
        Console.WriteLine("Connexion etablie.");
        break;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Essai {essai}/30 : MySQL pas encore pret ({ex.Message})");
        conn = null;
        Thread.Sleep(5000);
    }
}

if (conn == null)
{
    Console.WriteLine("MysqlError : impossible de se connecter.");
    return 1;
}

using (conn)
{
    new MySqlCommand(
        @"CREATE TABLE IF NOT EXISTS emp (
            id INT AUTO_INCREMENT PRIMARY KEY,
            name VARCHAR(50),
            city VARCHAR(50),
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP)", conn).ExecuteNonQuery();

    var insert = new MySqlCommand("INSERT INTO emp (name, city) VALUES (@name, @city)", conn);
    insert.Parameters.AddWithValue("@name", "dotnet-" + Environment.MachineName);
    insert.Parameters.AddWithValue("@city", "Tunis");
    insert.ExecuteNonQuery();
    Console.WriteLine("new Line inserted");

    var total = new MySqlCommand("SELECT COUNT(*) FROM emp", conn).ExecuteScalar();
    Console.WriteLine($"Nombre de lignes dans emp : {total}");
}
return 0;
