using HtmlAgilityPack;
using LearnwebNotifier.Library.Domain.Communication;
using LearnwebNotifier.Library.Domain.Models;
using LearnwebNotifier.Library.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace LearnwebNotifier.Library.Services
{
	public class CrawlerService : ICrawlerService
	{

		private readonly ILoggerFactory loggerFactory;
		private readonly ILogger logger;
		private readonly ClientService clientService;
		private readonly HttpClient client;

		public CrawlerService(ClientService _clientService, ILoggerFactory _loggerFactory)
		{
			loggerFactory = _loggerFactory;
			logger = loggerFactory.CreateLogger("Library.CrawlerService");
			clientService = _clientService;
			client = _clientService.GetClient();
		}


		/// <summary>
		/// Gets course HTML document from Learnweb
		/// </summary>
		/// <param name="course">Course</param>
		/// <returns>Status and course HTML</returns>
		private (ServiceStatus status, HtmlDocument courseHtml) GetCourse(Course course)
		{
			try
			{
				var res = client.GetAsync(course.Url);
				if (res.Result.IsSuccessStatusCode)
				{
					clientService.RefreshSession();
					string htmlString = res.Result.Content.ReadAsStringAsync().Result;
					HtmlDocument courseHtml = new HtmlDocument();
					courseHtml.LoadHtml(htmlString);
					return (ServiceStatus.OK, courseHtml);
				}
				else
				{
					logger.LogError($"Could not load course with id '{course.Id}' ({res.Result.StatusCode})");
					return (ServiceStatus.Exception, null);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return (ServiceStatus.UnhandledException, null);
			}
		}


		/// <summary>
		/// Validates course HTML document for activities and activity widget
		/// </summary>
		/// <param name="course">Course</param>
		/// <param name="courseHtml">Course HTML document</param>
		/// <returns>Status</returns>
		private ServiceStatus ValidateCourse(Course course, HtmlDocument courseHtml)
		{
			HtmlNode htmlNode = courseHtml.DocumentNode;
			HtmlNode activityHeader = htmlNode.SelectSingleNode("//div[@class='activityhead']");
			HtmlNode activity = htmlNode.SelectSingleNode("//p[@class='activity']");

			if (activityHeader == null)
			{
				logger.LogWarning($"No activity widget found for course id '{course.Id}'");
				return ServiceStatus.WidgetNotFound;
			}
			else if (activity == null)
			{
				logger.LogInformation($"No recent activity found for course id '{course.Id}'");
				return ServiceStatus.NoRecentActivities;
			}
			else if (activity != null)
			{
				logger.LogInformation($"New activities found for course id '{course.Id}'");
				return ServiceStatus.NewActivities;
			}
			else
			{
				logger.LogError($"Course validation error for course id '{course.Id}'");
				return ServiceStatus.Exception;
			}
		}


		/// <summary>
		/// Populates an Course object with relevant basic information for the course
		/// </summary>
		/// <param name="course">Course</param>
		/// <param name="courseHtml">Course HTML document</param>
		/// <returns>Status and populated Course object</returns>
		private (ServiceStatus status, Course populatedCourse) PopulateCourse(Course course, HtmlDocument courseHtml)
		{
			HtmlNode htmlNode = courseHtml.DocumentNode;
			HtmlNode courseName = htmlNode.SelectSingleNode("//*[@id='region-main']/h1");
			HtmlNode courseAbbrev = htmlNode.SelectSingleNode("//*[@id='page-navbar']/nav/ol/li[4]/a");

			if (htmlNode != null && courseName != null && courseAbbrev != null)
			{
				course.Name = courseName.InnerText;
				course.Abbrev = courseAbbrev.InnerText;
				return (ServiceStatus.OK, course);
			}
			else
			{
				logger.LogWarning($"Could not populate course with course id '{course.Id}'");
				return (ServiceStatus.Fail, course);
			}
		}


		/// <summary>
		/// Parses an course HTML document for activities
		/// </summary>
		/// <param name="course">Course</param>
		/// <returns>Status and list of parsed activities</returns>
		public (ServiceStatus status, List<Activity> activities) ParseCourse(Course course)
		{
			try
			{
				var courseGet = GetCourse(course);
				ServiceStatus courseValidation =
					(courseGet.status == ServiceStatus.OK)
					? ValidateCourse(course, courseGet.courseHtml)
					: ServiceStatus.Fail;

				if (courseValidation == ServiceStatus.NewActivities)
				{
					HtmlDocument courseHtml = courseGet.courseHtml;
					HtmlNode documentNode = courseHtml.DocumentNode;
					course = PopulateCourse(course, courseHtml).populatedCourse;

					HtmlNode activityHeader = documentNode.SelectSingleNode("//div[@class='activityhead']");
					string activityDateString = activityHeader.InnerText.Replace("Activity since ", "").Replace(",", "");
					DateTime activityDate = DateTime.Parse(activityDateString, new CultureInfo("de-DE"));

					List<Activity> activities = new List<Activity>();
					foreach (HtmlNode node in documentNode.SelectNodes("//p[@class='activity']"))
					{
						int nodeCount = node.ChildNodes.Count;
						string activityType = "N/A";
						string activityName = null;
						string activityUrl = null;

						if (nodeCount > 0)
							activityType = node.ChildNodes[0].InnerText;
						if (nodeCount > 2)
						{
							activityName = node.ChildNodes[2].InnerText;
							activityUrl = node.ChildNodes[2].Attributes["href"].Value;
						}

						Activity activity = new Activity(activityDate, course, activityType, activityName, activityUrl);
						activities.Add(activity);
					}
					return (ServiceStatus.NewActivities, activities);
				}
				else if (courseValidation == ServiceStatus.NoRecentActivities)
					return (ServiceStatus.NoRecentActivities, null);
				else
				{
					logger.LogError($"Course parsing error for course id '{course.Id}'");
					return (ServiceStatus.Exception, null);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return (ServiceStatus.UnhandledException, null);
			}
		}
	}
}


// (c) Passlick Development 2020. All rights reserved.
