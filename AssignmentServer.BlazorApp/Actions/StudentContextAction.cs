using AssignmentServer.BlazorApp.Data;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Actions
{
    public class StudentContextAction : CommonContextAction
    {
        static readonly Regex StudentIdRegex = new("2[0-9]{8}", RegexOptions.Compiled);
        static readonly SessionManager sessionManager; 

        static StudentContextAction() {
            sessionManager = new SessionManager();
        }

        public StudentContextAction(ProtectedBrowserStorage context) : base(context) 
        { 
            
        }

        public async Task<StudentInfo> Check() 
        {
            var request = await storage.GetAsync<string>("Authorization");

            if (request.Success)
            {
                return new StudentInfo(request.Value);
            }

            return null;
        }

        public async Task<LoginResult> Login(LoginFormData formData)
        {
            var result = new LoginResult();
            var loginStatus = await Check();

            if (loginStatus is not null && loginStatus.Valid)
            {
                result.ResultType = LoginResultType.AlreadyLogged;
                return result;
            }

            Match match = StudentIdRegex.Match(formData.StudentId);

            // check student id format
            if (!match.Success)
            {
                result.ResultType = LoginResultType.InvalidStudentId;
                return result;
            }

            var id = match.Value;
            var cabinet = $"Cabinet/Students/{id}/student.json";

            // check sanity
            if (!File.Exists(cabinet))
            {
                result.ResultType = LoginResultType.StudentNotExists;
                return result;
            }

            var cabData = File.ReadAllText(cabinet);
            var student = JsonConvert.DeserializeObject<Student>(cabData);

            if (student is null)
            {
                result.ResultType = LoginResultType.BrokenStudent;
            }
            else if (!student.PasswordMatches(formData.Password))
            {
                result.ResultType = LoginResultType.PasswordNotMatch;
            }
            else
            {
                result.ResultType = LoginResultType.Success;
                result.Student = new StudentInfo(student);
            }

            return result;
        }

        public async Task<PasswordChangeResultType> UpdatePassword(PasswordChangeFormData formData)
        {
            var currentUserInfo = await Check();

            if (currentUserInfo is null || !currentUserInfo.Valid)
            {
                return PasswordChangeResultType.LoginRevoked;
            }
            else if (formData.NewPassword != formData.NewPasswordConfirm)
            {
                return PasswordChangeResultType.ConfirmFailure;
            }

            var cabinet = $"Cabinet/Students/{currentUserInfo.Id}/student.json";

            if (!File.Exists(cabinet))
            {
                return PasswordChangeResultType.UserNotFound;
            }

            var cabData = File.ReadAllText(cabinet, Encoding.UTF8);
            var result = JsonConvert.DeserializeObject<Student>(cabData);

            if (!result.PasswordMatches(formData.Password))
            {
                return PasswordChangeResultType.PasswordNotMatch;
            }

            result.ToPassword(formData.NewPassword, true, true);
            result.LastPasswordSet = DateTime.Now;

            cabData = JsonConvert.SerializeObject(result);

            File.WriteAllText(cabinet, cabData, Encoding.UTF8);
            return PasswordChangeResultType.Success;
        }
    }
}
