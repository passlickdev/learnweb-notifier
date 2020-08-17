using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LearnwebNotifier.Library.Domain.Models
{
	public class ConfigStruct
	{
		[JsonPropertyName("service")]
		public ServiceConfig Service { get; set; } = new ServiceConfig(5);
		[JsonPropertyName("learnweb")]
		public LearnwebConfig Learnweb { get; set; } = new LearnwebConfig("", "", new List<string> { "" });
		[JsonPropertyName("pushover")]
		public PushoverConfig Pushover { get; set; } = new PushoverConfig("", "");
		[JsonPropertyName("sentry")]
		public SentryConfig Sentry { get; set; } = new SentryConfig(false, "");

		public ConfigStruct(LearnwebConfig learnweb, PushoverConfig pushover)
		{
			Learnweb = learnweb;
			Pushover = pushover;
		}
		public ConfigStruct() { }


		public class ServiceConfig
		{
			[JsonPropertyName("refresh")]
			public uint Refresh { get; set; }

			public ServiceConfig(uint refresh)
			{
				Refresh = refresh;
			}
			public ServiceConfig() { }
		}

		public class LearnwebConfig
		{
			[JsonPropertyName("user")]
			public string Username { get; set; }
			[JsonPropertyName("password")]
			public string Password { get; set; }
			[JsonPropertyName("courses")]
			public List<string> Courses { get; set; }

			public LearnwebConfig(string username, string password, List<string> courses)
			{
				Username = username;
				Password = password;
				Courses = courses;
			}
			public LearnwebConfig() { }
		}

		public class PushoverConfig
		{
			[JsonPropertyName("token")]
			public string Token { get; set; }
			[JsonPropertyName("recipient")]
			public string Recipient { get; set; }

			public PushoverConfig(string token, string recipient)
			{
				Token = token;
				Recipient = recipient;
			}
			public PushoverConfig() { }
		}

		public class SentryConfig
		{
			[JsonPropertyName("activate")]
			public bool Activate { get; set; }
			[JsonPropertyName("dsn")]
			public string Dsn { get; set; }

			public SentryConfig(bool activate, string dsn)
			{
				Activate = activate;
				Dsn = dsn;
			}
			public SentryConfig() { }
		}

	}
}


// (c) Passlick Development 2020. All rights reserved.
