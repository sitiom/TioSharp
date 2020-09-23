# TioSharp
C# library to interact asynchronously with [tio.run](https://tio.run)

`Tio` allows you to formulate valid requests for the TIO platform and
send them in an async manner.

Heavily based on [python-tio](https://github.com/FrenchMasterSword/python-tio).

## Usage

A basic example:
```cs
TioSharp site = new TioSharp();
byte[] requestData = site.CreateRequestData("cs-core", @"namespace HelloWorld
{
    class Hello {         
        static void Main(string[] args)
        {
            System.Console.WriteLine(""Hello World!"");

		}
	}
}
");
string response = await site.SendAsync(requestData);
Console.WriteLine(response);
```
```cs
Hello World!
Microsoft (R) Visual C# Compiler version 3.2.0-beta2-19303-01 (c9689b7a)
Copyright (C) Microsoft Corporation. All rights reserved.


Real time: 1.013 s
User time: 0.875 s
Sys. time: 0.121 s
CPU share: 98.31 %
Exit code: 0
```
The lib lets you configure inputs as well as compiler flags, command-line options and other arguments:
```cs
TioSharp site = new TioSharp();
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
```
```cs
Hello, sitiom!
Microsoft (R) Visual C# Compiler version 3.2.0-beta2-19303-01 (c9689b7a)
Copyright (C) Microsoft Corporation. All rights reserved.


Real time: 0.955 s
User time: 0.852 s
Sys. time: 0.115 s
CPU share: 101.21 %
Exit code: 0
```