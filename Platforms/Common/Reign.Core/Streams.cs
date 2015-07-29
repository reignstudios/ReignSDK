using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if OSX
using MonoMac.Foundation;
#endif

#if iOS
using MonoTouch.Foundation;
#endif

#if NaCl
using System.Collections.Generic;
#endif

#if WINRT || WP8
using Windows.Storage;
using Windows.ApplicationModel;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Foundation;
#endif

#if SILVERLIGHT
using System.Windows;
#endif

namespace Reign.Core
{
	#if NaCl
	class NaClFile
	{
		public string FileName;
		public Guid ID;
		public byte[] Data;
		public bool FailedToLoad;
		
		public NaClFile(string filename)
		{
			FileName = filename;
			ID = Guid.NewGuid();
		}
	}
	#endif

	public class StreamLoader : ILoadable
	{
		#region Properties
		public bool Loaded {get; private set;}
		public bool FailedToLoad {get; private set;}

		public Stream LoadedStream;
		bool fromFile;
		#if NaCl
		private NaClFile file;
		private Loader.LoadedCallbackMethod loadedCallback;
		#endif
		#endregion

		#region Constructors
		public StreamLoader(Stream stream, Loader.LoadedCallbackMethod loadedCallback)
		{
			Loader.AddLoadable(this);
			init(null, stream, loadedCallback);
		}

		public StreamLoader(string filename, Loader.LoadedCallbackMethod loadedCallback)
		{
			Loader.AddLoadable(this);
			init(filename, null, loadedCallback);
		}

		#if WINRT || WP8
		private async void init(string filename, Stream stream, Loader.LoadedCallbackMethod loadedCallback)
		#else
		private void init(string filename, Stream stream, Loader.LoadedCallbackMethod loadedCallback)
		#endif
		{
			try
			{
				if (filename != null)
				{
					fromFile = true;
					#if WINRT || WP8
					LoadedStream = await Streams.OpenFile(filename);
					#elif NaCl
					this.loadedCallback = loadedCallback;
					
					filename = filename.Replace('\\', '/');
					file = new NaClFile(filename);
					Streams.NaClFileLoadedCallback += fileLoaded;
					Streams.addPendingFile(file);
					Loader.AddLoadable(this);
					
					return;
					#else
					LoadedStream = Streams.OpenFile(filename);
					#endif
				}
				else
				{
					fromFile = false;
					LoadedStream = stream;
				}
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (loadedCallback != null) loadedCallback(this, false);
				return;
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this, true);
		}
		
		#if NaCl
		private void fileLoaded(NaClFile file)
		{
			if (file.ID == this.file.ID)
			{
				Streams.NaClFileLoadedCallback -= fileLoaded;
				
				if (file.FailedToLoad)
				{
					FailedToLoad = true;
					if (loadedCallback != null) loadedCallback(this, false);
					loadedCallback = null;
					Dispose();
					return;
				}
				
				try
				{
					LoadedStream = new MemoryStream(file.Data);
					LoadedStream.Position = 0;
				}
				catch (Exception e)
				{
					FailedToLoad = true;
					Loader.AddLoadableException(e);
					if (loadedCallback != null) loadedCallback(this, false);
					loadedCallback = null;
					Dispose();
					return;
				}
				
				Loaded = true;
				if (loadedCallback != null) loadedCallback(this, true);
				loadedCallback = null;
			}
		}
		#endif

		public bool UpdateLoad()
		{
			return Loaded;
		}

		public void Dispose()
		{
			#if NaCl
			loadedCallback = null;
			#endif
		
			if (fromFile && LoadedStream != null)
			{
				LoadedStream.Dispose();
				LoadedStream = null;
			}
		}

		~StreamLoader()
		{
			Dispose();
		}
		#endregion
	}

	public enum FolderLocations
	{
		Unknown,
		Application,
		Storage,
		Documents,
		Pictures,
		Music,
		Video
	}
	
