using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Rockaway.WebApp.Services;

namespace Rockaway.WebApp.Tests.Services;

public class StatusTests {

	private static readonly JsonSerializerOptions jsonSerializerOptions
		= new(JsonSerializerDefaults.Web);

	[Fact]
	public async Task Status_Endpoint_Returns_Success() {
		await using var factory = new WebApplicationFactory<Program>();
		using var client = factory.CreateClient();
		using var response = await client.GetAsync("/status");
		response.EnsureSuccessStatusCode();
	}

	[Fact]
	public async Task Status_Endpoint_Returns_Hostname() {
		await using var factory = new WebApplicationFactory<Program>();
		using var client = factory.CreateClient();
		var json = await client.GetStringAsync("/status");
		var status = JsonSerializer.Deserialize<ServerStatus>(json, jsonSerializerOptions);
		Assert.NotNull(status);
		Assert.Equal("HAYWORTH", status.Hostname);
	}
}