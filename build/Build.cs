using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = new[] {"main"},
    InvokedTargets = new[] { nameof(Test) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    [Solution] readonly Solution Solution;

    public static int Main () => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Restore => _ => _
        .Executes(() =>
        {
          DotNetRestore(_ => _
            .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
          DotNetBuild(_ => _
            .SetProjectFile(Solution)
            .EnableNoRestore());
        });

    Target Test => _ => _
      .DependsOn(Compile)
      .Executes(() =>
      {
        DotNetTest();
      });
}
