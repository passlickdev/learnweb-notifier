using LearnwebNotifier.Library.Domain.Communication;
using LearnwebNotifier.Library.Domain.Models;
using System.Collections.Generic;

namespace LearnwebNotifier.Library.Domain.Services
{
	public interface ICrawlerService
	{
		public (ServiceStatus status, List<Activity> activities) ParseCourse(Course course);
	}
}


// (c) Passlick Development 2020. All rights reserved.
