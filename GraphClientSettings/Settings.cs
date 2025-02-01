namespace GraphClientSettings
{
	using Microsoft.Extensions.Configuration;

	public class Settings
	{
		public string? ClientId { get; set; }
		public string? ClientSecret { get; set; }

		public string? TenantId { get; set; }
		public string? AuthTenant { get; set; }
		public string[]? GraphUserScopes { get; set; }

		public static Settings LoadSettings()
		{
			IConfiguration config = new ConfigurationBuilder()
			.AddJsonFile($"{Directory.GetCurrentDirectory()}\\appsettings.json", optional: false)
			.AddJsonFile($"{Directory.GetCurrentDirectory()}\\appsettings.Development.json", optional: true)
			.Build();
			return config!.GetRequiredSection("Settings")!.Get<Settings>()!;
		}
	}
}