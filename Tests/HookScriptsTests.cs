using System.Diagnostics;
using System.Text.RegularExpressions;

namespace At.luki0606.ClassPulse.Tests
{
    [TestFixture]
    internal sealed class HookScriptsTests
    {
        private string _messageFile = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _messageFile = Path.Combine(Path.GetTempPath(), $"classpulse-commit-message-{Guid.NewGuid()}.txt");
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_messageFile);
        }

        private static string FindRepositoryRoot()
        {
            DirectoryInfo? directory = new(TestContext.CurrentContext.TestDirectory);
            while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "ClassPulse.slnx")))
            {
                directory = directory.Parent;
            }

            Assert.That(directory, Is.Not.Null, "Repository root not found.");
            return directory!.FullName;
        }

        private static int RunScript(string scriptName, params string[] arguments)
        {
            ProcessStartInfo startInfo = new("powershell")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-ExecutionPolicy");
            startInfo.ArgumentList.Add("Bypass");
            startInfo.ArgumentList.Add("-File");
            startInfo.ArgumentList.Add(Path.Combine(FindRepositoryRoot(), "scripts", "hooks", scriptName));
            foreach (string argument in arguments)
            {
                startInfo.ArgumentList.Add(argument);
            }

            using Process process = Process.Start(startInfo)!;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            return process.ExitCode;
        }

        [TestCase("classpulse.db")]
        [TestCase("CLASSPULSE.DB")]
        [TestCase("Tests/data/copy.sqlite")]
        [TestCase("classpulse.db-wal")]
        [TestCase("classpulse.db.old")]
        [TestCase(@"backups\students.bak")]
        [TestCase("export/grades.csv")]
        [TestCase("export/grades.xlsx")]
        public void DataFilesScript_WithDataFile_Fails(string path)
        {
            int exitCode = RunScript("Test-DataFiles.ps1", "README.md", path);

            Assert.That(exitCode, Is.EqualTo(1));
        }

        [Test]
        public void DataFilesScript_WithOtherFiles_Succeeds()
        {
            int exitCode = RunScript("Test-DataFiles.ps1", "README.md", "ClassPulse/Data/AppDbContext.cs", "docs/data.md");

            Assert.That(exitCode, Is.EqualTo(0));
        }

        [TestCase("ISSUE 33 build: Add git hooks")]
        [TestCase("ISSUE 1 fix: Correct the average")]
        [TestCase("Merge branch 'main' into issue/33")]
        [TestCase("Revert \"ISSUE 33 build: Add git hooks\"")]
        public void CommitMessageScript_WithAllowedMessage_Succeeds(string message)
        {
            File.WriteAllText(_messageFile, message + "\n\nBody text.\n");

            int exitCode = RunScript("Test-CommitMessage.ps1", _messageFile);

            Assert.That(exitCode, Is.EqualTo(0));
        }

        [TestCase("Add git hooks")]
        [TestCase("docs: Link entries")]
        [TestCase("issue 33 build: Add git hooks")]
        [TestCase("ISSUE build: Add git hooks")]
        [TestCase("ISSUE 33 style: Add git hooks")]
        [TestCase("ISSUE 33 build:")]
        public void CommitMessageScript_WithInvalidMessage_Fails(string message)
        {
            File.WriteAllText(_messageFile, message + "\n");

            int exitCode = RunScript("Test-CommitMessage.ps1", _messageFile);

            Assert.That(exitCode, Is.EqualTo(1));
        }

        [Test]
        public void PreCommitConfig_Always_UsesOnlyWhitespaceFormatting()
        {
            string config = File.ReadAllText(Path.Combine(FindRepositoryRoot(), ".pre-commit-config.yaml"));

            Assert.Multiple(() =>
            {
                Assert.That(config, Does.Contain("entry: dotnet format whitespace "));
                Assert.That(Regex.IsMatch(config, @"^\s*entry:\s*dotnet format(?! whitespace )", RegexOptions.Multiline), Is.False);
            });
        }

        [Test]
        public void CiWorkflow_Always_ChecksTrackedFilesForDataFiles()
        {
            string workflow = File.ReadAllText(Path.Combine(FindRepositoryRoot(), ".github", "workflows", "ci.yml"));

            Assert.That(workflow, Does.Contain("scripts/hooks/Test-DataFiles.ps1 @(git -c core.quotepath=off ls-files)"));
        }
    }
}
