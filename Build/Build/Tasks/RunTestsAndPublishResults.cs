using System.Collections.Generic;

using Cake.Common.Build;
using Cake.Common.Build.AzurePipelines.Data;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core.IO;
using Cake.Core.IO.Arguments;
using Cake.Frosting;

using Path = System.IO.Path;

namespace Build
{
    public sealed class RunTestsAndPublishResults : FrostingTask<Context>
    {
        /// <summary>Runs the task using the specified context.</summary>
        /// <param name="context">The context.</param>
        public override void Run(Context context)
        {
            IDictionary<string, string> testProjects = new Dictionary<string, string>();
            foreach (var testProject in context.Tests.TestProjects)
            {
                testProjects.Add(
                    testProject.Key,
                    Path.Combine(context.Environment.WorkingDirectory.FullPath, testProject.Value));
            }

            try
            {
                foreach (KeyValuePair<string, string> nameAndPath in testProjects)
                {

                    context.DotNetTest(
                        nameAndPath.Value,
                        new DotNetTestSettings
                        {
                            VSTestReportPath =
                                Path.Combine(context.Environment.WorkingDirectory.FullPath,
                                    $"{context.General.ArtifactsDir}.tests", $"{nameAndPath.Key}.TestResult.xml"),
                            Configuration = context.Tests.BuildConfig,
                            ArgumentCustomization = delegate (ProcessArgumentBuilder argument)
                            {
                                argument.Append(new TextArgument(" /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura"));
                                return argument;
                            }
                        });
                }
            }
            finally
            {
                if (!context.BuildSystem().IsLocalBuild)
                {
                    foreach (KeyValuePair<string, string> nameAndPath in testProjects)
                    {
                        context.AzurePipelines().Commands.PublishTestResults(
                            new AzurePipelinesPublishTestResultsData
                            {
                                TestResultsFiles =
                                    new List<FilePath>
                                    {
                                        Path.Combine(context.Environment.WorkingDirectory.FullPath,
                                            $"{context.General.ArtifactsDir}.tests",
                                            $"{nameAndPath.Key}.TestResult.xml")
                                    },
                                TestRunner = AzurePipelinesTestRunnerType.VSTest
                            });
                    }
                }
            }
        }
    }
}
