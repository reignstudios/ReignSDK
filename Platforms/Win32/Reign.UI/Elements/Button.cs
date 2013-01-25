using System;
using Reign.Core;

namespace Reign.UI
{
	public class Button : Element
	{
		public Button(ElementTemplateTypes templateType)
		{
			switch (templateType)
			{
				case (ElementTemplateTypes.Metro):
					
					break;

				default:
					Debug.ThrowError("Button", "Unsuported Template Type: " + templateType.ToString());
					break;
			}
		}
	}
}
