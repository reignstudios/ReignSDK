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
		Enter,
		Exit,
		Over,
		Pressed,
		Down,
		Up
	}

	public enum HorizontalAlignments
	{
		Left,
		LeftOuter,
		Right,
		RightOuter,
		Center
	}

	public enum VerticalAlignments
	{
		Bottom,
		BottomOuter,
		Top,
		TopOuter,
		Center
	}

	public enum AlignmentTypes
	{
		Fixed,
		AutoScale,
		PercentOfParentWidth,
		PercentOfParentHeight
	}

	public class ElementEventArgs
	{
		public Point2 MousePosition;
	}

	public abstract class Element
	{
		#region Properties
		protected UI ui;
		public EffectI[] Effects;
		public bool Enabled;
		private ElementStates currentState, lastState;

		public VisualI[] Visuals {get; private set;}
		public ShapeI RolloverShape {get; private set;}

		private Element parent;
		public Element Parent {get{return parent;}}

		private List<Element> childeren;
		public ReadOnlyCollection<Element> Childeren {get {return childeren.AsReadOnly();}}

		public HorizontalAlignments HorizontalAlignment;
		public VerticalAlignments VerticalAlignment;
		public AlignmentTypes PositionAlignmentTypeX, PositionAlignmentTypeY, SizeAlignmentTypeWidth, SizeAlignmentTypeHeight;
		public bool CenterX, CenterY;

		public bool AutoScaleAll
		{
			set
			{
				var scale = value ? AlignmentTypes.AutoScale : AlignmentTypes.Fixed;
				PositionAlignmentTypeX = scale;
				PositionAlignmentTypeY = scale;
				SizeAlignmentTypeWidth = scale;
				SizeAlignmentTypeHeight = scale;
			}
		}

		public float PercentX, PercentY;
		public Point2 Position
		{
			get {return RolloverShape.Rect.Position;}
			set {RolloverShape.Rect = new Rect2(value, RolloverShape.Rect.Size);}
		}

		public float PercentWidth, PercentHeight;
		public Size2 Size
		{
			get {return RolloverShape.Rect.Size;}
			set {RolloverShape.Rect = new Rect2(RolloverShape.Rect.Position, value);}
		}

		private Rect2 visualRect;
		public Rect2 VisualRect {get {return visualRect;}}

		public delegate void MouseEventCallBack(Element sender, ElementEventArgs args);
		public event MouseEventCallBack OnEnter, OnExit, OnPressed, OnDown, OnUp, OnOver;
		private ElementEventArgs eventArgs;
		#endregion

		#region Constructors
		public Element(UI ui)
		{
			this.ui = ui;
			eventArgs = new ElementEventArgs();

			Enabled = true;
			HorizontalAlignment = HorizontalAlignments.Left;
			VerticalAlignment = VerticalAlignments.Bottom;
			AutoScaleAll = ui.AutoScaleAllDefault;
		}

		protected void init(VisualI[] visuals, ShapeI rolloverShape)
		{
			childeren = new List<Element>();
			Visuals = visuals;
			RolloverShape = rolloverShape;
		}
		#endregion

		#region Methods
		private int GetAlignmentValue(AlignmentTypes alignmentType, int parentWidth, int parentHeight, int value, float percentValue)
		{
			switch (alignmentType)
			{
				case AlignmentTypes.Fixed: return value;
				case AlignmentTypes.AutoScale: return (int)(value * ui.AutoScale);
				case AlignmentTypes.PercentOfParentWidth: return (int)(parentWidth * percentValue);
				case AlignmentTypes.PercentOfParentHeight: return (int)(parentHeight * percentValue);
			}

			return 0;
		}

		public virtual void Update(MouseI mouse)
		{
			// align, offset and scale rollover rect
			int parentLeft, parentRight, parentBottom, parentTop;
			int parentWidth, parentHeight;
			if (parent == null)
			{
				parentLeft = ui.Left;
				parentRight = ui.Right;
				parentBottom = ui.Bottom;
				parentTop = ui.Top;
				parentWidth = ui.Width;
				parentHeight = ui.Height;
			}
			else
			{
				var rect = parent.visualRect;
				parentLeft = rect.Position.X;
				parentRight = rect.Position.X + rect.Size.Width;
				parentBottom = rect.Position.Y;
				parentTop = rect.Position.Y + rect.Size.Height;
				parentWidth = rect.Size.Width;
				parentHeight = rect.Size.Height;
			}
			
			// get scaled size
			Rect2 rolloverRect = RolloverShape.Rect;
			rolloverRect.Position.X = GetAlignmentValue(PositionAlignmentTypeX, parentWidth, parentHeight, rolloverRect.Position.X, PercentX);
			rolloverRect.Position.Y = GetAlignmentValue(PositionAlignmentTypeY, parentWidth, parentHeight, rolloverRect.Position.Y, PercentY);
			rolloverRect.Size.Width = GetAlignmentValue(SizeAlignmentTypeWidth, parentWidth, parentHeight, rolloverRect.Size.Width, PercentWidth);
			rolloverRect.Size.Height = GetAlignmentValue(SizeAlignmentTypeHeight, parentWidth, parentHeight, rolloverRect.Size.Height, PercentHeight);
			if (CenterX) rolloverRect.Position.X -= rolloverRect.Size.Width / 2;
			if (CenterY) rolloverRect.Position.Y -= rolloverRect.Size.Height / 2;

			// align to parent
			switch (HorizontalAlignment)
			{
				case HorizontalAlignments.Left: rolloverRect.Position.X += parentLeft; break;
				case HorizontalAlignments.LeftOuter: rolloverRect.Position.X += parentLeft - rolloverRect.Size.Width; break;
				case HorizontalAlignments.Right: rolloverRect.Position.X += parentRight - rolloverRect.Size.Width; break;
				case HorizontalAlignments.RightOuter: rolloverRect.Position.X += parentRight; break;
				case HorizontalAlignments.Center: rolloverRect.Position.X += (parentLeft + parentRight) / 2; break;
			}

			switch (VerticalAlignment)
			{
				case VerticalAlignments.Bottom: rolloverRect.Position.Y += parentBottom; break;
				case VerticalAlignments.BottomOuter: rolloverRect.Position.Y += parentBottom - rolloverRect.Size.Height; break;
				case VerticalAlignments.Top: rolloverRect.Position.Y += (parentTop - rolloverRect.Size.Height); break;
				case VerticalAlignments.TopOuter: rolloverRect.Position.Y += parentTop; break;
				case VerticalAlignments.Center: rolloverRect.Position.Y += (parentBottom + parentTop) / 2; break;
			}

			// get mouse state
			eventArgs.MousePosition = mouse.Position;
			currentState = ElementStates.None;
			if (mouse.Position.Intersects(rolloverRect))
			{
				if (lastState == ElementStates.None)
				{
					currentState = ElementStates.Enter;
					if (OnEnter != null) OnEnter(this, eventArgs);
				}
				else if (lastState == ElementStates.Enter || lastState == ElementStates.Over || lastState == ElementStates.Pressed || lastState == ElementStates.Down || lastState == ElementStates.Up)
				{
					if (mouse.Left.On)
					{
						currentState = ElementStates.Pressed;
						if (OnPressed != null) OnPressed(this, eventArgs);
					}
					else
					{
						currentState = ElementStates.Over;
						if (OnOver != null) OnOver(this, eventArgs);
					}

					if (mouse.Left.Down)
					{
						currentState = ElementStates.Down;
						if (OnDown != null) OnDown(this, eventArgs);
					}
					else if (mouse.Left.Up)
					{
						currentState = ElementStates.Up;
						if (OnUp != null) OnUp(this, eventArgs);
					}
				}
			}
			else if (lastState == ElementStates.Enter || lastState == ElementStates.Over)
			{
				 currentState = ElementStates.Exit;
				 if (OnExit != null) OnExit(this, eventArgs);
			}
			lastState = currentState;

			// calculate visual effects
			if (Effects != null)
			{
				foreach (var effect in Effects) effect.Update(rolloverRect, Visuals, currentState, out visualRect);
			}
			else
			{
				visualRect = rolloverRect;
			}

			// update visuals
			foreach (var visual in Visuals) visual.Update(visualRect);

			// update childeren
			foreach (var child in Childeren)
			{
				child.Update(mouse);
			}
		}

		public virtual void Render()
		{
			foreach (var visual in Visuals) visual.Render(ui);
			foreach (var child in Childeren) child.Render();
		}

		public void AddChild(Element childElement)
		{
			if (!childeren.Contains(childElement))
			{
				childeren.Add(childElement);
				childElement.parent = this;
			}
			else
			{
				Debug.ThrowError("Element", "Already contains child");
			}
		}

		public void RemoveChild(Element childElement)
		{
			if (childeren.Contains(childElement))
			{
				childeren.Remove(childElement);
				childElement.parent = null;
			}
		}
		#endregion
	}
}
