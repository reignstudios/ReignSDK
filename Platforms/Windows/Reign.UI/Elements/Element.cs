using System;
using System.Collections.Generic;
using Reign.Core;
using System.Collections.ObjectModel;

namespace Reign.UI
{
	public enum ElementTemplateTypes
	{
		Metro
	}

	public class Element
	{
		#region Properties
		public VisualI[] Visuals;

		private List<Element> childeren;
		public ReadOnlyCollection<Element> Childeren {get {return childeren.AsReadOnly();}}

		private ShapeI rolloverShape;
		public ShapeI RolloverShape
		{
			get {return rolloverShape;}
			set
			{
				rolloverShape = value;
				boundingBox = value.BoundingBox;
			}
		}

		private BoundingBox2 boundingBox;
		public BoundingBox2 BoundingBox {get{return boundingBox;}}

		private BoundingBox2 childerenBoundingBox;
		public BoundingBox2 ChilderenBoundingBox {get {return childerenBoundingBox;}}
		#endregion

		#region Constructors
		public Element(VisualI[] visuals, ShapeI rolloverShape)
		{
			childeren = new List<Element>();
			Visuals = visuals;
			RolloverShape = rolloverShape;
		}

		protected Element()
		{
			childeren = new List<Element>();
		}
		#endregion

		#region Methods
		public void Update()
		{
			foreach (var child in Childeren) child.Update();
		}

		public virtual void Render()
		{
			foreach (var child in Childeren) child.Render();
		}

		public void AddChild(Element childElement)
		{
			if (!childeren.Contains(childElement)) childeren.Add(childElement);
			else Debug.ThrowError("Element", "Already contains child");
			updateChilderenBoundingBox();
		}

		private void updateChilderenBoundingBox()
		{
			boundingBox = BoundingBox2.Zero;
			foreach (var child in childeren)
			{
				BoundingBox2.Merge(ref boundingBox, ref child.boundingBox, out boundingBox);
			}
		}
		#endregion
	}
}
