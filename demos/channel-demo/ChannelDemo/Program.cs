using System.Threading.Channels;
const int CAPACITY = 20;
var sw = new System.Diagnostics.Stopwatch();

var options = new BoundedChannelOptions(CAPACITY) {
	FullMode = BoundedChannelFullMode.Wait
};
var channel = Channel.CreateBounded<Func<Task>>(options);



await Task.WhenAll(PushThingsIntoChannelAsync(), PullThingsOutOfChannel());

async Task PushThingsIntoChannelAsync() {
	sw.Start();
	const int TASK_COUNT = 50;
	for (var i = 0; i < TASK_COUNT; i++) {
		var localI = i;
		var task = async () => {
			await Task.Delay(TimeSpan.FromMilliseconds(500));
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Task {localI} completed");
		};
		await channel.Writer.WriteAsync(task);
		// await Task.Delay(TimeSpan.FromMilliseconds(500));
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine($"Task {i} added to the channel");
	}
	sw.Stop();
	Console.Write($"Queueing {TASK_COUNT} tasks took {sw.ElapsedMilliseconds} ms");
}

async Task PullThingsOutOfChannel() {
	while (true) {
		var task = await channel.Reader.ReadAsync();
		await task();
	}
}



