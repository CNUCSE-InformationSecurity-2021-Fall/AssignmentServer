using AssignmentServer.BlazorApp.Data;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        private Process SpawnDocker(string instanceId, string language, string code, string input, int memoryLimit, Action<CodeTesterBackgroundReport> reporter)
        {
            var outputBuilder = new StringBuilder();

            Directory.CreateDirectory($"/code/{instanceId}");
            File.WriteAllText($"/code/{instanceId}/code", code);

            var process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                Arguments = $"run -v /code/{instanceId}:/code-runner -i " +
                            $"--name={instanceId} --memory={memoryLimit}m " +
                            $"coderunner:release /exec.sh {language} {instanceId}",
                FileName = "/usr/bin/docker"
            };

            process.OutputDataReceived += (e, args) => {
                if (args.Data is null)
                    return;
                
                if (reporter is not null)
                    reporter(new()
                    {
                        Type = CodeTesterReportType.OutputReceived,
                        Output = args.Data,
                        ExitCode = -1,
                        Miliseconds = -1
                    });

                outputBuilder.Append(args.Data);
            };

            process.Exited += (e, args) =>
            {
                if (reporter is not null)
                    reporter(new()
                    {
                        Type = CodeTesterReportType.ExitReport,
                        Output = outputBuilder.ToString(),
                        ExitCode = process.ExitCode,
                        Miliseconds = (process.ExitTime - process.StartTime).Milliseconds
                    });

                Process.Start(new ProcessStartInfo()
                {
                    FileName = "/usr/bin/docker",
                    Arguments = $"rm {instanceId}"
                });

                process.CancelOutputRead();
            };

            process.Start();
            process.BeginOutputReadLine();

            process.StandardInput.WriteLine();
            process.StandardInput.Close();

            return process;
        }

        public void ExecuteCode(string language, string code, Action<CodeTesterBackgroundReport> reporter)
        {
            SpawnDocker(Guid.NewGuid().ToString(), language, code, TestInfo.InputExample, TestInfo.MemoryLimit, reporter);
        }

        public void SubmitCode(string language, string code, Action<CodeTestSubmitResult> reporter)
        {
            var totalCases = TestInfo.Testcases.Count;
            var acceptedCases = 0;
            var runningTimeAvg = new List<int>();

            foreach (var testCase in TestInfo.Testcases)
            {
                var dockerProcess = SpawnDocker(
                    Guid.NewGuid().ToString(),
                    language, code, testCase.Input, TestInfo.MemoryLimit, 
                    report =>
                    {
                        if (report.Type == CodeTesterReportType.ExitReport)
                        {
                            if (testCase.Output == report.Output.Trim())
                            {
                                acceptedCases++;
                                runningTimeAvg.Add(report.Miliseconds);
                            }
                        }
                    });
                dockerProcess.WaitForExit();
            }

            var score = (int)Math.Round(10 * (acceptedCases / (double)totalCases));
            reporter(new CodeTestSubmitResult()
            {
                AcceptedCases = acceptedCases,
                TotalCases = totalCases,
                AverageRunningTime = (int)Math.Round(runningTimeAvg.Average()),
                Score = score
            });
        }
    }
}
