using LearnwebNotifier.Library.Domain.Communication;
using LearnwebNotifier.Library.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sentry;
using static LearnwebNotifier.Library.Helper.HelperFunctions;

namespace LearnwebNotifier.Push
{
	public class Program
	{

		public static void Main(string[] args)
		{
			ConfigService configService = new ConfigService();

			PrintConsoleHeader();
			if (configService.CheckConfigExistence() == ServiceStatus.NotFound || (args.Length > 0 && args[0] == "--config"))
				configService.ConfigAssistant();
			else
			{
				if (configService.Config.Sentry.Activate)
				{
					using (SentrySdk.Init(configService.Config.Sentry.Dsn))
						CreateHostBuilder(args).Build().Run();
				}
				else
					CreateHostBuilder(args).Build().Run();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSystemd()
				.UseWindowsService()
				.ConfigureServices((hostContext, services) =>
				{
					services.AddHostedService<Worker>();
				});
	}
}


// (c) Passlick Development 2020. All rights reserved.
