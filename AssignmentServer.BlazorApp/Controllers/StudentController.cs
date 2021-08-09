using AssignmentServer.BlazorApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class StudentController : ApiController
    {
        static readonly Regex StudentIdRegex = new("2[0-9]{8}", RegexOptions.Compiled);

        SessionManager _sessionManager;
        HttpContext _httpContext;
        readonly string _token;

        public StudentController(IHttpContextAccessor hca, SessionManager manager)
        {
            _httpContext = hca.HttpContext;
            _sessionManager = manager;

            _httpContext.Request.Headers.TryGetValue("Authorization", out var token);
            _token = token.ToString();
        }

        public StudentInfo Check()
        {
            var result = new StudentInfo(_token);

            return result.Valid ? result : null;
        }
        
        [HttpPost]
        public StudentInfo Login([FromBody] LoginFormData formData) 
        {
            Match match = StudentIdRegex.Match(formData.StudentId);

            // check student id format
            if (!match.Success)
                return null;

            var id = match.Value;
            var cabinet = $"Cabinet/Students/{id}/student.json";

            // check sanity
            if (!System.IO.File.Exists(cabinet))
                return null;

            var cabData = System.IO.File.ReadAllText(cabinet, Encoding.UTF8);
            var result = JsonConvert.DeserializeObject<Student>(cabData);

            if (result is null)
                return null;

            // check password
            return result.PasswordMatches(formData.Password) ?
                   new StudentInfo(result) :
                   null;
        }

        [HttpPost]
        public PasswordChangeResultType UpdatePassword([FromBody]PasswordChangeFormData formData)
        {
            var user = new StudentInfo(_token);

            if (!user.Valid)
            {
                return PasswordChangeResultType.LoginRevoked;
            }
            else if (formData.NewPassword != formData.NewPasswordConfirm) 
            {
                return PasswordChangeResultType.ConfirmFailure;
            }

            var cabinet = $"Cabinet/Students/{user.Id}/student.json";

            if (!System.IO.File.Exists(cabinet))
            {
                return PasswordChangeResultType.UserNotFound;
            }

            var cabData = System.IO.File.ReadAllText(cabinet, Encoding.UTF8);
            var result = JsonConvert.DeserializeObject<Student>(cabData);

            if (!result.PasswordMatches(formData.Password))
            {
                return PasswordChangeResultType.PasswordNotMatch;
            }

            result.ToPassword(formData.NewPassword, true, true);
            result.LastPasswordSet = DateTime.Now;

            cabData = JsonConvert.SerializeObject(result);

            System.IO.File.WriteAllText(cabinet, cabData, Encoding.UTF8);
            return PasswordChangeResultType.Success;
        }
    }
}
