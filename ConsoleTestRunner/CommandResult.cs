using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTestRunner
{
	public class CommandResult
	{
		public int ExitCode;
		public string StdOut;
		public string StdErr;
		public override string ToString()
		{
			return $"ExitCode: {ExitCode}\nstdout: {StdOut}\nStdErr: {StdErr}";
		}
	}
}
