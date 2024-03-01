using System.Collections.Concurrent;

namespace MultiThreadLogger.Models;

public class ThreadLogger
{

    private int _logCount;
    private int _taskCount;
    private int _lineNumber;
    private string _filePath;
    private object _lineNumberLock = new();
    private object _logQueueLock = new();
    private ConcurrentQueue<string?> _logQueue = new();
    private CancellationTokenSource _ts = new();

    public ThreadLogger(string filePath, int taskCount, int logCount)
    {
        _filePath = filePath;
        _logCount = logCount;
        _taskCount = taskCount;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task RunLogThreadsAsync()
    {        
        //start the log consumer task
        var logConsumerTask = Task.Run(() => LogConsumerTask(_ts.Token));
        
        //all the log producing tasks
        Task[] tasks = GenerateLogProducerTasks(_ts.Token);
        
        try
        {
            //wait for all the log writing tasks to finish
            await Task.WhenAll(tasks);
        }
        catch(AggregateException ae)
        {
            foreach(var ex in ae.Flatten().InnerExceptions)
            {
                Console.WriteLine($"Exception while running log producing threads: {ex.Message}");
            }

            _ts.Cancel();
        }

        //signal the log consumer task to stop
        _logQueue.Enqueue(null);

        try
        {
            //wait for the log consumer task to finish
            await logConsumerTask;
        }
        catch(AggregateException ae)
        {
            foreach(var ex in ae.Flatten().InnerExceptions)
            {
                Console.WriteLine($"Exception while running log consuming thread: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// builds an array of tasks that produce logs to the queue
    /// </summary>
    /// <returns></returns>
    private Task[] GenerateLogProducerTasks(CancellationToken cancellationToken)
    {
        Task[] tasks = new Task[_taskCount];

        //fill each index of the tasks array with a new task that writes logs to the queue
        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Factory.StartNew(() => LogProducerTask(cancellationToken));
        }
        
        return tasks;
    }

    /// <summary>
    /// A task that produces logs to the queue
    /// </summary>
    /// <returns></returns>
    private Task LogProducerTask(CancellationToken cancellationToken)
    {
        try
        {
            //for the number of logs to write
            for (int i = 0; i < _logCount; i++)
            {
                //check for cancellation    
                cancellationToken.ThrowIfCancellationRequested();

                lock (_lineNumberLock)
                {
                    //create a log line and enqueue it
                    string line = $"{_lineNumber++}, {Thread.CurrentThread.ManagedThreadId}, {DateTime.Now.ToString("HH:mm:ss.fff")}";
                    _logQueue.Enqueue(line);
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Exception while producing logs to queue: {ex.Message}");
            _ts.Cancel();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// A task that consumes logs from the queue and writes them to the log file
    /// </summary>
    /// <returns></returns>
    private Task LogConsumerTask(CancellationToken cancellationToken)
    {
        try
        {
            using StreamWriter writer = new(_filePath, true);

            while(true)
            {
                //check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                //dequeue a log line
                if(!_logQueue.TryDequeue(out string? log))
                {
                    //if the queue is empty, continue
                    continue;
                }

                //if the log is null, break the loop
                if(log == null)
                {
                    break;
                }

                //write the log to the file
                lock(_logQueueLock)
                {
                    writer.WriteLine(log);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while writing logs to file: {ex.Message}");
        }
        
        return Task.CompletedTask;
    }
}
