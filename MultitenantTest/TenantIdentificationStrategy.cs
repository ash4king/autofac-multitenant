using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace MultitenantTest
{
    public class TenantIdentificationStrategy : ITenantIdentificationStrategy
    {
        public IHttpContextAccessor Accessor { get; private set; }
        public TenantIdentificationStrategy(IHttpContextAccessor httpContextAccessor)
        {
            Accessor = httpContextAccessor;
        }

        public bool TryIdentifyTenant(out object tenantId)
        {
            HttpContext context = Accessor.HttpContext;

            if(context == null)
            {
                tenantId = null;
                return false;
            }

            if(context.Items.TryGetValue("_tenantId", out tenantId))
            {
                return tenantId != null;
            }

            StringValues tenantValues;
            if (context.Request.Headers.TryGetValue("tenant", out tenantValues))
            {
                tenantId = tenantValues[0];
                context.Items["_tenantId"] = tenantId;
                return true;
            }

            tenantId = null;
            context.Items["_tenantId"] = null;
            return false;
        }
    }
}
