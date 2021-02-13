using System;
using NICE.Identity.TestClient.NETStandard2._0;

namespace NICE.Identity.TestClient.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("About to spin up the .NET standard 2.0 class library");

			var standardTester = new StandardTester();

			standardTester.Go();

			Console.WriteLine("Finished.");
		}
	}
}
