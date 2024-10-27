using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace churchWebAPI.Models
{
    public class ExceptionLogger
    {
        private static readonly string logDirectoryPath = "ExceptionLogs";
        private const int LineLength = 80;

        public static void LogException(HttpContext httpContext, Exception ex)
        {
            // Create the directory if it doesn't exist
            if (!Directory.Exists(logDirectoryPath))
            {
                Directory.CreateDirectory(logDirectoryPath);
            }

            // Create a file name based on the current date
            string fileName = $"ExepLog{DateTime.Now:yyyy-MM-dd}.txt";
            string filePath = Path.Combine(logDirectoryPath, fileName);

            // Create a line of -- with the specified length
            string line = new string('-', LineLength);

            // Get the controller name from HttpContext
            string controllerName = httpContext.Items["ControllerName"] as string ?? "Unknown";
            string methodName = httpContext.Items["MethodName"] as string ?? "Unknown";

            // Format the log message with -- lines at the start and end
            string logMessage = $"{line}\n[{DateTime.Now}] \nController: {controllerName}\nMethod: {methodName}\nException: {ex.GetType()}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}\n{line}\n";

            try
            {
                // Append the log message to the file
                File.AppendAllText(filePath, logMessage);
            }
            catch (Exception e)
            {
                // Handle any issues that occur while writing to the log file
                Console.WriteLine($"Failed to log exception: {e.Message}");
            }
        }
    }
}
