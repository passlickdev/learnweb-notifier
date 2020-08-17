using LearnwebNotifier.Library.Domain.Communication;
using System.Net.Http;

namespace LearnwebNotifier.Library.Domain.Services
{
	public interface IClientService
	{
		public ServiceStatus Init();
		public ServiceStatus Authorize();
		public ServiceStatus RefreshSession();
		public HttpClient GetClient();
		public void ClearCookies();
		public void Dispose();
	}
}


// (c) Passlick Development 2020. All rights reserved.
