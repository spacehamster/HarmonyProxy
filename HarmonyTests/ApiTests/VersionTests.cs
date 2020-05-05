using Harmony;
using NUnit.Framework;
using System.Linq;

namespace HarmonyTests.ApiTests
{
	[TestFixture]
	public class VersionTests
	{
		[Test]
		public void TestVersion()
		{
			var harmony = HarmonyInstance.Create("test");
			var version = harmony.VersionInfo(out System.Version currentVersion);
			Assert.AreEqual(currentVersion.ToString(), "2.0.0.8");
			Assert.AreEqual(version.Count, 0);
		}
	}
}
