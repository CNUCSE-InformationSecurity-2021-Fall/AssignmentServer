using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JWT.Algorithms;
using JWT.Builder;
using Newtonsoft.Json;

namespace AssignmentServer.BlazorApp.Data
{
    public class StudentInfo
    {
        // empty constructor for JsonConvert compatibility
        public StudentInfo() { }

        // generate student info with new jwt
        public StudentInfo(Student student)
        {
            Id = student.Id;
            Name = student.Name;
            Token = JwtBuilder.Create()
                              .WithAlgorithm(new HMACSHA256Algorithm())
                              .WithSecret(Program.JwtSecret)
                              .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(2).ToUnixTimeSeconds())
                              .AddClaim("studentId", student.Id)
                              .AddClaim("name", student.Name)
                              .AddClaim("passwordChanged", student.PasswordChanged)
                              .Encode();
            PasswordChanged = student.PasswordChanged;
            Valid = Token is not null;
        }

        // generate student info from jwt
        // invalid then valid = false
        public StudentInfo(string jwt)
        {
            try
            {
                var decodedToken = JwtBuilder.Create()
                                      .WithAlgorithm(new HMACSHA256Algorithm())
                                      .WithSecret(Program.JwtSecret)
                                      .MustVerifySignature()
                                      .Decode(jwt);
                var result = JsonConvert.DeserializeObject<JwtVerifyResult>(decodedToken);

                if (/* TODO: check token revoked in sessionmanager && */ 
                    DateTimeOffset.UtcNow > DateTimeOffset.FromUnixTimeSeconds(result.exp)) 
                {
                    throw new Exception();
                }

                Id    = result.studentId;
                Name  = result.name;
                PasswordChanged = result.passwordChanged;

                Valid = true;
            }
            catch
            {
                Valid = false;
            }
        }

        public string Id    { get; set; }
        public string Name  { get; set; }
        public string Token { get; set; }

        public List<AssignmentNavViewData> AssignmentStatus
        {
            get
            {
                if (!Valid)
                    return null;

                var result = new List<AssignmentNavViewData>();
                var directories = Directory.GetDirectories("Cabinet/Assignments");

                foreach (var directory in directories) 
                {
                    var files = Directory.GetFiles(directory);
                    
                    if (files.Where(file => file.EndsWith("meta.json")).Any())
                    {
                        var file = File.ReadAllText(directory + "/meta.json");
                        var assignment = JsonConvert.DeserializeObject<AssignmentNavViewData>(file);

                        if (assignment is null || !assignment.Visible)
                            continue;

                        var submitPath = $"Cabinet/Students/{Id}/Assignments/{assignment.Id}.json";

                        if (File.Exists(submitPath))
                        {
                            var submitData = File.ReadAllText(submitPath);
                            var submit = JsonConvert.DeserializeObject<StudentSubmit>(submitData);

                            assignment.Submitted = submit.Timestamp.Any();
                            assignment.LastSubmitDate = submit.Timestamp.LastOrDefault();
                        }
                        else
                        {
                            assignment.Submitted = false;
                        }

                        result.Add(assignment);
                    }
                }

                return result;
            }
        }

        public bool   PasswordChanged { get; set; }
        public bool   Valid { get; set; }
    }
}
