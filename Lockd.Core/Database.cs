using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace Lockd.Core;

public class Database 
{
	public readonly string? _dbPath;

	public Database()
	{
		// Create directory of Lockd in special folder (ApplicationData/.config)
		string specialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Lockd");
		Directory.CreateDirectory(specialDirectory);
		_dbPath = Path.Combine(specialDirectory, "lockd.db");

		if (!File.Exists(_dbPath))
			InitializeDatabase();
	}

	private void InitializeDatabase()
	{
		using var connection = new SqliteConnection($"Data Source={_dbPath}");
		connection.Open();

		string createMasterTable = @"
			CREATE TABLE IF NOT EXISTS master (
					masterp TEXT PRIMARY KEY;
			)";

		string createStorageTable = @"
			CREATE TABLE IF NOT EXISTS passwords (
					id INTEGER PRIMARY KEY AUTOINCREMENT,
					service TEXT NOT NULL,
					username TEXT NOT NULL,
					password TEXT NOT NULL
			)";

		using var masterCommand = new SqliteCommand(createMasterTable, connection);
		masterCommand.ExecuteNonQuery();

		using var storageCommand = new SqliteCommand(createStorageTable, connection);
		storageCommand.ExecuteNonQuery();
	}

	public bool CheckIfMasterPasswordExists()
	{
		using var connection = new SqliteConnection($"Data Source={_dbPath}");
		connection.Open();

		string masterQuery = "SELECT COUNT(*) FROM master";
		using var command = new SqliteCommand(masterQuery, connection);
		var result = command.ExecuteScalar();

		return Convert.ToInt32(result) > 0;
	}

	public void AddPassword(string service, string username, string password)
	{
		using var connection = new SqliteConnection($"Data Source={_dbPath}");	
		connection.Open();

		string insertQuery = @"
			INSERT INTO passwords (service, username, password)
			VALUES (@service, @username, @password);
		";

		using var command = new SqliteCommand(insertQuery, connection);
		command.Parameters.AddWithValue("@service", service);
		command.Parameters.AddWithValue("@username", username);
		command.Parameters.AddWithValue("@password", password);
		command.ExecuteNonQuery();

	}

	public void GetPasswords()
	{
		using var connection = new SqliteConnection($"Data Source={_dbPath}");
		connection.Open();

		string selectQuery = "SELECT service, username, password FROM passwords";
		using var command = new SqliteCommand(selectQuery, connection);
		using var reader = command.ExecuteReader();

		Console.WriteLine("\nStored Passwords:");
		while (reader.Read())
		{
			Console.WriteLine($"{reader.GetString(0)} | {reader.GetString(1)} | (Encrypted)");
		}
	}
}