	public static class Streams
	{
		#if WINRT || WP8
		private static PickerLocationId getFolderType(FolderLocations folderLocation)
		{
			PickerLocationId folder = PickerLocationId.Desktop;
			switch (folderLocation)
			{
				case FolderLocations.Documents: folder = PickerLocationId.DocumentsLibrary; break;
				case FolderLocations.Pictures: folder = PickerLocationId.PicturesLibrary; break;
				case FolderLocations.Music: folder = PickerLocationId.MusicLibrary; break;
				case FolderLocations.Video: folder = PickerLocationId.VideosLibrary; break;
				default: Debug.ThrowError("Streams", "Unsuported folder location"); break;
			}

			return folder;
		}
		#endif

		#if WINRT || WP8
		public static async Task<Stream> OpenFileDialog(FolderLocations folderLocation, string[] fileTypes)
		#else
		public static Stream OpenFileDialog(FolderLocations folderLocation, string[] fileTypes)
		#endif
		{
			#if WINRT || WP8
			var picker = new FileOpenPicker();
			foreach (var fileType in fileTypes) picker.FileTypeFilter.Add(fileType);
			picker.SuggestedStartLocation = getFolderType(folderLocation);
			var file = await picker.PickSingleFileAsync();
			if (file != null) return await file.OpenStreamForReadAsync();
			else return null;
			#else
			throw new NotImplementedException();
			#endif
		}

		#if !WP8
		#if WINRT
		public static async Task<Stream> SaveFileDialog(FolderLocations folderLocation, string[] fileTypes)
		#else
		public static Stream SaveFileDialog(FolderLocations folderLocation, string[] fileTypes)
		#endif
		{
			#if WINRT
			var picker = new FileSavePicker();
			picker.FileTypeChoices.Add(new KeyValuePair<string,IList<string>>("Supported File Types", fileTypes));
			picker.SuggestedStartLocation = getFolderType(folderLocation);
			var file = await picker.PickSaveFileAsync();
			if (file != null) return await file.OpenStreamForWriteAsync();
			else return null;
			#else
			throw new NotImplementedException();
			#endif
		}
		#endif

		#if NaCl
		internal delegate void NaClFileLoadedCallbackMethod(NaClFile file);
		internal static event NaClFileLoadedCallbackMethod NaClFileLoadedCallback;
		private static List<NaClFile> naclFiles;
		
		static Streams()
		{
			naclFiles = new List<NaClFile>();
		}
		
		[DllImport("__Internal", EntryPoint="URLLoader_LoadFile", ExactSpelling = true)]
		private extern static void URLLoader_LoadFile(string url, string id);
		
		private unsafe static void URLLoader_Done(byte* data, uint dataSize, byte* id, bool failedToLoad)
		{
			byte[] fileData = null;
			if (!failedToLoad)
			{
				fileData = new byte[dataSize];
				var ptr = new IntPtr(data);
				Marshal.Copy(ptr, fileData, 0, (int)dataSize);
				Marshal.FreeHGlobal(ptr);
			}
			
			var stringID = Marshal.PtrToStringAnsi(new IntPtr(id));
			var guidID = new Guid(stringID);
			foreach (var file in naclFiles.ToArray())
			{
				if (file.ID == guidID)
				{
					naclFiles.Remove(file);
					
					file.Data = fileData;
					file.FailedToLoad = failedToLoad;
					if (NaClFileLoadedCallback != null) NaClFileLoadedCallback(file);
					break;
				}
			}
		}
		
		internal static void addPendingFile(NaClFile file)
		{
			naclFiles.Add(file);
			URLLoader_LoadFile(file.FileName, file.ID.ToString());
		}
		#endif

		#if WINRT || WP8
		public static async Task<Stream> OpenFile(string filename)
		{
			return await OpenFile(filename, FolderLocations.Application);
		}
		#else
		public static Stream OpenFile(string filename)
		{
			return OpenFile(filename, FolderLocations.Application);
		}
		#endif

