using System;
using System.Collections.Generic;
using Reign.Core;
using Reign.Video;

namespace Reign.UI
{
	public class UI : Disposable
	{
		#region Properties
		private VideoI video;
		private List<GeometryI> geometries;
		private Element[] elements;
		private LinkedList<Element> activeElements;

		public delegate void RenderCallbackMethod();
		public RenderCallbackMethod RenderCallback;
		#endregion

		#region Constructors
		public UI(DisposableI parent, VideoI video)
		: base(parent)
		{
			init(video, null);
		}

		public UI(DisposableI parent, VideoI video, string fileName)
		: base(parent)
		{
			throw new NotImplementedException();//init(video, fileName);
		}

		private void init(VideoI video, string fileName)
		{
			try
			{
				this.video = video;
				geometries = new List<GeometryI>();
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}
		#endregion

		#region Methods
		public void Update()
		{
			foreach (var element in activeElements) element.Update();
		}

		public void Render()
		{
			if (RenderCallback != null) RenderCallback();
			foreach (var element in activeElements) element.Render();
		}

		public GeometryI LoadGeometry(Type geometryType)
		{
			foreach (var g in geometries)
			{
				if (g.GetType() == geometryType) return g;
			}

			if (geometryType == typeof(RectangleGeometry)) return new RectangleGeometry(this);
			
			Debug.ThrowError("UI", "Unsuported Geometry Type");
			return null;
		}
		#endregion
	}
}
