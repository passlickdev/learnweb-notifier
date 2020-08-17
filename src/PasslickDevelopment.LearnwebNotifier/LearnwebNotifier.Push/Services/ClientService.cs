using LearnwebNotifier.Library.Domain.Communication;
using LearnwebNotifier.Push.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace LearnwebNotifier.Push.Services
{
	public class ClientService : IClientService
	{

		private readonly ILoggerFactory loggerFactory;
		private readonly ILogger logger;
		public HttpClient client = new HttpClient();
		private readonly Uri baseUrl = new Uri("https://api.pushover.net/");

		public ClientService(ILoggerFactory _loggerFactory)
		{
			loggerFactory = _loggerFactory;
			logger = loggerFactory.CreateLogger("Push.ClientService");

			// Basic settings
			client.BaseAddress = baseUrl;
			client.Timeout = TimeSpan.FromSeconds(20);
		}


		/// <summary>
		/// Initializes the HttpClient with basic data and validates connection to Pushover API
		/// </summary>
		/// <returns>ServiceStatus Enum</returns>
		public ServiceStatus Init()
		{
			try
			{
				var res = client.GetAsync(baseUrl);
				var statusCode = res.Result;
				if (statusCode.IsSuccessStatusCode) return ServiceStatus.OK;
				else
				{
					logger.LogWarning($"Could not establish connection to Pushover API ({statusCode})");
					return ServiceStatus.Exception;
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return ServiceStatus.UnhandledException;
			}
		}


		/// <summary>
		/// Returns HttpClient
		/// </summary>
		/// <returns>HttpClient</returns>
		public HttpClient GetClient() => client;


		/// <summary>
		/// Disposes HttpClient
		/// </summary>
		public void Dispose() => client.Dispose();
	}
}


// (c) Passlick Development 2020. All rights reserved.
