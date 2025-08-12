using System.Data.SQLite;

public class PlateDatabase
{
    private readonly string _connectionString = "Data Source=plate_database.db; Version=3;";

    public PlateDatabase()
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS WhiteList (Plate TEXT PRIMARY KEY)", conn);
            cmd.ExecuteNonQuery();
        }
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS BlackList (Plate TEXT PRIMARY KEY)", conn);
            cmd.ExecuteNonQuery();
        }
    }

    public bool IsPlateAuthorized(string plate)
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SQLiteCommand("SELECT COUNT(*) FROM WhiteList WHERE Plate = @plate", conn);
            cmd.Parameters.AddWithValue("@plate", plate);
            long count = (long)cmd.ExecuteScalar();
            return count > 0;
        }
    }

    public bool IsPlateBlocked(string plate) 
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SQLiteCommand("SELECT COUNT(*) FROM BlackList WHERE Plate = @plate", conn);
            cmd.Parameters.AddWithValue("@plate", plate);
            long count = (long)cmd.ExecuteScalar();
            return count > 0;
        }
    }


    public void AddPlate(string plate)
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SQLiteCommand("INSERT OR IGNORE INTO WhiteList (Plate) VALUES (@plate)", conn);
            cmd.Parameters.AddWithValue("@plate", plate);
            cmd.ExecuteNonQuery();
        }
    }

    public void AddBlockedPlate(string plate) 
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SQLiteCommand("INSERT OR IGNORE INTO BlackList (Plate) VALUES (@plate)", conn);
            cmd.Parameters.AddWithValue("@plate", plate);
            cmd.ExecuteNonQuery();
        }
    }
}
