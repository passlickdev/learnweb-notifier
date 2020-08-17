using LearnwebNotifier.Library.Domain.Communication;
using LearnwebNotifier.Library.Domain.Models;

namespace LearnwebNotifier.Library.Domain.Services
{
	public interface IConfigService
	{
		public ServiceStatus LoadConfig();
		public ServiceStatus GenerateConfig(ConfigStruct config);
		public ServiceStatus CheckConfigExistence();
		public (ServiceStatus status, string value) GetLearnwebPassword();
		public ServiceStatus ConfigAssistant();
	}
}


// (c) Passlick Development 2020. All rights reserved.
