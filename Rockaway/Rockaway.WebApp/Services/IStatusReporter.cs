// Rockaway.WebApp/Services/IStatusReporter.cs

namespace Rockaway.WebApp.Services;

public interface IStatusReporter {
	public ServerStatus GetStatus();
}