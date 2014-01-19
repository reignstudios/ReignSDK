using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using System.Reflection;

namespace Reign.Compiler
{
	public abstract class Node
	{
		public readonly string Label;
		public List<Node> Children;
		public Node Next, Prev;
		protected AstNode astNode;

		public static Node New(CompilerBase compiler, AstNode astNode)
		{
			switch (compiler.baseOutputType)
			{
				case CompilerBaseOutputTypes.Cpp: return new CppNode(compiler, astNode);
				default: throw new Exception("Unsuported Node base type: " + compiler.baseOutputType);
			}
		}

		protected Node(CompilerBase compiler, AstNode astNode)
		{
			this.astNode = astNode;

			// create label
			Label = astNode.Role.ToString() + ": " + astNode.GetType().Name;
			bool hasProperties = false;
			foreach (var p in astNode.GetType().GetProperties())
			{
				if (p.Name == "NodeType" || p.Name == "IsNull" || p.Name == "IsFrozen" || p.Name == "HasChildren") continue;
				if (p.PropertyType == typeof(string) || p.PropertyType.IsEnum || p.PropertyType == typeof(bool))
				{
					if (!hasProperties)
					{
						hasProperties = true;
						Label += " (";
					}
					else
					{
						Label += ", ";
					}

					Label += p.Name;
					Label += " = ";

					try
					{
						object val = p.GetValue(astNode, null);
						Label += val != null ? val.ToString() : "**null**";
					}
					catch (TargetInvocationException ex)
					{
						Label += "**" + ex.InnerException.GetType().Name + "**";
					}
				}
			}

			if (hasProperties) Label += ")";

			// create childeren
			Children = new List<Node>();
			Node lastNode = null;
			foreach (var child in astNode.Children)
			{
				var newNode = New(compiler, child);
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
