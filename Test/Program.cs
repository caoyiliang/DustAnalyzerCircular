// See https://aka.ms/new-console-template for more information
using DustAnalyzerCircular;

Console.WriteLine("Hello, World!");
IDustAnalyzerCircular dustAnalyzerCircular = new DustAnalyzerCircular.DustAnalyzerCircular(new Communication.Bus.PhysicalPort.SerialPort("COM3", 38400));
await dustAnalyzerCircular.OpenAsync();
var rs = await dustAnalyzerCircular.Read("01");

Console.ReadLine();