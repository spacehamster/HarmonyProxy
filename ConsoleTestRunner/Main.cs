using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ConsoleTestRunner
{
	[TestFixture]
	public class Main
	{
		private static IEnumerable<TestCaseData> TestSource
		{
			get
			{
				Directory.SetCurrentDirectory(
					Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
				Util.Run("nunit3-console", "--explore:tests.txt;format=cases HarmonyTests.dll");
				var tests = File.ReadAllLines("tests.txt");
				foreach (var test in tests)
				{
					yield return new TestCaseData(test).SetName(test);
				}
			}
		}
		[TestCaseSource("TestSource")]
		public void Test(string testCase)
		{
			var args = new StringBuilder();
			args.Append("HarmonyTests.dll");
			args.Append($" --test={ testCase}");
			var result = Util.RunWithResult("nunit3-console", args.ToString());
			if (result.StdOut.Contains("Overall result: Warning"))
			{
				Warn.If(true, result.StdOut);
			}
			else
			{
				var version = Util.GetFrameworkVersion(Assembly.GetExecutingAssembly());
				Assert.True(result.ExitCode == 0, $"Version {version}\n" + result.StdOut);
			}
		}
	}
}