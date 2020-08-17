using LearnwebNotifier.Library.Domain.Communication;
using LearnwebNotifier.Push.Domain.Models;

namespace LearnwebNotifier.Push.Domain.Services
{
	public interface IPushService
	{
		public ServiceStatus Init();
		public ServiceStatus SendPush(PushNotification pushNotif);
		public void Dispose();
	}
}


// (c) Passlick Development 2020. All rights reserved.
