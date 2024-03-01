using MultiThreadLogger.Models;

namespace MultiThreadLogger.UnitTests;

[TestFixture]
public class ThreadLoggerTests
{
    private string TestFilePath = "C:\\junk\\out.txt";

    [OneTimeSetUp]
    public void Setup()
    {
        // Delete the test log file if it exists
        if (File.Exists(TestFilePath))
        {
            File.Delete(TestFilePath);
        }
    }

    [Test]
    public async Task RunLogThreadsAsync_ShouldWriteLogsToFile()
    {
        var logger = new ThreadLogger(TestFilePath, Program.DEFAULT_TASK_COUNT, Program.DEFAULT_LOG_COUNT);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            await logger.RunLogThreadsAsync();
        });

        Assert.IsTrue(File.Exists(TestFilePath));
        var logLines = await File.ReadAllLinesAsync(TestFilePath);
        Assert.That(logLines.Length, Is.EqualTo(Program.DEFAULT_LOG_COUNT * Program.DEFAULT_TASK_COUNT));

        int count = 0;
        foreach(string line in logLines)
        {
            string[] parts = line.Split(',');
            Assert.That(parts.Length, Is.EqualTo(3));
            Assert.That(int.Parse(parts[0]), Is.EqualTo(count));
            Assert.That(int.Parse(parts[1]), Is.GreaterThan(0));
            Assert.That(DateTime.TryParse(parts[2], out _), Is.True);   
            count++;
        }
    }
}
