
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace ConsoleTestRunner
{
	static class Util
	{
		public static bool ExistsOnPath(string fileName)
		{
			return GetFullPath(fileName) != null;
		}

		public static string GetFullPath(string fileName)
		{
			if (File.Exists(fileName))
				return Path.GetFullPath(fileName);

			var values = Environment.GetEnvironmentVariable("PATH");
			foreach (var path in values.Split(Path.PathSeparator))
			{
				var fullPath = Path.Combine(path, fileName);
				if (File.Exists(fullPath))
					return fullPath;
			}
			return fileName;
		}
		public static CommandResult RunWithResult(string exe, string args)
		{
			if (!exe.EndsWith(".exe")) exe += ".exe";
			var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var fullExePath = GetFullPath(exe);
			if (!File.Exists(fullExePath))
			{
				throw new ArgumentException($"Could not find exe {fullExePath} in {Environment.GetEnvironmentVariable("PATH")}");
			}
			var stdout = new StringBuilder();
			var stderr = new StringBuilder();
			Process process = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.RedirectStandardError = true;
			startInfo.RedirectStandardOutput = true;
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.FileName = fullExePath;
			startInfo.Arguments = args;
			startInfo.UseShellExecute = false;
			//startInfo.WorkingDirectory = workingDirectory;
			process.StartInfo = startInfo;
			process.OutputDataReceived += (obj, evt) => stdout.AppendLine(evt.Data);
			process.ErrorDataReceived += (obj, evt) => stdout.AppendLine(evt.Data);
			process.Start();
			process.BeginErrorReadLine();
			process.BeginOutputReadLine();
			process.WaitForExit();
			return new CommandResult()
			{
				ExitCode = process.ExitCode,
				StdOut = stdout.ToString(),
				StdErr = stderr.ToString()
			};
		}
		public static bool Run(string exe, string args)
		{
			if (!exe.EndsWith(".exe")) exe += ".exe";
			var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var fullExePath = GetFullPath(exe);
			if (!File.Exists(fullExePath))
			{
				throw new ArgumentException($"Could not find exe {fullExePath} in {Environment.GetEnvironmentVariable("PATH")}");
			}
			Process process = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.FileName = fullExePath;
			startInfo.Arguments = args;
			startInfo.UseShellExecute = false;
			//startInfo.WorkingDirectory = workingDirectory;
			process.StartInfo = startInfo;
			process.Start();
			process.WaitForExit();
			return process.ExitCode == 0;
		}
	}
}
