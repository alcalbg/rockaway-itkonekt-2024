// Rockaway.WebApp/Services/IStatusReporter.cs
using System.Reflection;

namespace Rockaway.WebApp.Services;

public interface IStatusReporter {
	public ServerStatus GetStatus();
}

public class ServerStatus {
	public string Assembly { get; set; } = String.Empty;
	public string Modified { get; set; } = String.Empty;
	public string Hostname { get; set; } = String.Empty;
	public string DateTime { get; set; } = String.Empty;
}

public class StatusReporter : IStatusReporter {
	private static readonly Assembly assembly = Assembly.GetEntryAssembly()!;
	public ServerStatus GetStatus() => new() {
		Assembly = assembly.FullName ?? "Assembly.GetEntryAssembly() returned null",
		Modified = new DateTimeOffset(File.GetLastWriteTimeUtc(assembly.Location), TimeSpan.Zero).ToString("O"),
		Hostname = Environment.MachineName,
		DateTime = DateTimeOffset.UtcNow.ToString("O")
	};
}