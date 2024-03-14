namespace SdrsDecoder.Support
{
    using System;
    using System.IO;

    public static class Log
    {
        public static void LogException(Exception exception)
        {
            try
            {
                var date = DateTime.Now;

                var filename = $"SdrSharpPocsagPlugin-{date.ToString("yyyy-MM-dd")}.log";

                var currentException = exception;

                while (currentException != null)
                {
                    File.AppendAllText(filename, $"[{date.ToString()}] {exception.Message}\r\n{exception.StackTrace}\r\n");

                    currentException = currentException.InnerException;
                }
            }
            catch
            {
                // abandon all hope, ye who enter here
            }
        }
    }
}
