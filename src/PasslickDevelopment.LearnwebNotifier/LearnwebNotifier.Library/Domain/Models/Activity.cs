using System;

namespace LearnwebNotifier.Library.Domain.Models
{
	public class Activity
	{
		public DateTime? Date { get; }
		public Course Course { get; }
		public string Type { get; }
		public string Name { get; }
		public string Url { get; }

		public Activity(DateTime? date, Course course, string type, string name, string url)
		{
			Date = date;
			Course = course;
			Type = type;
			Name = name;
			Url = url;
		}
	}
}


// (c) Passlick Development 2020. All rights reserved.
