using System;
using System.Text;

namespace TestWebSocketServer
{
    internal static class Logger
    {
        public enum LogInfo
        {
            Debug,
            Info,
            Warn,
            Error,
            Fatal,
        }

        public static void Debug(string message, params object[] objects)
        {
            Log(LogInfo.Debug, string.Format(message, objects));
        }

        public static void Info(string message, params object[] objects)
        {
            Log(LogInfo.Info, string.Format(message, objects));
        }

        public static void Warn(string message, params object[] objects)
        {
            Log(LogInfo.Warn, string.Format(message, objects));
        }

        public static void Error(string message, params object[] objects)
        {
            Log(LogInfo.Error, string.Format(message, objects));
        }

        public static void Fatal(string message, params object[] objects)
        {
            Log(LogInfo.Fatal, string.Format(message, objects));
        }

        public static void Debug<T>(T anonObject) where T : class
        {
            Log(LogInfo.Debug, anonObject);
        }

        public static void Info<T>(T anonObject) where T : class
        {
            Log(LogInfo.Info, anonObject);
        }

        public static void Warn<T>(T anonObject) where T : class
        {
            Log(LogInfo.Warn, anonObject);
        }

        public static void Error<T>(T anonObject) where T : class
        {
            Log(LogInfo.Error, anonObject);
        }

        public static void Fatal<T>(T anonObject) where T : class
        {
            Log(LogInfo.Fatal, anonObject);
        }

        public static void Log<T>(LogInfo info, T anonObject) where T : class
        {
            var sb = ParseArgs(anonObject);
            Log(info, sb.ToString());
        }

        private static StringBuilder ParseArgs<T>(T args) where T : class
        {
            var sb = new StringBuilder();
            foreach (var property in typeof(T).GetProperties())
            {
                var propertyValue = property.GetValue(args, null);
                sb.AppendFormat("[{0}={1}]", property.Name, propertyValue == null ? "(null)" : propertyValue.ToString());
            }
            return sb;
        }

        private static void Log(LogInfo logInfo, string log)
        {
            switch (logInfo)
            {
                case LogInfo.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;

                case LogInfo.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case LogInfo.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogInfo.Error:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;

                case LogInfo.Fatal:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }

            Console.WriteLine(log);

            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
