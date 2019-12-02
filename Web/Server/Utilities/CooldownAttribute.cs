using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Web.Server.Utilities
{
    public class CooldownAttribute : ActionFilterAttribute
    {
        public double Cooldown { get; set; }
        private readonly Dictionary<string, DateTime> _cooldowns;

        public CooldownAttribute()
        {
            _cooldowns = new Dictionary<string, DateTime>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HasCooldown(context.HttpContext.User.FindFirst(ClaimTypes.Name).Value, out int leftSeconds))
            {
                context.HttpContext.Response.Headers.Add("cooldown", leftSeconds.ToString());
                context.Result = new BadRequestResult();
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!HasCooldown(context.HttpContext.User.FindFirst(ClaimTypes.Name).Value, out int leftSeconds))
            {
                _cooldowns[context.HttpContext.User.FindFirst(ClaimTypes.Name).Value] = DateTime.Now.AddSeconds(Cooldown);
            }
        }

        public bool HasCooldown(string userId, out int leftSeconds)
        {
            leftSeconds = 0;
            if (_cooldowns.TryGetValue(userId, out DateTime value))
            {  
                leftSeconds = (int)(value - DateTime.Now).TotalSeconds;
                if (leftSeconds > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
