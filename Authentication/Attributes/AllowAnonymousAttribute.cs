using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authenticate
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    {

    }
}
