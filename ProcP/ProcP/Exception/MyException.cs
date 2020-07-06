using System;

namespace ProcP
{
    public class MyException : Exception
    {
        public MyException()
        {
        }

        public MyException(string msg) : base(msg)
        {
        }
    }
}
