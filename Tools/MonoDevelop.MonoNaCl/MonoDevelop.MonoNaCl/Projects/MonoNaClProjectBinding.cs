using System;
using MonoDevelop.Projects;

namespace MonoDevelop.MonoNaCl
{
	public class MonoNaClProjectBinding : IProjectBinding
	{
		public string Name {get{return "MonoNaCl";}}
	
		public Project CreateProject (ProjectCreateInformation info, System.Xml.XmlElement projectOptions)
		{
			string lang = projectOptions.GetAttribute ("language");
			return new MonoNaClProject (lang, info, projectOptions);
		}

		public Project CreateSingleFileProject (string sourceFile)
		{
			throw new InvalidOperationException ();
		}

		public bool CanCreateSingleFileProject (string sourceFile)
		{
			return false;
		}
	}
}