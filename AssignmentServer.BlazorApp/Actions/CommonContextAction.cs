using AssignmentServer.BlazorApp.Data;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Actions
{
    public abstract class CommonContextAction
    {
        protected StudentInfo currentUserInfo;
        
        public CommonContextAction(ProtectedBrowserStorage context) 
        {
            var tokenResult = context.GetAsync<string>("Authorization")
                                    .GetAwaiter().GetResult();

            if (tokenResult.Success) {
                currentUserInfo = new(tokenResult.Value);
            }
        }
    }
}
