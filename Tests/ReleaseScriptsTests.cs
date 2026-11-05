using System.Text.RegularExpressions;

namespace At.luki0606.ClassPulse.Tests
{
    [TestFixture]
    internal sealed class ReleaseScriptsTests
    {
        private static string ReadRepositoryFile(string fileName)
        {
            DirectoryInfo? directory = new(TestContext.CurrentContext.TestDirectory);
            while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "ClassPulse.slnx")))
            {
                directory = directory.Parent;
            }

            Assert.That(directory, Is.Not.Null, "Repository root not found.");
            return File.ReadAllText(Path.Combine(directory!.FullName, fileName));
        }

        [Test]
        public void SetupScript_Always_DoesNotDefineVersion()
        {
            string script = ReadRepositoryFile("ClassPulseSetup.iss");

            Assert.That(Regex.IsMatch(script, @"^\s*#define\s+MyAppVersion\b", RegexOptions.Multiline), Is.False);
        }

        [Test]
        public void SetupScript_WithoutVersion_StopsCompilation()
        {
            string script = ReadRepositoryFile("ClassPulseSetup.iss");

            Assert.That(Regex.IsMatch(script, @"^\s*#ifndef\s+MyAppVersion\s*\r?\n\s*#error\b", RegexOptions.Multiline), Is.True);
        }

        [Test]
        public void SetupScript_Always_PackagesOutputOfProjectTargetFramework()
        {
            string project = ReadRepositoryFile(@"ClassPulse\ClassPulse.csproj");
            string script = ReadRepositoryFile("ClassPulseSetup.iss");

            string targetFramework = Regex.Match(project, "<TargetFramework>(.+)</TargetFramework>").Groups[1].Value;

            Assert.That(script, Does.Contain($@"\ClassPulse\bin\Release\{targetFramework}\win-x64\publish"));
        }

        [Test]
        public void BuildScript_Always_PassesVersionFromProjectToInstaller()
        {
            string script = ReadRepositoryFile("build.ps1");

            Assert.Multiple(() =>
            {
                Assert.That(script, Does.Contain(@"$version = dotnet msbuild ClassPulse\ClassPulse.csproj -getProperty:Version"));
                Assert.That(script, Does.Contain("iscc.exe \"/DMyAppVersion=$version\" ClassPulseSetup.iss"));
            });
        }
    }
}
