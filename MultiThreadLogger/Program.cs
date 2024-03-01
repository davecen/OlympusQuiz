using MultiThreadLogger.Models;
using System.Text.RegularExpressions;

public class Program
{
    public const string DEFAULT_CONTAINER_PATH = "/log";
    public const string DEFAULT_FILE_NAME = "out.txt";
    public const int DEFAULT_TASK_COUNT = 10;
    public const int DEFAULT_LOG_COUNT = 10;
    private static string _filePath = Path.Combine(Directory.GetCurrentDirectory(), DEFAULT_FILE_NAME);

    public static async Task Main(string[] args)
    {
        try
        {
            //get file path based on env
            _filePath = DetermineFilePath();

            //create the log file
            CreateLogFile();
            
            Console.WriteLine($"Writing logs to {_filePath}.");

            //new thread logger
            ThreadLogger threadLogger = new(_filePath, DEFAULT_TASK_COUNT, DEFAULT_LOG_COUNT);
            
            //run the log threads
            await threadLogger.RunLogThreadsAsync();
            
            Console.WriteLine($"Logs written successfully to {_filePath}.");
            
            Console.WriteLine("Press any key to exit...");
            
            Console.Read();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public static string DetermineFilePath()
    {
        bool isDocker = Environment.GetEnvironmentVariable("RUNNING_IN_DOCKER") == "true";

        if (isDocker)
        {
            return Path.Combine(DEFAULT_CONTAINER_PATH, DEFAULT_FILE_NAME);
        }
        else
        {
            return Path.Combine(Directory.GetCurrentDirectory(), DEFAULT_FILE_NAME);
        }
    }



    /// <summary>
    /// Create the directory and log file if they do not exist
    /// </summary>
    public static void CreateLogFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
                using FileStream fs = File.Create(_filePath);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error creating log file: {ex.Message}");
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied while creating log file: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error while creating log file: {ex.Message}");
            throw;
        }
    }
}