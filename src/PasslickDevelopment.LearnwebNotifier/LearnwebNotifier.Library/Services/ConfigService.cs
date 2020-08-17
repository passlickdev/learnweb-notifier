using LearnwebNotifier.Library.Domain.Communication;
using LearnwebNotifier.Library.Domain.Models;
using LearnwebNotifier.Library.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static LearnwebNotifier.Library.Helper.HelperFunctions;

namespace LearnwebNotifier.Library.Services
{
	public class ConfigService : IConfigService
	{

		private readonly ILoggerFactory loggerFactory;
		private readonly ILogger logger;
		public ConfigStruct Config;

		public ConfigService()
		{
			loggerFactory = LoggerFactory.Create(builder =>
			{
				builder
					.AddConsole();
			});
			logger = loggerFactory.CreateLogger("Library.ConfigService");

			if (CheckConfigExistence() == ServiceStatus.Exists)
			{
				LoadConfig();
				if (Config.Sentry.Activate)
				{
					loggerFactory = LoggerFactory.Create(builder =>
					{
						builder
							.AddConsole()
							.AddSentry(o =>
							{
								o.Debug = false;
								o.Dsn = Config.Sentry.Dsn;
								o.MaxBreadcrumbs = 150;
								o.MinimumBreadcrumbLevel = LogLevel.Information;
								o.MinimumEventLevel = LogLevel.Warning;
							});
					});
					logger = loggerFactory.CreateLogger("Library.ConfigService");
				}
			}
		}


		/// <summary>
		/// Loads config file
		/// </summary>
		/// <returns>Service Status Code</returns>
		public ServiceStatus LoadConfig()
		{
			try
			{
				Config = JsonSerializer.Deserialize<ConfigStruct>(File.ReadAllText("config.json"));
				return ServiceStatus.OK;
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return ServiceStatus.UnhandledException;
			}
		}


		/// <summary>
		/// Generates config file for the service in the working directory
		/// </summary>
		/// <returns>Service Status Code</returns>
		public ServiceStatus GenerateConfig(ConfigStruct config)
		{
			try
			{
				config.Learnweb.Password = EncryptString(config.Learnweb.Password);

				JsonSerializerOptions options = new JsonSerializerOptions
				{
					WriteIndented = true
				};

				string configJson = JsonSerializer.Serialize<ConfigStruct>(config, options);
				File.WriteAllText("config.json", configJson);
				return ServiceStatus.OK;
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return ServiceStatus.UnhandledException;
			}
		}


		/// <summary>
		/// Checks the config file existence
		/// </summary>
		/// <returns>Service Status Code</returns>
		public ServiceStatus CheckConfigExistence()
		{
			try
			{
				if (File.Exists("config.json"))
					return ServiceStatus.Exists;
				else
					return ServiceStatus.NotFound;
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return ServiceStatus.UnhandledException;
			}
		}


		/// <summary>
		/// Returns decrypted Learnweb password
		/// </summary>
		/// <returns>Decrypted Learnweb password</returns>
		public (ServiceStatus status, string value) GetLearnwebPassword()
		{
			try
			{
				string password = DecryptString(Config.Learnweb.Password);
				return (ServiceStatus.OK, password);
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return (ServiceStatus.UnhandledException, null);
			}
		}


		/// <summary>
		/// Configuration Assistant
		/// </summary>
		/// <returns>Service Status Code</returns>
		public ServiceStatus ConfigAssistant()
		{
			try
			{

				ConsoleWriteLine("\n --- LEARNWEB NOTIFIER CONFIGURATION ASSISTANT --- \n\n", ConsoleColor.Black, ConsoleColor.Green);

				Console.WriteLine("The service will create a configuration file which is necessary for the execution of the service. " +
					"Please enter all the data below and confirm each by pressing 'Enter'. Your password will be encrypted and only stored on your machine.");

				ConsoleWrite("\nLearnweb User (a_bcde01):", ConsoleColor.Black, ConsoleColor.Gray);
				Console.Write(" ");
				string inputLearnwebUser = Console.ReadLine();

				ConsoleWrite("Learnweb Password:", ConsoleColor.Black, ConsoleColor.Gray);
				Console.Write(" ");
				string inputLearnwebPassword = ReadPassword();

				Console.WriteLine("\n\n\nEnter the course IDs for the courses which should be monitored by the service. " +
					"You can add one or more courses. If you want to add more than one course, please separate the IDs with a comma, but without spaces (12345,67890).\n");

				ConsoleWrite("Learnweb Course IDs:", ConsoleColor.Black, ConsoleColor.Gray);
				Console.Write(" ");
				List<string> inputCourses = Console.ReadLine().Split(',').ToList();

				Console.WriteLine("\n\nEnter the data for the Pushover notification service. " +
					"You need an API token and a token for the receiver, which can be a single user or a usergroup.\n");

				ConsoleWrite("Pushover API Token:", ConsoleColor.Black, ConsoleColor.Gray);
				Console.Write(" ");
				string inputPushoverToken = Console.ReadLine();

				ConsoleWrite("Pushover Recipient Token:", ConsoleColor.Black, ConsoleColor.Gray);
				Console.Write(" ");
				string inputPushoverRecipient = Console.ReadLine();
				Console.WriteLine("");


				ConfigStruct newConfig = new ConfigStruct(
					new ConfigStruct.LearnwebConfig(inputLearnwebUser, inputLearnwebPassword, inputCourses),
					new ConfigStruct.PushoverConfig(inputPushoverToken, inputPushoverRecipient)
					);

				if (GenerateConfig(newConfig) == ServiceStatus.OK)
					ConsoleWriteLine("\nConfiguration file generated, please restart the service now!", ConsoleColor.Green);
				else
					ConsoleWriteLine("\nConfiguration file generation failed! Aborting...", ConsoleColor.Red);

				Console.WriteLine("Press any key to quit...\n");
				Console.ReadKey(true);
				Environment.Exit(0);
				return ServiceStatus.OK;

			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				Environment.Exit(0);
				return ServiceStatus.UnhandledException;
			}
		}

	}
}


// (c) Passlick Development 2020. All rights reserved.
