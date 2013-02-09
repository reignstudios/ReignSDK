using Reign.Core;
using System.IO;
using System;
using System.Collections.Generic;

namespace Reign.Video
{
	public class Character
	{
		public char Char {get; private set;}
		public Vector2 Offset, SizeRatio;

		private Vector2 size;
		public Vector2 Size
		{
			get {return size;}
			set
			{
				size = value;
				SizeRatio = (value.X >= value.Y) ? new Vector2(1, value.Y / Size.X) : new Vector2(value.X / value.Y, 1);
			}
		}

		public Character(char @char, Vector2 offset, Vector2 size)
		{
			Char = @char;
			Offset = offset;
			Size = size;
		}
	}

	public class FontMetrics
	{
		public struct Character
		{
			public char Key;
			public int X, Y, Width, Height;
		}

		public Character[] Characters;

		public void Load(Stream stream)
		{
			var reader = new BinaryReader(stream);
			int characterCount = reader.ReadInt32();
			Characters = new Character[characterCount];
			for (int i = 0; i != characterCount; ++i)
			{
				Characters[i] = new Character()
				{
					Key = reader.ReadChar(),
					X = reader.ReadInt32(),
					Y = reader.ReadInt32(),
					Width = reader.ReadInt32(),
					Height = reader.ReadInt32(),
				};
			}
		}

		public void Save(Stream stream)
		{
			var writer = new BinaryWriter(stream);
			writer.Write(Characters.Length);
			foreach (var character in Characters)
			{
				writer.Write(character.Key);
				writer.Write(character.X);
				writer.Write(character.Y);
				writer.Write(character.Width);
				writer.Write(character.Height);
			}
		}
	}

	public class Font : Disposable, LoadableI
	{
		#region Properties
		public bool Loaded {get; protected set;}
		public bool FailedToLoad {get; protected set;}

		private ShaderI shader;
		private ShaderVariableI shaderCamera, shaderPosition, shaderSize, shaderPositionUV, shaderSizeUV, texelOffset, shaderColor;
		private ShaderResourceI shaderTexture;
		private Texture2DI texture;
		private BufferLayoutI layout;
		private IndexBufferI indexBuffer;
		private VertexBufferI vertexBuffer;
		private bool instancing;

		public Character[] Characters {get; private set;}

		public static int[] Indices = new int[6]
		{
			0, 1, 2,
			0, 2, 3
		};

		public static float[] Vertices = new float[8]
		{
			0, 0,
			0, 1,
			1, 1,
			1, 0,
		};
		#endregion

		#region Constructors
		public Font(DisposableI parent, ShaderI shader, Texture2DI texture, string metricsFileName, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			new StreamLoader(metricsFileName,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					init(shader, texture, ((StreamLoader)sender).LoadedStream, metricsFileName, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		private void init(ShaderI shader, Texture2DI texture, Stream stream, string metricsFileName, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				// load characters
				var metrics = new FontMetrics();
				metrics.Load(stream);

				Characters = new Character[metrics.Characters.Length];
				for (int i = 0; i != metrics.Characters.Length; ++i)
				{
					var character = metrics.Characters[i];
					Characters[i] = new Character(character.Key, new Vector2(character.X, character.Y), new Vector2(character.Width, character.Height));
				}

				// get shader variables
				this.texture = texture;
				this.shader = shader;
				shaderCamera = shader.Variable("Camera");
				shaderPosition = shader.Variable("Position");
				shaderSize = shader.Variable("Size");
				shaderPositionUV = shader.Variable("PositionUV");
				shaderSizeUV = shader.Variable("SizeUV");
				texelOffset = shader.Variable("TexelOffset");
				shaderColor = shader.Variable("Color");
				shaderTexture = shader.Resource("DiffuseTexture");

				// create buffers
				var layoutDesc = BufferLayoutDescAPI.New(BufferLayoutTypes.Position2);
				layout = BufferLayoutAPI.New(this, shader, layoutDesc);

				indexBuffer = IndexBufferAPI.New(this, BufferUsages.Default, Indices);
				vertexBuffer = VertexBufferAPI.New(this, layoutDesc, BufferUsages.Default, VertexBufferTopologys.Triangle, Vertices);
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (loadedCallback != null) loadedCallback(this, false);
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this, true);
		}

		public bool UpdateLoad()
		{
			return Loaded;
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (layout != null) layout.Dispose();
			if (indexBuffer != null) indexBuffer.Dispose();
			if (vertexBuffer != null) vertexBuffer.Dispose();
			base.Dispose();
		}
		#endregion

		#region Methods
		public int CharacterIndex(char @char)
		{
			int i = ((int)@char) - 32;
			i = (i < 0) ? 0 : i;
			return (i >= Characters.Length) ? (Characters.Length-1) : i;
		}

		public Character FindCharacter(char @char)
		{
			return Characters[CharacterIndex(@char)];
		}

		public void DrawStart(Camera camera)
		{
			vertexBuffer.Enable(indexBuffer);
			shaderCamera.Set(camera.TransformMatrix);
			texelOffset.Set(texture.TexelOffset);
			shaderTexture.Set(texture);
			layout.Enable();
			instancing = false;
		}

		public void Draw(string text, Vector2 position, Vector4 color, float size, bool centeredX, bool centeredY)
		{
			if (instancing)
			{
				
			}
			else
			{
				draw(text, texture.SizeF, position, color, size, centeredX, centeredY);
			}
		}

		private void draw(string text, Vector2 textureSize, Vector2 position, Vector4 color, float size, bool centeredX, bool centeredY)
		{
			if (string.IsNullOrEmpty(text)) return;

			var centeredLoc = new Vector2();
			if (centeredX || centeredY)
			{
				for (int i = 0; i != text.Length; ++i)
				{
					var c = FindCharacter(text[i]);
					if (centeredX) centeredLoc.X += c.SizeRatio.X * size;
					if (centeredY) centeredLoc.Y += c.SizeRatio.Y * size;
				}
				if (text.Length != 0)
				{
					centeredLoc.Y /= (float)text.Length;
					centeredLoc *= .5f;
				}
			}

			var offset = new Vector2();
			for (int i = 0; i != text.Length; ++i)
			{
				var c = FindCharacter(text[i]);
				var sizeScaled = c.SizeRatio * size;
				drawCharacter(offset + position - centeredLoc, sizeScaled, c.Offset / textureSize, c.Size / textureSize, color);

				offset.X += sizeScaled.X;
			}
		}

		private void drawCharacter(Vector2 position, Vector2 size, Vector2 positionUV, Vector2 sizeUV, Vector4 color)
		{
			shaderPosition.Set(position);
			shaderSize.Set(size);
			shaderPositionUV.Set(positionUV);
			shaderSizeUV.Set(sizeUV);
			shaderColor.Set(color);
			shader.Apply();
			vertexBuffer.Draw();
		}
		#endregion
	}
}