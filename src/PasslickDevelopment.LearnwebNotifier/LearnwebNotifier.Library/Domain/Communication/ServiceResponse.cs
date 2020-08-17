using LearnwebNotifier.Library.Domain.Models;
using System;
using System.Collections.Generic;

namespace LearnwebNotifier.Library.Domain.Communication
{
	public class ServiceResponse
	{
		public Guid ExecId { get; }
		public DateTime ExecDate { get; }
		public ServiceStatus Status { get; }
		public List<Activity> Activities { get; }

		public ServiceResponse(ServiceStatus status, List<Activity> activities)
		{
			ExecId = Guid.NewGuid();
			ExecDate = DateTime.UtcNow;
			Status = status;
			Activities = activities;
		}
	}
}


// (c) Passlick Development 2020. All rights reserved.
