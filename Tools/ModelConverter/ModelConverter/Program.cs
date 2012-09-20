using System;
using Reign.Core;

namespace ModelConverter
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			OS.Run(new MainWindow(), UpdateAndRenderModes.Stepping, 60);
		}
	}
}
