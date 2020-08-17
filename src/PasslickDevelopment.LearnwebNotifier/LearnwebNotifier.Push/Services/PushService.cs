using LearnwebNotifier.Library.Domain.Communication;
using LearnwebNotifier.Push.Domain.Models;
using LearnwebNotifier.Push.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LearnwebNotifier.Push.Services
{
	public class PushService : IPushService
	{

		private readonly ILoggerFactory loggerFactory;
		private readonly ILogger logger;
		private readonly JsonSerializerOptions jsonOptions;
		private readonly ClientService clientService;

		public PushService(ILoggerFactory _loggerFactory)
		{
			loggerFactory = _loggerFactory;
			logger = loggerFactory.CreateLogger("Push.PushService");

			jsonOptions = new JsonSerializerOptions
			{
				IgnoreNullValues = true
			};

			clientService = new ClientService(loggerFactory);
		}


		/// <summary>
		/// Initializes ClientService
		/// </summary>
		/// <returns>ServiceStatus</returns>
		public ServiceStatus Init() => clientService.Init();


		/// <summary>
		/// Send push notification using Pushover API
		/// </summary>
		/// <param name="pushNotif">Push notification object</param>
		/// <returns>ServiceStatus</returns>
		public ServiceStatus SendPush(PushNotification pushNotif)
		{
			try
			{
				string pushJson = JsonSerializer
					.Serialize(pushNotif, jsonOptions);
				StringContent reqBody = new StringContent(pushJson, Encoding.UTF8, "application/json");
				var res = clientService
					.client
					.PostAsync("https://api.pushover.net/1/messages.json", reqBody);

				if (res.Result.IsSuccessStatusCode) return ServiceStatus.PushSent;
				else
				{
					logger.LogWarning("Failed to send push notification, retry attempt...");
					return RetryPush(pushNotif);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return ServiceStatus.UnhandledException;
			}
		}


		/// <summary>
		/// Retry push notification delivery after failed first attempt using Pushover API
		/// </summary>
		/// <param name="pushNotif">Push notification object</param>
		/// <returns>ServiceStatus</returns>
		private ServiceStatus RetryPush(PushNotification pushNotif)
		{
			try
			{
				string pushJson = JsonSerializer
					.Serialize(pushNotif, jsonOptions);
				StringContent reqBody = new StringContent(pushJson, Encoding.UTF8, "application/json");

				for (int i = 0; i < 5; i++)
				{
					var res = clientService
						.client
						.PostAsync("https://api.pushover.net/1/messages.json", reqBody);

					if (res.Result.IsSuccessStatusCode) return ServiceStatus.PushSent;
					else logger.LogWarning($"Failed to send push notification on attempt #{i + 1}");
					Task.Delay(5000);
				}

				logger.LogError("Failed to send push notification on final attempt");
				return ServiceStatus.PushFailed;
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return ServiceStatus.UnhandledException;
			}
		}


		/// <summary>
		/// Disposes ClientService connection
		/// </summary>
		public void Dispose() => clientService.Dispose();
	}
}


// (c) Passlick Development 2020. All rights reserved.
