using System;
using System.Data.SQLite;

public class PlateDatabase
{
    private readonly string _connectionString = "Data Source=plate_database.db; Version=3;";

    public PlateDatabase()
    {
        try
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                Console.WriteLine("Veritabanı bağlantısı başarılı.");

                string createWhiteList = "CREATE TABLE IF NOT EXISTS WhiteList (Plate TEXT PRIMARY KEY)";
                string createBlackList = "CREATE TABLE IF NOT EXISTS BlackList (Plate TEXT PRIMARY KEY)";

                using (var cmd = new SQLiteCommand(createWhiteList, conn))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(createBlackList, conn))
                    cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Veritabanı bağlantı hatası: " + ex.Message);
            throw; // Hatanın üst katmana iletilmesi
        }
    }

    private bool PlateExists(string tableName, string plate)
    {
        try
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string query = $"SELECT COUNT(*) FROM {tableName} WHERE Plate = @plate";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@plate", plate);
                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sorgu hatası ({tableName}): {ex.Message}");
            return false;
        }
    }

    public bool IsPlateAuthorized(string plate) => PlateExists("WhiteList", plate);
    public bool IsPlateBlocked(string plate) => PlateExists("BlackList", plate);

    private void AddPlateToTable(string tableName, string plate)
    {
        try
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string query = $"INSERT OR IGNORE INTO {tableName} (Plate) VALUES (@plate)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@plate", plate);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ekleme hatası ({tableName}): {ex.Message}");
        }
    }

    public void AddPlate(string plate) => AddPlateToTable("WhiteList", plate);
    public void AddBlockedPlate(string plate) => AddPlateToTable("BlackList", plate);
}
