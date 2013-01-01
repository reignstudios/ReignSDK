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

#if METRO
using Windows.Storage;
using Windows.ApplicationModel;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Foundation;
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
		
		public NaClFile(string fileName)
		{
			FileName = fileName;
			ID = Guid.NewGuid();
		}
	}
	#endif

	public class StreamLoader : LoadableI
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
			#if METRO
			Loader.AddLoadable(this);
			#endif
			init(null, stream, loadedCallback);
		}

		public StreamLoader(string fileName, Loader.LoadedCallbackMethod loadedCallback)
		{
			#if METRO
			Loader.AddLoadable(this);
			#endif
			init(fileName, null, loadedCallback);
		}

		#if METRO
		private async void init(string fileName, Stream stream, Loader.LoadedCallbackMethod loadedCallback)
		#else
		private void init(string fileName, Stream stream, Loader.LoadedCallbackMethod loadedCallback)
		#endif
		{
			try
			{
				if (fileName != null)
				{
					fromFile = true;
					#if METRO
					LoadedStream = await Streams.OpenFile(fileName);
					#elif NaCl
					this.loadedCallback = loadedCallback;
					this.failedToLoadCallback = failedToLoadCallback;
					
					fileName = fileName.Replace('\\', '/');
					file = new NaClFile(fileName);
					Streams.NaClFileLoadedCallback += fileLoaded;
					Streams.addPendingFile(file);
					Loader.AddLoadable(this);
					
					return;
					#else
					LoadedStream = Streams.OpenFile(fileName);
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
					failedToLoadCallback = null;
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
					failedToLoadCallback = null;
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
		#if METRO
		private static PickerLocationId getFolderType(FolderLocations folderLocation)
		{
			PickerLocationId folder = PickerLocationId.Desktop;
			switch (folderLocation)
			{
				case (FolderLocations.Documents): folder = PickerLocationId.DocumentsLibrary; break;
				case (FolderLocations.Pictures): folder = PickerLocationId.PicturesLibrary; break;
				case (FolderLocations.Music): folder = PickerLocationId.MusicLibrary; break;
				case (FolderLocations.Video): folder = PickerLocationId.VideosLibrary; break;
				default: Debug.ThrowError("Streams", "Unsuported folder location"); break;
			}

			return folder;
		}
		#endif

		#if METRO
		public static async Task<Stream> OpenFileDialog(FolderLocations folderLocation, string[] fileTypes)
		#else
		public static Stream OpenFileDialog(FolderLocations folderLocation, string[] fileTypes)
		#endif
		{
			#if METRO
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

		#if METRO
		public static async Task<Stream> SaveFileDialog(FolderLocations folderLocation, string[] fileTypes)
		#else
		public static Stream SaveFileDialog(FolderLocations folderLocation, string[] fileTypes)
		#endif
		{
			#if METRO
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

		#if METRO
		public static async Task<Stream> OpenFile(string fileName)
		{
			return await OpenFile(fileName, FolderLocations.Application);
		}
		#else
		public static Stream OpenFile(string fileName)
		{
			return OpenFile(fileName, FolderLocations.Application);
		}
		#endif

		#if METRO
		public static async Task<Stream> OpenFile(string fileName, FolderLocations folderLocation)
		#else
		public static Stream OpenFile(string fileName, FolderLocations folderLocation)
		#endif
		{
			if (folderLocation == FolderLocations.Unknown) Debug.ThrowError("Streams", "Unsuported folder type: " + folderLocation.ToString());
			
			#if OSX || iOS
			fileName = fileName.Replace('\\', '/');
			string directory = GetFileDirectory(fileName);
			string ext = GetFileExt(fileName);
			ext = (ext.Length > 0) ? ext.Remove(0, 1) : "";
			string file = GetFileNameWithoutExt(fileName);
			
			string path = NSBundle.MainBundle.PathForResource(file, ext, directory, "");
			if (string.IsNullOrEmpty(path)) Debug.ThrowError("Streams", "Could not find file: " + fileName);
			
			return new FileStream(path, FileMode.Open, FileAccess.Read);
			#elif ANDROID
			try
			{
				fileName = fileName.Replace('\\', '/');
				using (var stream = OS.CurrentApplication.Assets.Open(fileName))
				{
					return CopyToMemoryStream(stream);
				}
			}
			catch (Exception e)
			{
				Debug.ThrowError("Streams", "Could not find file: " + fileName + " - SubErrorMessage: " + e.Message);
				return null;
			}
			#elif NaCl
			Debug.ThrowError("Streams", "Method not supported in NaCl. Use StreamLoader instead");
			return null;
			#elif METRO
			fileName = fileName.Replace('/', '\\');
			switch (folderLocation)
			{
				case (FolderLocations.Application):
					var appFolder = Package.Current.InstalledLocation;
					return await appFolder.OpenStreamForReadAsync(fileName);

				case (FolderLocations.Storage):
					var storageFolder = ApplicationData.Current.LocalFolder;
					return await storageFolder.OpenStreamForReadAsync(fileName);

				case (FolderLocations.Documents):
					var docFile = await KnownFolders.DocumentsLibrary.GetFileAsync(fileName);
					return await docFile.OpenStreamForReadAsync();

				case (FolderLocations.Pictures):
					var picFile = await KnownFolders.PicturesLibrary.GetFileAsync(fileName);
					return await picFile.OpenStreamForReadAsync();

				case (FolderLocations.Music):
					var musicFile = await KnownFolders.MusicLibrary.GetFileAsync(fileName);
					return await musicFile.OpenStreamForReadAsync();

				case (FolderLocations.Video):
					var videoFile = await KnownFolders.VideosLibrary.GetFileAsync(fileName);
					return await videoFile.OpenStreamForReadAsync();
			}
			return null;
			#elif SILVERLIGHT
			fileName = fileName.Replace('\\', '/');
			var file = Application.GetResourceStream(new Uri(fileName, UriKind.Relative));
			if (file == null) Debug.ThrowError("Streams", "Failed to find file: " + fileName);
			return file.Stream;
			#else
			#if LINUX
			fileName = fileName.Replace('\\', '/');
			#else
			fileName = fileName.Replace('/', '\\');
			#endif
			return new FileStream(fileName, FileMode.Open, FileAccess.Read);
			#endif
		}

		#if METRO
		public static async Task<Stream> SaveFile(string fileName)
		{
			return await SaveFile(fileName, FolderLocations.Storage);
		}
		#else
		public static Stream SaveFile(string fileName)
		{
			return SaveFile(fileName, FolderLocations.Storage);
		}
		#endif

		#if METRO
		public static async Task<Stream> SaveFile(string fileName, FolderLocations folderLocation)
		#else
		public static Stream SaveFile(string fileName, FolderLocations folderLocation)
		#endif
		{
			#if OSX || iOS
			throw new NotImplementedException();
			#elif ANDROID
			throw new NotImplementedException();
			#elif NaCl
			throw new NotImplementedException();
			#elif METRO
			fileName = fileName.Replace('/', '\\');
			switch (folderLocation)
			{
				case (FolderLocations.Application):
					var appFolder = Package.Current.InstalledLocation;
					return await appFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting);

				case (FolderLocations.Storage):
					var storageFolder = ApplicationData.Current.LocalFolder;
					return await storageFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting);

				case (FolderLocations.Documents):
					var docFile = await KnownFolders.DocumentsLibrary.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
					return await docFile.OpenStreamForWriteAsync();

				case (FolderLocations.Pictures):
					var picFile = await KnownFolders.PicturesLibrary.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
					return await picFile.OpenStreamForWriteAsync();

				case (FolderLocations.Music):
					var musicFile = await KnownFolders.MusicLibrary.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
					return await musicFile.OpenStreamForWriteAsync();

				case (FolderLocations.Video):
					var videoFile = await KnownFolders.VideosLibrary.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
					return await videoFile.OpenStreamForWriteAsync();

				default:
					Debug.ThrowError("Streams", "Unsuported folder location: " + folderLocation.ToString());
					break;
			}
			return null;
			#else
			#if LINUX
			fileName = fileName.Replace('\\', '/');
			#else
			fileName = fileName.Replace('/', '\\');
			#endif
			return new FileStream(fileName, FileMode.Create, FileAccess.Write);
			#endif
		}

		#if METRO
		public static async Task<bool> FileExists(string fileName)
		{
			return await FileExists(fileName, FolderLocations.Storage);
		}
		#else
		public static bool FileExists(string fileName)
		{
			return FileExists(fileName, FolderLocations.Storage);
		}
		#endif

		#if METRO
		public static async Task<bool> FileExists(string fileName, FolderLocations folderLocation)
		#else
		public static bool FileExists(string fileName, FolderLocations folderLocation)
		#endif
		{
			#if OSX || iOS
			throw new NotImplementedException();
			#elif ANDROID
			throw new NotImplementedException();
			#elif NaCl
			throw new NotImplementedException();
			#elif METRO
			fileName = fileName.Replace('/', '\\');
			try
			{
				switch (folderLocation)
				{
					case (FolderLocations.Application):
						var appFolder = Package.Current.InstalledLocation;
						return (await appFolder.GetFileAsync(fileName)) != null;

					case (FolderLocations.Storage):
						var storageFolder = ApplicationData.Current.LocalFolder;
						return (await storageFolder.GetFileAsync(fileName)) != null;

					case (FolderLocations.Documents):
						return (await KnownFolders.DocumentsLibrary.GetFileAsync(fileName)) != null;

					case (FolderLocations.Pictures):
						return (await KnownFolders.PicturesLibrary.CreateFileAsync(fileName)) != null;

					case (FolderLocations.Music):
						return (await KnownFolders.MusicLibrary.CreateFileAsync(fileName)) != null;

					case (FolderLocations.Video):
						return (await KnownFolders.VideosLibrary.CreateFileAsync(fileName)) != null;

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
			fileName = fileName.Replace('\\', '/');
			#else
			fileName = fileName.Replace('/', '\\');
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

		public static string StripFileExt(string fileName)
		{
			var match = Regex.Match(fileName, @".*\.");
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				fileName = match.Value.Substring(0, match.Value.Length-1);
			}

			return fileName;
		}
		
		public static string GetFileDirectory(string fileName)
		{
			bool pass = false;
			foreach (var c in fileName)
			{
				if (c == '/' || c == '\\')
				{
					pass = true;
					break;
				}
			}
			if (!pass) return "";

			var match = Regex.Match(fileName, @".*[/\\]");
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				fileName = match.Value.Substring(0, match.Value.Length-1);
			}

			#if METRO || LINUX
			return fileName + '\\';
			#else
			return fileName + '/';
			#endif
		}
		
		public static string GetFileNameWithExt(string fileName)
		{
			var match = Regex.Match(fileName, @".*[/\\]");
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				fileName = fileName.Substring(match.Value.Length, fileName.Length - match.Value.Length);
			}

			return fileName;
		}
		
		public static string GetFileNameWithoutExt(string fileName)
		{
			fileName = GetFileNameWithExt(fileName);
			string ext = GetFileExt(fileName);
			return fileName.Substring(0, fileName.Length - ext.Length);
		}

		public static string GetFileExt(string fileName)
		{
			var names = fileName.Split('.');
			if (names.Length < 2) return null;
			return '.' + names[names.Length-1];
		}

		public static bool IsAbsolutePath(string fileName)
		{
			#if WINDOWS
			var match = Regex.Match(fileName, @"A|C|D|E|F|G|H|I:/|\\");
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
	}
}