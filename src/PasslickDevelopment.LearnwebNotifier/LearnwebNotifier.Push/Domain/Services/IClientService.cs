using LearnwebNotifier.Library.Domain.Communication;
using System.Net.Http;

namespace LearnwebNotifier.Push.Domain.Services
{
	public interface IClientService
	{
		public ServiceStatus Init();
		public HttpClient GetClient();
		public void Dispose();
	}
}


// (c) Passlick Development 2020. All rights reserved.
