using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ArkEcho
{
    public static class EM_Exception
    {
        public static string FullMessage(this Exception ex)
        {
            string getExceptionTime()
            {
                object exTime = ex.Data["DateTimeInfo"];
                DateTime time = exTime != null ? (DateTime)exTime : DateTime.Now;
                return $"{time:yyyy-MM-ddTHH-mm-ss-fff}";
            }

            string result = string.Empty;

            result += $"Exception in Assembly {Assembly.GetEntryAssembly().GetName().FullName}\r\nat {getExceptionTime()}\r\n\r\n";

            Exception exLevel = ex;
            int level = 0;

            do
            {
                result += $"Level #{level}\r\n";
                result += $"\t{exLevel.Message}(\"{exLevel.GetType().FullName}\")\r\n";
                result += $"\t{exLevel.StackTrace}\r\n";

                exLevel = exLevel.InnerException;
                level++;
            }
            while (exLevel != null);

            return result;
        }

        public static bool IsCritical(this Exception ex)
        {
            if (ex is OutOfMemoryException || ex is AppDomainUnloadedException || ex is BadImageFormatException
            || ex is CannotUnloadAppDomainException || ex is InvalidProgramException || ex is ThreadAbortException)
                return true;
            return false;
        }
    }
}
