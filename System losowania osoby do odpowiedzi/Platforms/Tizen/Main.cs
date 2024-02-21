using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace System_losowania_osoby_do_odpowiedzi;

class Program : MauiApplication
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	static void Main(string[] args)
	{
		var app = new Program();
		app.Run(args);
	}
}
