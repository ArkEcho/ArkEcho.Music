﻿using System;
using System.Reflection;
using System.Threading;

namespace ArkEcho
{
    public static class EM_Exception
    {
        public static string GetFullMessage(this Exception ex)
        {
            string getExceptionTime()
            {
                object exTime = ex.Data["DateTimeInfo"];
                DateTime time = exTime != null ? (DateTime)exTime : DateTime.Now;
                return $"{time:yyyy-MM-ddTHH-mm-ss-fff}";
            }

            string result = string.Empty;
            string assName = string.Empty; ;

            Assembly ass = Assembly.GetEntryAssembly();
            if (ass != null)
                assName = ass.GetName().FullName;
            else
                assName = "UNKNOWN";

            result += $"Exception in Assembly {assName}\r\nat {getExceptionTime()}\r\n\r\n";

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
