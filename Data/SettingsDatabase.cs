using System.Data.SQLite;

public class SettingsDatabase
{
    private readonly string _connectionString = "Data Source=plate_database.db;Version=3;";

    public SettingsDatabase()
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Settings (Key TEXT PRIMARY KEY, Value TEXT)", conn);
            cmd.ExecuteNonQuery();
        }
    }

    public void SaveSetting(string key, string value)
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SQLiteCommand("INSERT OR REPLACE INTO Settings (Key, Value) VALUES (@key, @value)", conn);
            cmd.Parameters.AddWithValue("@key", key);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.ExecuteNonQuery();
        }
    }

    public string GetSetting(string key)
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SQLiteCommand("SELECT Value FROM Settings WHERE Key = @key", conn);
            cmd.Parameters.AddWithValue("@key", key);
            var result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : null;
        }
    }
}
