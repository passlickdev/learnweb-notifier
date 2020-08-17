namespace LearnwebNotifier.Library.Domain.Models
{
	public class Course
	{
		public string Id { get; }
		public string Name { get; set; }
		public string Abbrev { get; set; }
		public string Url { get; set; }

		public Course(string id)
		{
			Id = id;
			Name = "N/A";
			Abbrev = "N/A";
			Url = $"https://sso.uni-muenster.de/LearnWeb/learnweb2/course/view.php?id={Id}&lang=en";
		}
	}
}


// (c) Passlick Development 2020. All rights reserved.
