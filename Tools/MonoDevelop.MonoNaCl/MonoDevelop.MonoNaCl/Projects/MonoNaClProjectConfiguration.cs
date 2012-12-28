using System;
using MonoDevelop.Core;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Projects;
using System.IO;
using System.Text;

namespace MonoDevelop.MonoNaCl
{

	public class MonoNaClProjectConfiguration : DotNetProjectConfiguration
	{
		public MonoNaClProjectConfiguration ()
		: base ()
		{
		}

		public MonoNaClProjectConfiguration (string name)
		: base (name)
		{
		}
		
		public string AppFullName
		{
			get
			{
				string asm = (CompileTarget == MonoDevelop.Projects.CompileTarget.Exe) ? "main" : OutputAssembly;
				var cmp = StringComparison.OrdinalIgnoreCase;
				if (!asm.EndsWith (".exe", cmp) && !asm.EndsWith (".dll", cmp))
				{
					asm += (CompileTarget == MonoDevelop.Projects.CompileTarget.Exe) ? ".exe" : ".dll";
				}
				
				return asm;
			}
		}

		public string AppName
		{
			get
			{
				string asm = OutputAssembly;
				var cmp = StringComparison.OrdinalIgnoreCase;
				if (asm.EndsWith (".exe", cmp) || asm.EndsWith (".dll", cmp))
				{
					asm = asm.Substring (0, asm.Length - ".exe".Length);
				}
				
				return asm;
			}
		}

		public override void CopyFrom (ItemConfiguration configuration)
		{
			base.CopyFrom (configuration);
			//var cfg = configuration as MonoMacProjectConfiguration;
		}
	}
}