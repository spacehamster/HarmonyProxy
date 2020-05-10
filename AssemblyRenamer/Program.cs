using CommandLine;
using dnlib.DotNet;
using System.IO;
using System.Linq;

namespace AssemblyRenamer
{
	class Program
	{
		public class Options
		{
			[Option('f', "find", Required = false, HelpText = "Modify the namespace by replacing find value with replace value")]
			public string NamespaceFind { get; set; }
			[Option('r', "replace", Required = false, HelpText = "Modify the namespace by replacing find value with replace value")]
			public string NamespaceReplace { get; set; }
			[Option('a', "assembly-name", Required = true, HelpText = "The name for the output assembly.")]
			public string AssemblyName { get; set; }
			[Option('p', "target-path", Required = true, HelpText = "Set output to verbose messages.")]
			public string TargetPath { get; set; }
		}
		static void RenameAssembly(Options o)
		{
			var fromNamespace = o.NamespaceFind;
			var toNamespace = o.NamespaceReplace;
			var newAssemblyName = o.AssemblyName;
			var moduleName = o.TargetPath;
			var newModuleName = Path.Combine(Path.GetDirectoryName(o.TargetPath), $"{o.AssemblyName}.dll"); 

			var mod = ModuleDefMD.Load(moduleName);
			mod.Assembly.Name = newAssemblyName;
			//Note: fixing AssemblyTitleAttribute is optional
			var title = mod.Assembly.CustomAttributes.FirstOrDefault(
				a => a.TypeFullName == "System.Reflection.AssemblyTitleAttribute");
			if (title != null)
			{
				var arg = title.ConstructorArguments[0];
				arg.Value = newAssemblyName;
				title.ConstructorArguments[0] = arg;
			}
			foreach (dnlib.DotNet.TypeDef type in mod.Types)
			{
				type.Namespace = type.Namespace.Replace(fromNamespace, toNamespace);
			}
			mod.Write(newModuleName);
		}
		static void Main(string[] args)
		{
			Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
			{
				RenameAssembly(o);
			});
		}
	}
}
