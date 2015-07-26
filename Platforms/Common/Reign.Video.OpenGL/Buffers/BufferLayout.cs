using Reign.Core;
using System;

namespace Reign.Video.OpenGL
{
	public class BufferLayout : Disposable, BufferLayoutI
	{
		#region Properties
		private Video video;
		private Shader shader;
		private GLBufferElement[] layout;
		private int[] attribLocations, streamBytesSizes;
		private bool[] enabledStreamIndices;
		internal static bool enabled;
		#endregion

		#region Constructors
		public static BufferLayout New(DisposableI parent, ShaderI shader, BufferLayoutDescI desc)
		{
			return new BufferLayout(parent, shader, desc);
		}

		public BufferLayout(DisposableI parent, ShaderI shader, BufferLayoutDescI desc)
		: base(parent)
		{
			video = parent.FindParentOrSelfWithException<Video>();
			this.shader = (Shader)shader;
			enabledStreamIndices = new bool[2];

			streamBytesSizes = desc.StreamBytesSizes;
			layout = ((BufferLayoutDesc)desc).desc;
			attribLocations = new int[layout.Length];
			for (int i = 0; i != layout.Length; ++i)
			{
				attribLocations[i] = GL.GetAttribLocation(this.shader.program, layout[i].Name);
				Video.checkForError();
			}
		}
		#endregion

		#region Methods
		public void Enable()
		{
			if (video.currentBufferLayout != this && video.currentBufferLayout != null)
			{
				var bufferLayout = video.currentBufferLayout;
				var attbs = bufferLayout.attribLocations;
				for (int stream = 0; stream != bufferLayout.enabledStreamIndices.Length; ++stream)
				{
					if (!bufferLayout.enabledStreamIndices[stream]) continue;

					for (int i = 0; i != attbs.Length; ++i)
					{
						if (attbs[i] == -1 || bufferLayout.layout[i].StreamIndex != stream) continue;
						GL.DisableVertexAttribArray((uint)attbs[i]);
					}
				}

				enabled = false;
			}

			video.currentBufferLayout = this;
			for (int i = 0; i != enabledStreamIndices.Length; ++i) enabledStreamIndices[i] = false;
		}

		internal unsafe void enable(uint currentStreamIndex)
		{
			enabled = true;
			enabledStreamIndices[currentStreamIndex] = true;
			
			for (int i = 0; i != layout.Length; ++i)
			{
				if (attribLocations[i] == -1) continue;

				uint streamIndex = layout[i].StreamIndex;
				if (streamIndex != currentStreamIndex) continue;

				uint atLoc = (uint)attribLocations[i];
			    GL.EnableVertexAttribArray(atLoc);

				uint format = GL.FLOAT;
				int floatCount = layout[i].FloatCount;
				bool normalize = false;
				if (layout[i].Usage == GLBufferElementUsages.Color)
				{
				    format = GL.UNSIGNED_BYTE;
				    floatCount = 4;
					normalize = true;
				}
				
			    GL.VertexAttribPointer(atLoc, floatCount, format, normalize, streamBytesSizes[streamIndex], layout[i].Offset.ToPointer());
			    #if !iOS && !ANDROID && !NaCl && !RPI
				if (video.Caps.HardwareInstancing)
				{
					GL.VertexAttribDivisor(atLoc, (layout[i].Usage == GLBufferElementUsages.Index) ? 1u : 0u);
				}
				#endif
			}

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}