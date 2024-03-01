using System.IO;

namespace MultiThreadLogger.UnitTests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ProgramTests
{
    [Test]
    public void ShouldReturnDefaultPath()
    {
        string path = Program.DetermineFilePath();
        Assert.That(path, Is.EqualTo(Path.Combine(Directory.GetCurrentDirectory(), Program.DEFAULT_FILE_NAME)));
    }

    [Test]
    public void ShouldReturnDefaultPathWhenRunningInDocker()
    {
        Environment.SetEnvironmentVariable("RUNNING_IN_DOCKER", "true");
        string path = Program.DetermineFilePath();
        Assert.That(path, Is.EqualTo(Path.Combine(Program.DEFAULT_CONTAINER_PATH, Program.DEFAULT_FILE_NAME)));
    }

    [Test]
    public void ShouldCreateLogFile()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), Program.DEFAULT_FILE_NAME);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        Program.CreateLogFile();
        Assert.IsTrue(File.Exists(path));
    }
}
