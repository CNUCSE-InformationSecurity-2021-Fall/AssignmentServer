﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public enum PasswordChangeResultType
    {
        UserNotFound, LoginRevoked, PasswordNotMatch, ConfirmFailure, Success
    }
}
