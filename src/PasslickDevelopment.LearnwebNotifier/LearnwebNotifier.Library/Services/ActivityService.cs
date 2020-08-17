using LearnwebNotifier.Library.Domain.Communication;
using LearnwebNotifier.Library.Domain.Models;
using LearnwebNotifier.Library.Domain.Services;
using Microsoft.Extensions.Logging;
using System;

namespace LearnwebNotifier.Library.Services
{
	public class ActivityService : IActivityService
	{

		private readonly ILoggerFactory loggerFactory;
		private readonly ILogger logger;
		private readonly ClientService clientService;
		private CrawlerService crawlerService;

		public ActivityService(ILoggerFactory _loggerFactory)
		{
			loggerFactory = _loggerFactory;
			logger = loggerFactory.CreateLogger("Library.ActivityService");
			clientService = new ClientService(loggerFactory);
		}


		/// <summary>
		/// Initializes ActivityService with CrawlerService
		/// </summary>
		/// <returns>ServiceStatus</returns>
		public ServiceStatus Init()
		{
			if (clientService.Authorize() == ServiceStatus.OK)
			{
				crawlerService = new CrawlerService(clientService, loggerFactory);
				return ServiceStatus.OK;
			}
			else return ServiceStatus.Fail;
		}


		/// <summary>
		/// Fetches activities by inheriting CrawlerService for courseId
		/// </summary>
		/// <param name="courseId">Course ID</param>
		/// <returns>ServiceResponse object</returns>
		public ServiceResponse FetchActivities(string courseId)
		{
			try
			{
				Course course = new Course(courseId);
				if (crawlerService != null)
				{
					var (status, activities) = crawlerService.ParseCourse(course);
					return new ServiceResponse(status, activities);
				}
				else return new ServiceResponse(ServiceStatus.Exception, null);
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return new ServiceResponse(ServiceStatus.UnhandledException, null);
			}
		}


		/// <summary>
		/// Refreshes ClientService session
		/// </summary>
		/// <returns>ServiceStatus</returns>
		public ServiceStatus RefreshSession() => clientService.RefreshSession();


		/// <summary>
		/// Clears ClientService cookies
		/// </summary>
		public void ClearCookies() => clientService.ClearCookies();


		/// <summary>
		/// Disposes ClientService connection
		/// </summary>
		public void Dispose() => clientService.Dispose();
	}
}


// (c) Passlick Development 2020. All rights reserved.
