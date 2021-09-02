using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class LoginResult 
    {
        public LoginResultType ResultType { get; set; }
        public StudentInfo Student { get; set; }
    }

    public enum LoginResultType
    {
        Success, AlreadyLogged, InvalidStudentId, StudentNotExists, BrokenStudent, PasswordNotMatch
    }
}
