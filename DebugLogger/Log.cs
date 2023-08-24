using System;
using System.Collections.Generic;
using System.Text;

namespace DevExpress.CodeRush.Platform.Diagnostics
{
    public static class Log
    {
        public static void SendException(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }
}