		#if WINRT || WP8
		public static async Task<Stream> OpenFile(string filename, FolderLocations folderLocation)
		#else
		public static Stream OpenFile(string filename, FolderLocations folderLocation)
		#endif
		{
			if (folderLocation == FolderLocations.Unknown) Debug.ThrowError("Streams", "Unsuported folder type: " + folderLocation.ToString());
			
			#if OSX || iOS
			filename = filename.Replace('\\', '/');
			string directory = GetFileDirectory(filename);
			string ext = GetFileExt(filename);
			ext = (ext.Length > 0) ? ext.Remove(0, 1) : "";
			string file = GetFileNameWithoutExt(filename);
			
			string path = NSBundle.MainBundle.PathForResource(file, ext, directory, "");
			if (string.IsNullOrEmpty(path)) Debug.ThrowError("Streams", "Could not find file: " + filename);
			
			return new FileStream(path, FileMode.Open, FileAccess.Read);
			#elif ANDROID
			try
			{
				filename = filename.Replace('\\', '/');
				using (var stream = ((AndroidApplication)OS.CurrentApplication).Assets.Open(filename))
				{
					return CopyToMemoryStream(stream);
				}
			}
			catch (Exception e)
			{
				Debug.ThrowError("Streams", "Could not find file: " + filename + " - SubErrorMessage: " + e.Message);
				return null;
			}
			#elif NaCl
			Debug.ThrowError("Streams", "Method not supported in NaCl. Use StreamLoader instead");
			return null;
			#elif WINRT || WP8
			filename = filename.Replace('/', '\\');
			switch (folderLocation)
			{
				case FolderLocations.Application:
					var appFolder = Package.Current.InstalledLocation;
					return await appFolder.OpenStreamForReadAsync(filename);

				case FolderLocations.Storage:
					var storageFolder = ApplicationData.Current.LocalFolder;
					return await storageFolder.OpenStreamForReadAsync(filename);

				case FolderLocations.Documents:
					var docFile = await KnownFolders.DocumentsLibrary.GetFileAsync(filename);
					return await docFile.OpenStreamForReadAsync();

				case FolderLocations.Pictures:
					var picFile = await KnownFolders.PicturesLibrary.GetFileAsync(filename);
					return await picFile.OpenStreamForReadAsync();

				case FolderLocations.Music:
					var musicFile = await KnownFolders.MusicLibrary.GetFileAsync(filename);
					return await musicFile.OpenStreamForReadAsync();

				case FolderLocations.Video:
					var videoFile = await KnownFolders.VideosLibrary.GetFileAsync(filename);
					return await videoFile.OpenStreamForReadAsync();
			}
			return null;
			#elif SILVERLIGHT
			filename = filename.Replace('\\', '/');
			var file = Application.GetResourceStream(new Uri(filename, UriKind.Relative));
			if (file == null) Debug.ThrowError("Streams", "Failed to find file: " + filename);
			return file.Stream;
			#else
			#if LINUX
			filename = filename.Replace('\\', '/');
			#else
			filename = filename.Replace('/', '\\');
			#if VITA
			filename = "/Application/" + filename;
			#endif
			#endif
			return new FileStream(filename, FileMode.Open, FileAccess.Read);
			#endif
		}

		#if WINRT || WP8
		public static async Task<Stream> SaveFile(string filename)
		{
			return await SaveFile(filename, FolderLocations.Storage);
		}
		#else
		public static Stream SaveFile(string filename)
		{
			return SaveFile(filename, FolderLocations.Storage);
		}
		#endif

