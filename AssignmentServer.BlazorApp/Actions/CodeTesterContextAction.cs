using AssignmentServer.BlazorApp.Data;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Actions
{
    public class CodeTesterContextAction
    {
        private string AssignmentId { get; set; }
        private StudentInfo Student { get; set; }

        public CodeTestInfo TestInfo { get; set; }

        public CodeTesterContextAction(string assignmentId, StudentInfo student)
        {
            AssignmentId = assignmentId;
            Student = student?.Valid == true ? student : null;

            LoadAssignmentDetail();
        }

        private void AbandonContext()
        {
            AssignmentId = null;
            Student = null;
        }

        private void LoadAssignmentDetail()
        {
            var cabinetPath = $"Cabinet/Assignments/{AssignmentId}";
            var assMeta = cabinetPath + "/meta.json";

            if (!File.Exists(assMeta))
            {
                AbandonContext();
                return;
            }

            var json = File.ReadAllText(assMeta);
            var obj = JsonConvert.DeserializeObject<Assignment>(json);

            if (obj.Application != "App_CodeTester")
            {
                AbandonContext();
                return;
            }

            var assCodeMeta = cabinetPath + "/codetester.json";

            if (!File.Exists(assCodeMeta))
            {
                AbandonContext();
                return;
            }

            json = File.ReadAllText(assCodeMeta);
            TestInfo = JsonConvert.DeserializeObject<CodeTestInfo>(json);

            if (TestInfo is null)
            {
                AbandonContext();
            }
        }

        private Process SpawnDocker(string language, string input, int memoryLimit, Action<CodeTesterBackgroundReport> reporter)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                Arguments = $"-it --memory={memoryLimit}m code-runner-{language}",
                FileName = "/usr/bin/docker"
            };

            process.OutputDataReceived += (e, args) => {
                if (args.Data is null)
                    return;

                reporter(new()
                {
                    Type = CodeTesterReportType.OutputReceived,
                    Output = args.Data,
                    ExitCode = -1,
                    Miliseconds = -1
                });
            };

            process.Exited += (e, args) =>
            {
                reporter(new()
                {
                    Type = CodeTesterReportType.ExitReport,
                    Output = null,
                    ExitCode = process.ExitCode,
                    Miliseconds = (process.ExitTime - process.StartTime).Milliseconds
                });
            };

            process.Start();

            process.StandardInput.WriteLine(input);
            process.StandardInput.Close();

            return process;
        }

        public void ExecuteCode(string language, string code, Action<CodeTesterBackgroundReport> reporter)
        {
            SpawnDocker(language, code, TestInfo.MemoryLimit, reporter);
        }

        public void SubmitCode(string language, string code, Action reporter)
        {
            var totalCases = TestInfo.Testcases.Count;

        }
    }
}
