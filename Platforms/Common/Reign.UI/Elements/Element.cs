using System;
using System.Collections.Generic;
using Reign.Core;
using System.Collections.ObjectModel;
using Reign.Input;
using Reign.Video;

namespace Reign.UI
{
	public enum ElementStates
	{
		None,
		MouseEnter,
		MouseExit,
		MouseOver
	}

	public enum HorizontalAlignments
	{
		Left,
		Right,
		Center
	}

	public enum VerticalAlignments
	{
		Bottom,
		Top,
		Center
	}

	public abstract class Element
	{
		#region Properties
		protected UI ui;
		public bool Enabled;
		private ElementStates currentState, lastState;

		public VisualI[] Visuals {get; private set;}
		public ShapeI RolloverShape {get; private set;}

		private List<Element> childeren;
		public ReadOnlyCollection<Element> Childeren {get {return childeren.AsReadOnly();}}

		public HorizontalAlignments HorizontalAlignment;
		public VerticalAlignments VerticalAlignment;
		public bool CenterX, CenterY, AutoScalePositionX, AutoScalePositionY, AutoScaleWidth, AutoScaleHeight;

		public Point2 Position
		{
			get {return rect.Position;}
			set {rect.Position = value;}
		}

		public Size2 Size
		{
			get {return rect.Size;}
			set {rect.Size = value;}
		}

		private Rect2 rect;
		public Rect2 Rect {get {return rect;}}

		private Rect2 visualRect;
		public Rect2 VisualRect {get {return visualRect;}}
		#endregion

		#region Constructors
		public Element(UI ui, int x, int y, int width, int height)
		{
			this.ui = ui;
			rect = new Rect2(x, y, width, height);

			Enabled = true;
			HorizontalAlignment = HorizontalAlignments.Left;
			VerticalAlignment = VerticalAlignments.Bottom;
		}

		protected void init(VisualI[] visuals, ShapeI rolloverShape)
		{
			childeren = new List<Element>();
			Visuals = visuals;
			RolloverShape = rolloverShape;
		}
		#endregion

		#region Methods
		public virtual void Update(MouseI mouse)
		{
			// update childeren
			var childState = ElementStates.None;
			foreach (var child in Childeren)
			{
				child.Update(mouse);
				if (child.currentState != ElementStates.None) childState = child.currentState;
			}

			// get mouse state
			currentState = ElementStates.None;
			if (mouse.Position.Intersects(rect) && childState == ElementStates.None)
			{
				if (lastState == ElementStates.None) currentState = ElementStates.MouseEnter;
				else if (lastState == ElementStates.MouseEnter) currentState = ElementStates.MouseOver;
			}
			else
			{
				if (lastState == ElementStates.MouseEnter || lastState == ElementStates.MouseOver) currentState = ElementStates.MouseExit;
			}
			lastState = currentState;

			// calculate visual rect
			visualRect = rect;

			// update visuals
			foreach (var visual in Visuals) visual.Update(visualRect);
		}

		public virtual void Render()
		{
			foreach (var visual in Visuals) visual.Render(ui.camera);
			foreach (var child in Childeren) child.Render();
		}

		public void AddChild(Element childElement)
		{
			if (!childeren.Contains(childElement)) childeren.Add(childElement);
			else Debug.ThrowError("Element", "Already contains child");
		}

		public void RemoveChild(Element childElement)
		{
			if (childeren.Contains(childElement)) childeren.Remove(childElement);
		}
		#endregion
	}
}
