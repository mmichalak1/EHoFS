using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.OurException
{
    public class NoColliderException:Exception
    {
        public NoColliderException() { }
        public NoColliderException(string message) : base(message) { }
        public NoColliderException(string message, Exception inner): base(message, inner) { }
    }
}
