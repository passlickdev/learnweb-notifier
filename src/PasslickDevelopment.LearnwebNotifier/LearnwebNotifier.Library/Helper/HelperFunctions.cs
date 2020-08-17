using System;
using System.Reflection;

namespace LearnwebNotifier.Library.Helper
{
	public static class HelperFunctions
	{

		public static void ConsoleWrite(string text, ConsoleColor fgcolor = ConsoleColor.Gray, ConsoleColor bgcolor = ConsoleColor.Black)
		{
			Console.ForegroundColor = fgcolor;
			Console.BackgroundColor = bgcolor;
			Console.Write(text);
			Console.ResetColor();
		}


		public static void ConsoleWriteLine(string text, ConsoleColor fgcolor = ConsoleColor.Gray, ConsoleColor bgcolor = ConsoleColor.Black)
		{
			Console.ForegroundColor = fgcolor;
			Console.BackgroundColor = bgcolor;
			Console.WriteLine(text);
			Console.ResetColor();
		}


		public static string ReadPassword()
		{
			string password = string.Empty;
			ConsoleKey key;
			do
			{
				var keyInfo = Console.ReadKey(true);
				key = keyInfo.Key;

				if (key == ConsoleKey.Backspace && password.Length > 0)
				{
					Console.Write("\b \b");
					password = password[0..^1];
				}
				else if (!char.IsControl(keyInfo.KeyChar))
				{
					Console.Write("∗");
					password += keyInfo.KeyChar;
				}
			} while (key != ConsoleKey.Enter);
			return password;
		}


		public static void PrintConsoleHeader()
		{
			Console.Clear();
			Console.WriteLine("\n***************************************************************************\n");
			Console.WriteLine($"  WWU Learnweb Notifier Service v{Assembly.GetEntryAssembly().GetName().Version}");
			Console.WriteLine($"  (c) Passlick Development {DateTime.UtcNow.Year}. All rights reserved.");
			Console.WriteLine("\n***************************************************************************\n");
		}


		// Hardcoded encryption key for weak string protection against easy plaintext read -- does not serve safety-critical purpose
		private static readonly string encryptionKey = "d7z8jhL5uvPFbsMzyfobhwHdoEeovkUGlztcTVFrdp86NwItxK9VN6kvNDhaAtIo";

		public static string EncryptString(string value) => SimpleAES.AES256.Encrypt(value, encryptionKey);
		public static string DecryptString(string value) => SimpleAES.AES256.Decrypt(value, encryptionKey);

	}
}


// (c) Passlick Development 2020. All rights reserved.