		#if WINRT || WP8
		public static async Task<Stream> SaveFile(string filename, FolderLocations folderLocation)
		#else
		public static Stream SaveFile(string filename, FolderLocations folderLocation)
		#endif
		{
			#if OSX || iOS
			throw new NotImplementedException();
			#elif ANDROID
			throw new NotImplementedException();
			#elif NaCl
			throw new NotImplementedException();
			#elif WINRT || WP8
			filename = filename.Replace('/', '\\');
			switch (folderLocation)
			{
				case FolderLocations.Application:
					var appFolder = Package.Current.InstalledLocation;
					return await appFolder.OpenStreamForWriteAsync(filename, CreationCollisionOption.ReplaceExisting);

				case FolderLocations.Storage:
					var storageFolder = ApplicationData.Current.LocalFolder;
					return await storageFolder.OpenStreamForWriteAsync(filename, CreationCollisionOption.ReplaceExisting);

				case FolderLocations.Documents:
					var docFile = await KnownFolders.DocumentsLibrary.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
					return await docFile.OpenStreamForWriteAsync();

				case FolderLocations.Pictures:
					var picFile = await KnownFolders.PicturesLibrary.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
					return await picFile.OpenStreamForWriteAsync();

				case FolderLocations.Music:
					var musicFile = await KnownFolders.MusicLibrary.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
					return await musicFile.OpenStreamForWriteAsync();

				case FolderLocations.Video:
					var videoFile = await KnownFolders.VideosLibrary.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
					return await videoFile.OpenStreamForWriteAsync();

				default:
					Debug.ThrowError("Streams", "Unsuported folder location: " + folderLocation.ToString());
					break;
			}
			return null;
			#else
			#if LINUX
			filename = filename.Replace('\\', '/');
			#else
			filename = filename.Replace('/', '\\');
			#endif
			return new FileStream(filename, FileMode.Create, FileAccess.Write);
			#endif
		}

		#if WINRT || WP8
		public static async Task<bool> FileExists(string filename)
		{
			return await FileExists(filename, FolderLocations.Storage);
		}
		#else
		public static bool FileExists(string filename)
		{
			return FileExists(filename, FolderLocations.Storage);
		}
		#endif

		#if WINRT || WP8
		public static async Task<bool> FileExists(string filename, FolderLocations folderLocation)
		#else
		public static bool FileExists(string filename, FolderLocations folderLocation)
		#endif
		{
			#if OSX || iOS
			throw new NotImplementedException();
			#elif ANDROID
			throw new NotImplementedException();
			#elif NaCl
			throw new NotImplementedException();
			#elif WINRT || WP8
			filename = filename.Replace('/', '\\');
			try
			{
				switch (folderLocation)
				{
					case FolderLocations.Application:
						var appFolder = Package.Current.InstalledLocation;
						return (await appFolder.GetFileAsync(filename)) != null;

					case FolderLocations.Storage:
						var storageFolder = ApplicationData.Current.LocalFolder;
						return (await storageFolder.GetFileAsync(filename)) != null;

					case FolderLocations.Documents:
						return (await KnownFolders.DocumentsLibrary.GetFileAsync(filename)) != null;

					case FolderLocations.Pictures:
						return (await KnownFolders.PicturesLibrary.CreateFileAsync(filename)) != null;

					case FolderLocations.Music:
						return (await KnownFolders.MusicLibrary.CreateFileAsync(filename)) != null;

					case FolderLocations.Video:
						return (await KnownFolders.VideosLibrary.CreateFileAsync(filename)) != null;

					default:
						Debug.ThrowError("Streams", "Unsuported folder location: " + folderLocation.ToString());
						break;
				}
			}
			catch
			{
				return false;
			}
			return false;
			#else
			#if LINUX
			filename = filename.Replace('\\', '/');
			#else
			filename = filename.Replace('/', '\\');
			#endif
			throw new NotImplementedException();
			#endif
		}
		
		public static MemoryStream CopyToMemoryStream(Stream stream)
		{
			var memoryStream = new MemoryStream();
			var buffer = new byte[1024];
			while (true)
			{
				int readLength = stream.Read(buffer, 0, buffer.Length);
				memoryStream.Write(buffer, 0, readLength);
				if (readLength != buffer.Length) break;
			}
		
			memoryStream.Position = 0;
			return memoryStream;
		}

