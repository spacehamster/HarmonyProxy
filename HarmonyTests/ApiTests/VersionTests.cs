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
			Assert.AreEqual(currentVersion.Major, 2);
			Assert.AreEqual(currentVersion.Minor, 0);
			Assert.AreEqual(version.Count, 0);
		}
	}
}
