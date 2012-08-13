﻿using Reign.Core;
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
		private uint[] enabledStreamIndices;
		private int enabledStreamIndicesCount;
		private bool enabled;
		#endregion

		#region Constructors
		public unsafe BufferLayout(DisposableI parent, ShaderI shader, BufferLayoutDescI bufferLayoutDesc)
		: base(parent)
		{
			video = parent.FindParentOrSelfWithException<Video>();
			this.shader = (Shader)shader;
			enabledStreamIndices = new uint[2];

			streamBytesSizes = bufferLayoutDesc.StreamBytesSizes;
			layout = ((BufferLayoutDesc)bufferLayoutDesc).Desc;
			attribLocations = new int[layout.Length];
			for (int i = 0; i != layout.Length; ++i)
			{
				attribLocations[i] = GL.GetAttribLocation(this.shader.Program, layout[i].Name);
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
				for (int stream = 0; stream != bufferLayout.enabledStreamIndicesCount; ++stream)
				{
					for (int i = 0; i != attbs.Length; ++i)
					{
						if (attbs[i] == -1 || bufferLayout.layout[i].StreamIndex != bufferLayout.enabledStreamIndices[stream]) continue;
						GL.DisableVertexAttribArray((uint)attbs[i]);
					}
				}
			}

			video.currentBufferLayout = this;
			enabledStreamIndicesCount = 0;
			enabled = false;
		}

		internal unsafe void enable(uint currentStreamIndex)
		{
			if (enabled) return;
			enabled = true;
		
			enabledStreamIndices[enabledStreamIndicesCount] = currentStreamIndex;
			++enabledStreamIndicesCount;
			
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
			    #if !iOS && !ANDROID
				if (video.Caps.HardwareInstancing)
				{
					GL.VertexAttribDivisor(atLoc, (layout[i].Usage == GLBufferElementUsages.Index) ? 1u : 0u);
				}
				#endif
				//GL.BindAttribLocation(shader.Program, atLoc, layout[i].Name);
			}

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}