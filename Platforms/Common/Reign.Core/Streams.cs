﻿using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

#if OSX
using MonoMac.Foundation;
#endif

#if iOS
using MonoTouch.Foundation;
#endif

#if NaCl
using System.Collections.Generic;
#endif

namespace Reign.Core
{
	public enum SourceTypes
	{
		File,
		Memory
	}
	
	public static class Streams
	{
		#if NaCl
		class NaClFile
		{
			public string FileName;
			public byte[] Data;
		}
		private static List<NaClFile> files = new List<NaClFile>();
		private static NaClFile pendingFile;
		private static bool pendingFiles;
		
		[DllImport("__Internal", EntryPoint="URLLoader_LoadFile", ExactSpelling = true)]
		private extern static void URLLoader_LoadFile(string url);
		
		private static byte[] fileData;
		private unsafe static void URLLoader_Done(byte* data, uint dataSize)
		{
			fileData = new byte[dataSize];
			var ptr = new IntPtr(data);
			Marshal.Copy(ptr, fileData, 0, (int)dataSize);
			Marshal.FreeHGlobal(ptr);
		}
		
		public static void AddPendingFiles(List<string> fileNames)
		{
			pendingFiles = true;
			foreach (var fileName in fileNames)
			{
				var file = new NaClFile()
				{
					FileName = fileName
				};
				files.Add(file);
			}
		}
		
		public static bool LoadPendingFiles()
		{
			if (!pendingFiles) return true;
		
			if (pendingFile != null)
			{
				if (fileData != null)
				{
					pendingFile.Data = fileData;
					fileData = null;
					pendingFile = null;
				}
				else
				{
					return false;
				}
			}
			
			foreach (var file in files)
			{
				if (file.Data == null)
				{
					URLLoader_LoadFile(file.FileName);
					pendingFile = file;
					return false;
				}
			}
			
			pendingFiles = false;
			return true;
		}
		
		public static void RemoveFiles(List<string> fileNames)
		{
			var removingList = new List<NaClFile>();
			for (int i = 0; i != fileNames.Count; ++i)
			{
				for (int i2 = 0; i2 != files.Count; ++i2)
				{
					if (fileNames[i] == files[i2].FileName) removingList.Add(files[i2]);
				}
			}
			
			foreach (var file in removingList)
			{
				files.Remove(file);
			}
		}
		#endif
	
		public static string GetResourceName(string assemblyName)
		{
			return "/" + new AssemblyName(assemblyName).Name + ";component/";
		}

		public static Stream OpenStream(string url)
		{
			// for Embeded Resources
			//var assembly = Assembly.GetAssembly(OS.GetType("Reign.Core", "Reign.Core", "ClassName"));
			//var resource = assembly.GetManifestResourceStream("File.xml");
			//if (resource == null) Debug.ThrowError("Streams", "Resource not found: " + url);
			//return resource;

			#if WP7
			var match = Regex.Match(url, ";");
			if (match.Success) return OpenResource(url);
			else return OpenFile(url);
			#else
			return OpenFile(url);
			#endif
		}

		public static Stream OpenFile(string fileName)
		{
			#if OSX || iOS
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
			foreach (var file in files)
			{
				if (fileName == file.FileName)
				{
					return new MemoryStream(file.Data);
				}
			}
			Debug.ThrowError("Streams", "Could not find file: " + fileName);
			return new MemoryStream();
			#elif METRO
			throw new NotImplementedException();
			#else
			return new FileStream(fileName, FileMode.Open, FileAccess.Read);
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

		#if WP7
		public static Stream OpenResource(string resourceName)
		{
			var resource = System.Windows.Application.GetResourceStream(new Uri(resourceName, UriKind.Relative));
			if (resource == null) Debug.ThrowError("Streams", "Resource not found: " + resourceName);
			return resource.Stream;
		}
		#endif

		public static string StripFileExt(string fileName)
		{
			var match = Regex.Match(fileName, @".*\.");
			if (!string.IsNullOrEmpty(match.Value))
			{
				fileName = match.Value.Substring(0, match.Value.Length-1);
			}

			return fileName;
		}
		
		public static string GetFileDirectory(string fileName)
		{
			var match = Regex.Match(fileName, @".*/|\\");
			if (!string.IsNullOrEmpty(match.Value))
			{
				fileName = match.Value.Substring(0, match.Value.Length-1);
			}

			return fileName;
		}
		
		public static string GetFileNameWithExt(string fileName)
		{
			var match = Regex.Match(fileName, @".*/|\\");
			if (!string.IsNullOrEmpty(match.Value))
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

		public static int MakeFourCC(char ch0, char ch1, char ch2, char ch3)
		{
			return (((int)(byte)(ch0)) | ((int)(byte)(ch1) << 8) | ((int)(byte)(ch2) << 16) | ((int)(byte)(ch3) << 24));
		}
	}
}