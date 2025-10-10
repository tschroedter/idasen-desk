using System;

namespace Idasen.SystemTray.Win11.Utils.Exceptions
{
    public class CustomUnhandledException : Exception
    {
        public CustomUnhandledException(string message) : base(message)
        {
        }
    }
}