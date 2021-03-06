﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.CodeAnalysis.Tools.Tests
{
    public class CodeFormatterTests : IClassFixture<MSBuildFixture>
    {
        private static string SolutionPath => Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.Parent.FullName;

        public CodeFormatterTests(MSBuildFixture msBuildFixture)
        {
            msBuildFixture.RegisterInstance();
        }

        [Fact]
        public async Task NoFilesFormattedInFormattedProject()
        {
            var logger = new TestLogger();
            var path = Path.GetFullPath("tests/projects/for_code_formatter/formatted_project/formatted_project.csproj", SolutionPath);

            var exitCode = await CodeFormatter.FormatWorkspaceAsync(logger, path, isSolution: false, logAllWorkspaceWarnings: false, saveFormattedFiles: false, cancellationToken: CancellationToken.None);
            var log = logger.GetLog();
            var pattern = string.Format(Resources.Formatted_0_of_1_files_in_2_ms, "(\\d+)", "\\d+", "\\d+");
            var filesFormatted = new Regex(pattern, RegexOptions.Multiline);
            var match = filesFormatted.Match(log);

            Assert.True(match.Success, log);
            Assert.Equal("0", match.Groups[1].Value);
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public async Task NoFilesFormattedInFormattedSolution()
        {
            var logger = new TestLogger();
            var path = Path.GetFullPath("tests/projects/for_code_formatter/formatted_solution/formatted_solution.sln", SolutionPath);

            var exitCode = await CodeFormatter.FormatWorkspaceAsync(logger, path, isSolution: true, logAllWorkspaceWarnings: false, saveFormattedFiles: false, cancellationToken: CancellationToken.None);
            var log = logger.GetLog();
            var pattern = string.Format(Resources.Formatted_0_of_1_files_in_2_ms, "(\\d+)", "\\d+", "\\d+");
            var filesFormatted = new Regex(pattern, RegexOptions.Multiline);
            var match = filesFormatted.Match(log);

            Assert.Equal(0, exitCode);
            Assert.True(match.Success, log);
            Assert.Equal("0", match.Groups[1].Value);
        }

        [Fact]
        public async Task FilesFormattedInUnformattedProject()
        {
            var logger = new TestLogger();
            var path = Path.GetFullPath("tests/projects/for_code_formatter/unformatted_project/unformatted_project.csproj", SolutionPath);

            var exitCode = await CodeFormatter.FormatWorkspaceAsync(logger, path, isSolution: false, logAllWorkspaceWarnings: false, saveFormattedFiles: false, cancellationToken: CancellationToken.None);
            var log = logger.GetLog();
            var pattern = string.Format(Resources.Formatted_0_of_1_files_in_2_ms, "(\\d+)", "\\d+", "\\d+");
            var filesFormatted = new Regex(pattern, RegexOptions.Multiline);
            var match = filesFormatted.Match(log);

            Assert.True(match.Success, log);
            Assert.Equal("1", match.Groups[1].Value);
            Assert.Equal(0, exitCode);
        }

        [Fact]
        public async Task FilesFormattedInUnformattedSolution()
        {
            var logger = new TestLogger();
            var path = Path.GetFullPath("tests/projects/for_code_formatter/unformatted_solution/unformatted_solution.sln", SolutionPath);

            var exitCode = await CodeFormatter.FormatWorkspaceAsync(logger, path, isSolution: true, logAllWorkspaceWarnings: false, saveFormattedFiles: false, cancellationToken: CancellationToken.None);
            var log = logger.GetLog();
            var pattern = string.Format(Resources.Formatted_0_of_1_files_in_2_ms, "(\\d+)", "\\d+", "\\d+");
            var filesFormatted = new Regex(pattern, RegexOptions.Multiline);
            var match = filesFormatted.Match(log);

            Assert.Equal(0, exitCode);
            Assert.True(match.Success, log);
            Assert.Equal("1", match.Groups[1].Value);
        }

        [Fact]
        public async Task FSharpProjectsDoNotCreateException()
        {
            var logger = new TestLogger();
            var path = Path.GetFullPath("tests/projects/for_code_formatter/fsharp_project/fsharp_project.fsproj", SolutionPath);

            var exitCode = await CodeFormatter.FormatWorkspaceAsync(logger, path, isSolution: false, logAllWorkspaceWarnings: false, saveFormattedFiles: false, cancellationToken: CancellationToken.None);
            var logLines = logger.GetLog().Split(Environment.NewLine);

            Assert.Equal(4, logLines.Length);
            var actualErrorMessage = logLines[2];
            var expectedErrorMessage = string.Format(Resources.Could_not_format_0_Format_currently_supports_only_CSharp_and_Visual_Basic_projects, path);
            Assert.Equal(expectedErrorMessage, actualErrorMessage);
            Assert.Equal(1, exitCode);
        }
    }
}
