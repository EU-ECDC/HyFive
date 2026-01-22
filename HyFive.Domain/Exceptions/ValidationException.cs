using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public string Code { get; }
        public object[] Args { get; }

        public ValidationException(string code, params object[] args)
            : base(code)
        {
            Code = code;
            Args = args;
        }
    }
}
