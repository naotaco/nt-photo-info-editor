using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NtPhotoInfoEditor.Utils
{
    public class Logger
    {
        public static void Log(string s)
        {
            Debug.WriteLine(s);
        }
    }
}
