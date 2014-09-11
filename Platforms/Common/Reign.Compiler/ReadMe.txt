NOTE: Set Roslyn to release.

NOTE2: Disable Roslyn code signing.

NOTE3: Change assembly checker code.
1) Open: "roslyn\Src\Workspaces\Core\Workspace\Host\Mef\MefHostServices.cs"
2) Change: "LoadAssembly(assemblies, string.Format("Microsoft.CodeAnalysis.CSharp.Workspaces, Version={0}, Culture=neutral, PublicKeyToken={1}", assemblyVersion, publicKeyToken));"
2.1) To: "LoadAssembly(assemblies,"Microsoft.CodeAnalysis.CSharp.Workspaces");"