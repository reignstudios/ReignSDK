using Reign.Core;
using System.IO;
using System.Xml.Serialization;
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

	[XmlRoot("FontMetrics")]
	public class FontMetrics
	{
		public class Character
		{
			[XmlAttribute("Key")]
			public char Key;

			[XmlElement("X")]
			public int X;

			[XmlElement("Y")]
			public int Y;
				
			[XmlElement("Width")]
			public int Width;
				
			[XmlElement("Height")]
			public int Height;
		}

		[XmlElement("Character")]
		public Character[] Characters;
	}

	public abstract class FontI : Disposable
	{
		#region Properties
		public bool Loaded {get; protected set;}
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
		public FontI(DisposableI parent)
		: base(parent)
		{
			Characters = new Character[95];
			for (int i = 0; i != Characters.Length; ++i)
			{
				Characters[i] = new Character((char)(i + 32), new Vector2(), new Vector2());
			}
		}

		public FontI(DisposableI parent, string metricsFileName)
		: base(parent)
		{
			
		}

		#if METRO
		protected virtual async void init(ShaderI shader, Texture2DI texture, string metricsFileName)
		{
			using (var stream = await Streams.OpenFile(metricsFileName))
		#else
		protected virtual void init(ShaderI shader, Texture2DI texture, string metricsFileName)
		{
			using (var stream = Streams.OpenFile(metricsFileName))
		#endif
			{
				var xml = new XmlSerializer(typeof(FontMetrics));
				var metrics = xml.Deserialize(stream) as FontMetrics;
				if (metrics == null) Debug.ThrowError("FontI", "Failed to deserialize font metrics: " + metricsFileName);

				Characters = new Character[metrics.Characters.Length];
				for (int i = 0; i != metrics.Characters.Length; ++i)
				{
					var character = metrics.Characters[i];
					Characters[i] = new Character(character.Key, new Vector2(character.X, character.Y), new Vector2(character.Width, character.Height));
				}
			}
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

		public abstract void DrawStart(Camera camera);
		public abstract void Draw(string text, Vector2 location, Vector4 color, float Size, bool centeredX, bool centeredY);

		protected void draw(string text, Vector2 textureSize, Vector2 location, Vector4 color, float size, bool centeredX, bool centeredY)
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
				draw(offset + location - centeredLoc, sizeScaled, c.Offset / textureSize, c.Size / textureSize, color);

				offset.X += sizeScaled.X;
			}
		}

		protected abstract void draw(Vector2 location, Vector2 size, Vector2 locationUV, Vector2 sizeUV, Vector4 color);
		#endregion
	}
}