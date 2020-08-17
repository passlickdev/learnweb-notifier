using System.Text.Json.Serialization;

namespace LearnwebNotifier.Push.Domain.Models
{
	public class PushNotification
	{
		[JsonPropertyName("token")]
		public string Token { get; set; }
		[JsonPropertyName("user")]
		public string UserToken { get; set; }
		[JsonPropertyName("title")]
		public string Title { get; set; }
		[JsonPropertyName("message")]
		public string Message { get; set; }
		[JsonPropertyName("url")]
		public string Url { get; set; }
		[JsonPropertyName("url_title")]
		public string UrlTitle { get; set; }
		[JsonPropertyName("priority")]
		public string Priority { get; set; }

		public PushNotification(string token, string userToken, string title, string message, string url, string urlTitle, string priority)
		{
			Token = token;
			UserToken = userToken;
			Title = title;
			Message = message;
			Url = url;
			UrlTitle = urlTitle;
			Priority = priority;
		}
	}
}


// (c) Passlick Development 2020. All rights reserved.
