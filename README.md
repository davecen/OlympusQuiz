# MultiThreadLogger

An exercise to demonstrate the ability to perform multithreaded programming in a safe manner.

## Getting Started

### Visual Studio

To get started with the MultiThreadLogger application, follow these steps:

1. Clone the repository or download the source code.
2. Open the solution in Visual Studio.
3. Build the solution.
4. Run the application using the visual studio debugger.

### Docker

To run the application using Docker, follow these steps:

1. Pull the image from Docker Hub using the following command:

```bash
docker pull davecen/multithreadlogger
```

2. Run the image using the following command:

```bash
docker run -i -v <host-path>:/log davecen/multithreadlogger
```

Replace `<host-path>` with the path to the directory on your local machine where you want the log file to be saved.

## Considerations

1. The application is designed to be run in a multithreaded environment. The application will create a number of threads and each thread will write to the log file. The application is designed to be run in a Docker container, so the log file will be written to the `/log` directory in the container. The log file will be named `out.txt`			.
2. The assessment specifies the timestamp be accurate to one millisecond so the log file will contain the timestamp in the format `yyyy-MM-dd HH:mm:ss.fff`. Many lines in the file may appear to have the same timestamp, but this is due to the fact that the timestamp is only accurate to the millisecond. Adjusting the timestamp for further accuracy could be done to see the changes in the log file.

## Developer Notes

## Program.cs

The `Program.cs` file is the entry point of the `MultiThreadLogger` application. It contains the `Main` method, which is the starting point of the program execution. 

### Constants
- `DEFAULT_CONTAINER_PATH`: A constant string representing the default container path for the log file.
- `DEFAULT_FILE_NAME`: A constant string representing the default file name for the log file.
- `DEFAULT_TASK_COUNT`: A constant integer representing the default number of log threads to create.
- `DEFAULT_LOG_COUNT`: A constant integer representing the default number of logs to write.

### Variables
- `_filePath`: A static string variable that holds the path of the log file.

### Main Method
The `Main` method is an asynchronous method that serves as the entry point of the program. It performs the following steps:
1. Determines the file path based on the environment by calling the `DetermineFilePath` method.
2. Creates the log file by calling the `CreateLogFile` method.
3. Prints the file path to the console.
4. Creates a new instance of the `ThreadLogger` class, passing the file path, default task count, and default log count as parameters.
5. Runs the log threads asynchronously by calling the `RunLogThreadsAsync` method of the `ThreadLogger` instance.
6. Prints a success message to the console.
7. Waits for a key press before exiting the program.

### DetermineFilePath Method
The `DetermineFilePath` method determines the file path based on whether the program is running in a Docker container or not. It checks the value of the `RUNNING_IN_DOCKER` environment variable and returns the appropriate file path.

### CreateLogFile Method
The `CreateLogFile` method creates the directory and log file if they do not exist. It performs the following steps:
1. Checks if the log file already exists. If not, it creates the directory using the `CreateDirectory` method of the `Directory` class.
2. Creates the log file using the `File.Create` method.

If any exception occurs during the creation of the log file, an error message is printed to the console and the exception is re-thrown.


## ThreadLogger.cs

The `ThreadLogger` class is a model class that represents a logger for multi-threaded programming. It provides functionality to run log threads asynchronously and write logs to a file in a safe manner.

### Properties

- `_logCount`: An integer representing the number of logs to write.
- `_taskCount`: An integer representing the number of log threads to create.
- `_lineNumber`: An integer representing the current line number for the logs.
- `_filePath`: A string representing the path of the log file.
- `_lineNumberLock`: An object used for locking the `_lineNumber` variable.
- `_logQueueLock`: An object used for locking the log queue.
- `_logQueue`: A `ConcurrentQueue<string?>` used for storing the logs.
- `_ts`: A `CancellationTokenSource` used for canceling the log tasks.

### Constructor

- `ThreadLogger(string filePath, int taskCount, int logCount)`: Initializes a new instance of the `ThreadLogger` class with the specified file path, task count, and log count.

### Methods

- `RunLogThreadsAsync()`: Runs the log threads asynchronously. It starts the log consumer task, generates log producer tasks, waits for all log writing tasks to finish, and then waits for the log consumer task to finish.
- `GenerateLogProducerTasks(CancellationToken cancellationToken)`: Builds an array of tasks that produce logs to the queue.
- `LogProducerTask(CancellationToken cancellationToken)`: A task that produces logs to the queue.
- `LogConsumerTask(CancellationToken cancellationToken)`: A task that consumes logs from the queue and writes them to the log file.

## Testing


The `ThreadLoggerTests` class contains unit tests for the `ThreadLogger` class in the `MultiThreadLogger` application. Let's go through each test case:

1. `Setup` method:
   - This method is decorated with the `[OneTimeSetUp]` attribute, indicating that it should be executed once before all the test cases in the class.
   - It deletes the test log file if it exists, ensuring a clean state for each test run.

2. `RunLogThreadsAsync_ShouldWriteLogsToFile` test case:
   - This test case verifies that the `RunLogThreadsAsync` method of the `ThreadLogger` class writes logs to the file correctly.
   - It performs the following assertions:
     - Creates an instance of the `ThreadLogger` class with the test file path and default task count and log count.
     - Calls the `RunLogThreadsAsync` method and asserts that it does not throw any exceptions.
     - Asserts that the test log file exists.
     - Reads all the lines from the test log file and asserts that the number of lines is equal to the product of the default task count and default log count.
     - Iterates through each log line and performs the following assertions:
       - Splits the line by comma and asserts that the number of parts is equal to 3.
       - Parses the first part as an integer and asserts that it is equal to the expected count.
       - Parses the second part as an integer and asserts that it is greater than 0.
       - Tries to parse the third part as a `DateTime` and asserts that it is successful.
       - Increments the count for each log line.

These tests ensure that the `ThreadLogger` class correctly writes logs to the file in a multi-threaded environment.
