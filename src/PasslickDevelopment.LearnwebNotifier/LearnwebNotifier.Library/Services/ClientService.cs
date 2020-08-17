using LearnwebNotifier.Library.Domain.Communication;
using LearnwebNotifier.Library.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace LearnwebNotifier.Library.Services
{
	public class ClientService : IClientService
	{

		private readonly ILoggerFactory loggerFactory;
		private readonly ILogger logger;
		private readonly ConfigService configService;
		private static readonly HttpClientHandler handler = new HttpClientHandler();
		private readonly HttpClient client = new HttpClient(handler);
		private static CookieContainer cookies = new CookieContainer();
		private readonly Uri baseUrl = new Uri("https://uni-muenster.de/LearnWeb/learnweb2/?lang=en");

		public ClientService(ILoggerFactory _loggerFactory)
		{
			loggerFactory = _loggerFactory;
			logger = loggerFactory.CreateLogger("Library.ClientService");
			configService = new ConfigService();

			// Basic settings
			handler.AllowAutoRedirect = true;
			handler.CookieContainer = cookies;
			client.BaseAddress = baseUrl;
			client.Timeout = TimeSpan.FromSeconds(60);

			// Request headers
			client.DefaultRequestHeaders.Add("Connection", "keep-alive");
			client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
			client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			client.DefaultRequestHeaders.Add("DNT", "1");
			client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.92 Safari/537.36");
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
			client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
			client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
			client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
			client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
		}


		/// <summary>
		/// Initializes the HttpClient with basic data and validates connection to Learnweb
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
					ClearCookies();
					logger.LogWarning($"Could not establish connection to Learnweb ({statusCode})");
					return ServiceStatus.Exception;
				}
			}
			catch (Exception ex)
			{
				ClearCookies();
				logger.LogError(ex.Message);
				return ServiceStatus.UnhandledException;
			}
		}


		/// <summary>
		/// Authorizes to Learnweb and populates cookie container with session cookies
		/// </summary>
		/// <returns>ServiceStatus Enum</returns>
		public ServiceStatus Authorize()
		{
			string username = configService.Config.Learnweb.Username;
			string password = configService.GetLearnwebPassword().value;

			if (Init() == ServiceStatus.OK)
			{
				List<KeyValuePair<string, string>> reqBody = new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("httpd_username", username),
					new KeyValuePair<string, string>("httpd_password", password)
				};

				try
				{
					var res = client.PostAsync("https://sso.uni-muenster.de/LearnWeb/learnweb2/?lang=en", new FormUrlEncodedContent(reqBody));
					if (res.Result.IsSuccessStatusCode) return ServiceStatus.OK;
					else
					{
						ClearCookies();
						logger.LogError($"Authentication for '{username}' failed ({res.Result.StatusCode})");
						return ServiceStatus.Exception;
					}
				}
				catch (Exception ex)
				{
					ClearCookies();
					logger.LogError(ex.Message);
					return ServiceStatus.UnhandledException;
				}
			}
			else
			{
				ClearCookies();
				logger.LogWarning("Learnweb initialization failed. Authentication not possible.");
				return ServiceStatus.Fail;
			}
		}


		/// <summary>
		/// Returns HttpClient
		/// </summary>
		/// <returns>HttpClient</returns>
		public HttpClient GetClient() => client;


		/// <summary>
		/// Refreshes Learnweb session without loosing authorization
		/// </summary>
		/// <returns>Status</returns>
		public ServiceStatus RefreshSession()
		{
			foreach (Cookie cookie in cookies.GetCookies(new Uri("https://sso.uni-muenster.de")))
			{
				if (cookie.Name == "MoodleSessionLearnweb2prod")
				{
					cookie.Expired = true;
					return ServiceStatus.OK;
				}
			}
			logger.LogWarning("Could not refresh session (cookie not found)");
			return ServiceStatus.Fail;
		}


		/// <summary>
		/// Clears cookies by setting new CookieContainer
		/// </summary>
		public void ClearCookies() => cookies = new CookieContainer();


		/// <summary>
		/// Disposes HttpClient and HttpClientHandler
		/// </summary>
		public void Dispose()
		{
			client.Dispose();
			handler.Dispose();
		}
	}
}


// (c) Passlick Development 2020. All rights reserved.
