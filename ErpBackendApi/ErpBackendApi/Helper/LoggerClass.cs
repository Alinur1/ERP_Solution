using System;
using System.IO;

namespace ErpBackendApi.Helper
{
    public static class LoggerClass
    {
        private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ERP_API_Logger.txt");

        public static void Logger(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
                {
                    writer.WriteLine($"[{DateTime.Now:dd-MM-yyyy hh:mmtt}] {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logger failed: {ex.Message}");
            }
        }
    }
}
