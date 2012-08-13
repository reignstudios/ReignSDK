using System;
using System.Xml;
using MonoDevelop.Core;

namespace MonoDevelop.MonoNaCl
{
	public class MonoNaClSettingsService : ICustomXmlSerializer
	{
		const string propertyName = "MonoNaCl.Settings";
		public string NACL_SDK_ROOT, NACL_MONO_ROOT;
		
		private static MonoNaClSettingsService instance;
		public static MonoNaClSettingsService Instance
		{
			get
			{
				if (instance == null)
				{
					instance = PropertyService.Get<MonoNaClSettingsService>(propertyName);
					if (instance == null) instance = new MonoNaClSettingsService();
				}
				
				return instance;
			}
		}
		
		~MonoNaClSettingsService()
		{
			PropertyService.Set(propertyName, this);
			PropertyService.SaveProperties ();
		}
	
		public ICustomXmlSerializer ReadFrom (XmlReader x)
		{
			if (!x.Read ()) return this;

			while (x.Read())
			{
				switch (x.LocalName)
				{
					case ("NACL_SDK_ROOT"):
						NACL_SDK_ROOT = x.ReadString();
						break;
						
					case ("NACL_MONO_ROOT"):
						NACL_MONO_ROOT = x.ReadString();
						break;
				}
			}

			return this;
		}
		
		public void WriteTo (XmlWriter x)
		{
			x.WriteStartElement("NACL_SDK_ROOT");
			x.WriteCData(NACL_SDK_ROOT);
			x.WriteEndElement();
			
			x.WriteStartElement("NACL_MONO_ROOT");
			x.WriteCData(NACL_MONO_ROOT);
			x.WriteEndElement();
		}
	}
}

