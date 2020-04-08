using NUnit.Framework;
using System;
using System.Text;

namespace ConsoleTestRunner
{
	[TestFixture]
	class AllTests
	{
		[TestCase]
		public void Run()
		{
			var args = new StringBuilder();
			args.Append("HarmonyTests.dll");
			var result = Util.RunWithResult("nunit3-console", args.ToString());
			Assert.True(result.ExitCode == 0, $"Version {Environment.Version}\n" + result.StdOut);
		}
	}
}