		public static string StripFileExt(string filename)
		{
			var match = Regex.Match(filename, @".*\.");
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				filename = match.Value.Substring(0, match.Value.Length-1);
			}

			return filename;
		}
		
		public static string GetFileDirectory(string filename)
		{
			bool pass = false;
			foreach (var c in filename)
			{
				if (c == '/' || c == '\\')
				{
					pass = true;
					break;
				}
			}
			if (!pass) return "";

			var match = Regex.Match(filename, @".*[/\\]");
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				filename = match.Value.Substring(0, match.Value.Length-1);
			}

			#if WINRT || WP8 || LINUX
			return filename + '\\';
			#else
			return filename + '/';
			#endif
		}
		
		public static string GetFileNameWithExt(string filename)
		{
			var match = Regex.Match(filename, @".*[/\\]");
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				filename = filename.Substring(match.Value.Length, filename.Length - match.Value.Length);
			}

			return filename;
		}
		
		public static string GetFileNameWithoutExt(string filename)
		{
			filename = GetFileNameWithExt(filename);
			string ext = GetFileExt(filename);
			return filename.Substring(0, filename.Length - ext.Length);
		}

		public static string GetFileExt(string filename)
		{
			var names = filename.Split('.');
			if (names.Length < 2) return null;
			return '.' + names[names.Length-1];
		}

		public static bool IsAbsolutePath(string filename)
		{
			#if WIN32
			var match = Regex.Match(filename, @"A|C|D|E|F|G|H|I:/|\\");
			return match.Success;
			#else
			throw new NotImplementedException();
			#endif
		}

		public static int MakeFourCC(char ch0, char ch1, char ch2, char ch3)
		{
			return (((int)(byte)(ch0)) | ((int)(byte)(ch1) << 8) | ((int)(byte)(ch2) << 16) | ((int)(byte)(ch3) << 24));
		}
	}

	public static class StreamExtensions
	{
		#region Vectors
		public static void WriteVector(this BinaryWriter writer, Vector2 value)
		{
			writer.Write(value.X);
			writer.Write(value.Y);
		}

		public static void WriteVector(this BinaryWriter writer, Vector3 value)
		{
			writer.Write(value.X);
			writer.Write(value.Y);
			writer.Write(value.Z);
		}

		public static void WriteVector(this BinaryWriter writer, Vector4 value)
		{
			writer.Write(value.X);
			writer.Write(value.Y);
			writer.Write(value.Z);
			writer.Write(value.W);
		}

		public static Vector2 ReadVector2(this BinaryReader reader)
		{
			return new Vector2(reader.ReadSingle(), reader.ReadSingle());
		}

		public static Vector3 ReadVector3(this BinaryReader reader)
		{
			return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static Vector4 ReadVector4(this BinaryReader reader)
		{
			return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}
		#endregion

		#region Matrices
		public static void WriteMatrix(this BinaryWriter writer, Matrix2 value)
		{
			writer.WriteVector(value.X);
			writer.WriteVector(value.Y);
		}

		public static void WriteMatrix(this BinaryWriter writer, Matrix3 value)
		{
			writer.WriteVector(value.X);
			writer.WriteVector(value.Y);
			writer.WriteVector(value.Z);
		}

		public static void WriteMatrix(this BinaryWriter writer, Matrix4 value)
		{
			writer.WriteVector(value.X);
			writer.WriteVector(value.Y);
			writer.WriteVector(value.Z);
			writer.WriteVector(value.W);
		}

		public static Matrix2 ReadMatrix2(this BinaryReader reader)
		{
			return new Matrix2(reader.ReadVector2(), reader.ReadVector2());
		}

		public static Matrix3 ReadMatrix3(this BinaryReader reader)
		{
			return new Matrix3(reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3());
		}

		public static Matrix4 ReadMatrix4(this BinaryReader reader)
		{
			return new Matrix4(reader.ReadVector4(), reader.ReadVector4(), reader.ReadVector4(), reader.ReadVector4());
		}
		#endregion
	}
}