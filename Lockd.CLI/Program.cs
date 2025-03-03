using Lockd.Core;

Database db = new Database();

while (true)
{
	Console.WriteLine("\nLockd - CLI Password Manager");
	Console.WriteLine("1. Add Password");
	Console.WriteLine("2. View Stored Passwords");
	Console.WriteLine("3. Exit");
	Console.Write("Choose an option: ");

	string? choice = Console.ReadLine();

	switch (choice)
	{
		case "1":
			Console.Write("Service: ");
			string? service = Console.ReadLine();
			Console.Write("Username: ");
			string? username = Console.ReadLine();
			Console.Write("Password: ");
			string? password = Console.ReadLine();

			if (service == null || username == null || password == null)
			{
				Console.WriteLine("Invalid input.");
				break;
			}

			var encryptedPassword = Encryption.Encrypt(password);
			db.AddPassword(service, username, password);
			break;
		case "2":
			db.GetPasswords();
			break;
		case "3":
			return;
		default:
			Console.WriteLine("Invalid choice.");
			break;
	}
}
