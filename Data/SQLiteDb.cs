using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Dapper;

public class SQLiteDb
{
    private readonly string _connectionString;

    public SQLiteDb(string dbPath)
    {
        if (!File.Exists(dbPath))
        {
            SQLiteConnection.CreateFile(dbPath);
        }

        _connectionString = string.Format("Data Source={0};Version=3;", dbPath);
        EnsureTables();
    }

    private void EnsureTables()
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();

            string whiteListSql = @"CREATE TABLE IF NOT EXISTS WhiteList (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Plate TEXT NOT NULL UNIQUE,
                OwnerName TEXT,
                IsActive INTEGER DEFAULT 1
            );";

            string logSql = @"CREATE TABLE IF NOT EXISTS AccessLog (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Plate TEXT,
                Time TEXT,
                IsWhiteListed INTEGER
            );";

            conn.Execute(whiteListSql);
            conn.Execute(logSql);
        }
    }

    public bool IsPlateInWhiteList(string plate)
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            var result = conn.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM WhiteList WHERE Plate = @plate AND IsActive = 1",
                new { plate });
            return result > 0;
        }
    }

    public void LogAccess(string plate, bool isWhiteListed)
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            conn.Execute(
                "INSERT INTO AccessLog (Plate, Time, IsWhiteListed) VALUES (@plate, @time, @status)",
                new
                {
                    plate = plate,
                    time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    status = isWhiteListed ? 1 : 0
                });
        }
    }

    public List<string> GetAllPlates()
    {
        using (var conn = new SQLiteConnection(_connectionString))
        {
            conn.Open();
            return conn.Query<string>(
                "SELECT Plate FROM WhiteList WHERE IsActive = 1").AsList();
        }
    }
}
