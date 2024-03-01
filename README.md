# MultiThreadLogger

An exercise to demonstrate the ability to perform multithreaded programming in a safe manner. All together this exercies demonstrates: 
- In `ThreadLogger.LogConsumerTask` A suitable method for ensuring synchronized access to a single file.
- Good exception and error handling in `ThreadLogger.LogConsumerTask` and `ThreadLogger.RunLogThreadsAsync`. 
- Good OOP principles in the `ThreadLogger` class.

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
2. The assessment specifies the timestamp be accurate to one millisecond so the log file will contain the timestamp in the format `yyyy-MM-dd HH:mm:ss.fff`. Many lines in the log file may appear to have the same timestamp, but this is due to the fact that the timestamp is only accurate to one millisecond. Adjusting the timestamp for greater accuracy would ensure that each line has a unique timestamp.

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
