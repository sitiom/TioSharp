using System;
using System.Threading.Tasks;

namespace TioSharp.Example
{
	class Program
	{
		private static async Task Main(string[] args)
		{
			TioApi site = new TioApi();
			byte[] requestData = site.CreateRequestData("cs-core", @"namespace HelloWorld
{
	class Hello {         
		static void Main(string[] args)
		{
			string name = System.Console.ReadLine();
			System.Console.WriteLine($""Hello, {name}!"");

		}
	}
}
", new[] { "sitiom" });
			string response = await site.SendAsync(requestData);

			Console.WriteLine(response);
		}
	}
}
