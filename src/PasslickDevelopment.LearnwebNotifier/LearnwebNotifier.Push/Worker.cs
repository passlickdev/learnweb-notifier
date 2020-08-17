using LearnwebNotifier.Library.Domain.Communication;
using LearnwebNotifier.Library.Domain.Models;
using LearnwebNotifier.Library.Services;
using LearnwebNotifier.Push.Domain.Models;
using LearnwebNotifier.Push.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LearnwebNotifier.Push
{
	public class Worker : BackgroundService
	{

		private readonly ILoggerFactory loggerFactory;
		private readonly ILogger logger;
		private readonly ConfigService configService;
		private readonly ActivityService activityService;
		private readonly PushService pushService;

		private List<string> courseIds;
		private string pushoverToken;
		private string pushoverUser;
		private uint refreshDuration;

		public Worker()
		{
			configService = new ConfigService();

			if (configService.Config.Sentry.Activate)
			{
				loggerFactory = LoggerFactory.Create(builder =>
				{
					builder
						.AddConsole()
						.AddSentry(o =>
						{
							o.Debug = false;
							o.Dsn = configService.Config.Sentry.Dsn;
							o.MaxBreadcrumbs = 150;
							o.MinimumBreadcrumbLevel = LogLevel.Information;
							o.MinimumEventLevel = LogLevel.Warning;
						});
				});
			}
			else
			{
				loggerFactory = LoggerFactory.Create(builder =>
				{
					builder
						.AddConsole();
				});
			}
			logger = loggerFactory.CreateLogger("Push.Worker");

			activityService = new ActivityService(loggerFactory);
			pushService = new PushService(loggerFactory);
		}


		/// <summary>
		/// StartAsync Service
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override Task StartAsync(CancellationToken cancellationToken)
		{
			try
			{
				courseIds = configService.Config.Learnweb.Courses;
				pushoverToken = configService.Config.Pushover.Token;
				pushoverUser = configService.Config.Pushover.Recipient;
				refreshDuration = configService.Config.Service.Refresh;

				ServiceStatus activityStatus = activityService.Init();
				ServiceStatus pushStatus = pushService.Init();

				if (activityStatus == ServiceStatus.OK && pushStatus == ServiceStatus.OK)
					return base.StartAsync(cancellationToken);
				else
				{
					logger.LogError("Could not initialize 'Learnweb Notifier Service'");
					return base.StopAsync(cancellationToken);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return base.StopAsync(cancellationToken);
			}
		}


		/// <summary>
		/// StopAsync Service
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override Task StopAsync(CancellationToken cancellationToken)
		{
			activityService.Dispose();
			pushService.Dispose();
			return base.StopAsync(cancellationToken);
		}


		/// <summary>
		/// ExecuteAsync Service
		/// </summary>
		/// <param name="stoppingToken"></param>
		/// <returns></returns>
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				logger.LogInformation("Executing 'Learnweb Notifier Service' at {time}", DateTimeOffset.Now);
				try
				{
					foreach (string courseId in courseIds)
					{
						var res = activityService.FetchActivities(courseId);
						if (res.Status == ServiceStatus.NewActivities)
						{
							logger.LogInformation("Sending push notifications...");
							foreach (Activity activity in res.Activities)
							{
								string courseAbbrev = activity.Course.Abbrev;
								string activityType = activity.Type;
								string activityName = activity.Name;
								string activityUrl = activity.Url;

								if (activityName == null)
								{
									PushNotification notif = new PushNotification(
										pushoverToken,
										pushoverUser,
										"New course activity",
										$"There is new activity in {courseAbbrev}\n{activityType}",
										activityUrl,
										"View activity in Learnweb",
										"0"
									);
									pushService.SendPush(notif);
								}
								else
								{
									PushNotification notif = new PushNotification(
										pushoverToken,
										pushoverUser,
										"New course activity",
										$"There is new activity in {courseAbbrev}\n{activityType}: {activityName}",
										activityUrl,
										"View activity in Learnweb",
										"0"
									);
									pushService.SendPush(notif);
								}
								await Task.Delay(1000);
							}
						}
						else if (res.Status >= ServiceStatus.Fail) logger.LogWarning("Worker execution aborted (could not fetch activities)");
						await Task.Delay(1000);
					}
				}
				catch (Exception ex)
				{
					logger.LogError(ex.Message);
				}

				logger.LogInformation("Finished execution 'Learnweb Notifier Service' at {time}", DateTimeOffset.Now);
				logger.LogInformation("Next execution: {time}", DateTimeOffset.Now.AddMinutes(refreshDuration));
				await Task.Delay(TimeSpan.FromMinutes(refreshDuration), stoppingToken);
			}
		}
	}
}


// (c) Passlick Development 2020. All rights reserved.
