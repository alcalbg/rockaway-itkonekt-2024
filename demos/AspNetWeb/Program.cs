var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Inline, return a string
app.MapGet("/", () => "Hello Belgrade!");

// Inline, returning an object - which will be turned into JSON
app.MapGet("/greeting", () => new Greeting("Belgrade"));

// Return a method call
app.MapGet("/time", GetCurrentTime);

app.MapGet("/greeting/{name}", (string name) => new Greeting(name));

app.Run();

DateTime GetCurrentTime() {
	return DateTime.MinValue;
}

class Greeting {
	public string Name { get; set; }
	public Greeting(string name) {
		this.Name = name;
	}
}