using System.IO;

using Build.Common.Enums;
using Build.Common.Extensions;

using Cake.Common.Tools.MSBuild;
using Cake.Common.Tools.DotNet;
using Cake.Frosting;

namespace Build
{
    public sealed class Build : FrostingTask<Context>
    {
        public override void Run(Context context)
        {
            string versionPrefix, versionSuffix;
            foreach (var projectToBuild in context.Lib)
            {
                if (!context.General.IsLocal && context.General.CurrentBranch == Branches.Main)
                {
                    versionPrefix =
                        $"{projectToBuild.ArtifactVersion.Major}.{projectToBuild.ArtifactVersion.Minor}.{projectToBuild.ArtifactVersion.Patch}";
                    versionSuffix = string.Empty;
                }
                else
                {
                    versionPrefix = $"{projectToBuild.ArtifactVersion.Major}.{projectToBuild.ArtifactVersion.Minor}.{projectToBuild.ArtifactVersion.Patch}";
                    versionSuffix = $"{projectToBuild.ArtifactVersion.Prerelease}-{projectToBuild.ArtifactVersion.Build}";
                }
                context.DotNetBuild(Path.Combine(context.Environment.WorkingDirectory.FullPath, projectToBuild.MainProject),
                    new Cake.Common.Tools.DotNet.Build.DotNetBuildSettings()
                    {
                        Configuration = "Release",
                        OutputDirectory = "./.artifacts",
                        Verbosity = DotNetVerbosity.Normal,
                        VersionSuffix = versionSuffix
                    });
            }
        }
    }
}
