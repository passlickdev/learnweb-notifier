namespace LearnwebNotifier.Library.Domain.Communication
{
	public enum ServiceStatus
	{
		Info = 1000,
		InfoRequested = 1001,
		OK = 2000,
		NewActivities = 2001,
		NoRecentActivities = 2002,
		PushSent = 2003,
		Valid = 2005,
		Exists = 2006,
		Warning = 3000,
		WidgetNotFound = 3001,
		Fail = 4000,
		PushFailed = 4003,
		NotFound = 4004,
		Invalid = 4005,
		Exception = 5000,
		Unauthorized = 5001,
		CriticalException = 6000,
		UnhandledException = 7000
	}
}


// (c) Passlick Development 2020. All rights reserved.
