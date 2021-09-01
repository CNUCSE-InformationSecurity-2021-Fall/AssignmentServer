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
        protected ProtectedBrowserStorage storage;
        protected StudentInfo currentUserInfo;
        
        public CommonContextAction(ProtectedBrowserStorage context) 
        {
            storage = context;
        }
    }
}
