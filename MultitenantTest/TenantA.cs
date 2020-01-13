using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultitenantTest
{
    public class TenantA : ITenantTest
    {
        public string DoAThing()
        {
            return "Greetings From Tenant A";
        }
    }
}
