using LearnwebNotifier.Library.Domain.Communication;

namespace LearnwebNotifier.Library.Domain.Services
{
	public interface IActivityService
	{
		public ServiceStatus Init();
		public ServiceResponse FetchActivities(string courseId);
		public ServiceStatus RefreshSession();
		public void ClearCookies();
		public void Dispose();
	}
}


// (c) Passlick Development 2020. All rights reserved.
