using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.OurException
{
    public class NoModelException : Exception
    {
        public NoModelException() { }
        public NoModelException(string message) : base(message)
        {

        }
        public NoModelException(string message, Exception inner) : base(message,inner)
        {

        }

    }
}
