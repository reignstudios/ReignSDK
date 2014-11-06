using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Reign.Compiler
{
	public abstract class Node
	{
		public string Label;
		public List<Node> Children;
		public Node Next, Prev;
		internal CSharpSyntaxNode syntaxNode;
		protected SemanticModel semanticModel;

		public static Node New(CompilerSolution compiler, CSharpSyntaxNode syntaxNode, SemanticModel semanticModel)
		{
			switch (compiler.baseOutputType)
			{
				case CompilerBaseOutputTypes.Cpp:
					var node = new CppNode();
					node.init(compiler, syntaxNode, semanticModel);
					return node;

				default: throw new Exception("Unsuported Node base type: " + compiler.baseOutputType);
			}
		}

		private void init(CompilerSolution compiler, CSharpSyntaxNode syntaxNode, SemanticModel semanticModel)
		{
			this.syntaxNode = syntaxNode;
			this.semanticModel = semanticModel;

			//// create label
			Label = "???";
			//Label = syntaxNode.Role.ToString() + ": " + syntaxNode.GetType().Name;
			//bool hasProperties = false;
			//foreach (var p in syntaxNode.GetType().GetProperties())
			//{
			//	if (p.Name == "NodeType" || p.Name == "IsNull" || p.Name == "IsFrozen" || p.Name == "HasChildren") continue;
			//	if (p.PropertyType == typeof(string) || p.PropertyType.IsEnum || p.PropertyType == typeof(bool))
			//	{
			//		if (!hasProperties)
			//		{
			//			hasProperties = true;
			//			Label += " (";
			//		}
			//		else
			//		{
			//			Label += ", ";
			//		}

			//		Label += p.Name;
			//		Label += " = ";

			//		try
			//		{
			//			object val = p.GetValue(syntaxNode, null);
			//			Label += val != null ? val.ToString() : "**null**";
			//		}
			//		catch (TargetInvocationException ex)
			//		{
			//			Label += "**" + ex.InnerException.GetType().Name + "**";
			//		}
			//	}
			//}

			//if (hasProperties) Label += ")";

			// create childeren
			Children = new List<Node>();
			Node lastNode = null;
			foreach (var child in syntaxNode.ChildNodes())
			{
				var newNode = New(compiler, (CSharpSyntaxNode)child, semanticModel);
				if (lastNode != null) lastNode.Next = newNode;
				newNode.Prev = lastNode;
				Children.Add(newNode);
				lastNode = newNode;
			}
		}

		public virtual void Compile(StreamWriter writer, int mode)
		{
			foreach (var child in Children) child.Compile(writer, mode);
		}
	}
}
