using System.Collections.Generic;
using System;
using System.Reflection;

namespace Reign.Core
{
	public class ConstructorParam
	{
		public Type Type;
		public object Value;

		public ConstructorParam(Type type, object value)
		{
			Type = type;
			Value = value;
		}
	}

	public interface DisposableI : IDisposable
	{
		bool IsDisposed {get;}

		new void Dispose();
		void DisposeReference();
		T FindChild<T>(string methodName, params ConstructorParam[] methodParameters) where T : Disposable;
		T FindParent<T>() where T : Disposable;
		T FindParentOrSelf<T>() where T : Disposable;
		T FindParentOrSelfWithException<T>() where T : Disposable;
	}

	public abstract class Disposable : DisposableI
	{
		#region Properties
		public bool IsDisposed {get; private set;}
		private Disposable parent;
		public Disposable Parent {get{return parent;}}
		private List<Disposable> children;
		protected uint referenceCount;
		private Dictionary<string, object> constructorParameters;
		#endregion

		#region Constructors
		protected Disposable(DisposableI parent)
		{
			init(parent, null);
		}

		protected Disposable(DisposableI parent, Type type, params ConstructorParam[] constructorParameters)
		{
		    init(parent, createDictionaryFromConstructor(type, constructorParameters));
		}

		private void init(DisposableI parent, Dictionary<string,object> constructorParameters)
		{
			Disposable disposable = null;
			if (parent != null)
			{
				disposable = parent as Disposable;
				if (disposable == null) Debug.ThrowError("Disposable", "Parent object must be a Disposable class object, NOT just an interface");
			}

			this.parent = disposable;
			if (this.parent != null) this.parent.children.Add(this);
			children = new List<Disposable>();
			referenceCount = 1;
			this.constructorParameters = constructorParameters;
		}

		protected void disposeChilderen()
		{
			if (children != null)
			{
				var childrenTEMP = new List<Disposable>();
				foreach (var child in children) childrenTEMP.Add(child);
				foreach (var child in childrenTEMP) child.Dispose();
				children = null;
			}
		}

		~Disposable() {Dispose();}
		public virtual void Dispose()
		{
			disposeChilderen();
			if (parent != null)
			{
				if (parent.children != null)
				{
					parent.children.Remove(this);
				}
				parent = null;
			}

			IsDisposed = true;
		}

		public void DisposeReference()
		{
			--referenceCount;
			if (referenceCount == 0) Dispose();
		}
		#endregion

		#region Methods
		private Dictionary<string,object> createDictionaryFromConstructor(Type type, params ConstructorParam[] constructorParameters)
		{
		    var types = new Type[constructorParameters.Length];
		    for (int i = 0; i != constructorParameters.Length; ++i)
		    {
		        types[i] = constructorParameters[i].Type;
		    }
			
			#if WINRT
			ConstructorInfo constructor = null;
			foreach (var c in type.GetTypeInfo().DeclaredConstructors)
			{
				bool pass = true;
				var args = c.GetGenericArguments();
				if (types.Length == args.Length)
				{
					for (int i = 0; i != args.Length; ++i)
					{
						if (types[i] != args[i])
						{
							pass = false;
							break;
						}
					}
				}
				else
				{
					pass = false;
				}

				if (pass)
				{
					constructor = c;
					break;
				}
			}
			#else
		    var constructor = type.GetConstructor(types);
			#endif
		    if (constructor == null) Debug.ThrowError("Disposable", "Invalid constructor parameters");

		    var perameters = constructor.GetParameters();
		    if (constructorParameters.Length != perameters.Length) Debug.ThrowError("Disposable", "ConstructorParameters count do not match ConstructorInfo");

		    var dictionary = new Dictionary<string,object>();
		    for (int i = 0; i != constructorParameters.Length; ++i)
		    {
		        dictionary.Add(perameters[i].Name, constructorParameters[i].Value);
		    }

		    return dictionary;
		}

		protected Dictionary<string,object> createDictionaryFromMethod(Type type, string methodName, params ConstructorParam[] methodParameters)
		{
			var types = new Type[methodParameters.Length];
			for (int i = 0; i != methodParameters.Length; ++i)
			{
			    types[i] = methodParameters[i].Type;
			}

			#if WINRT
			var methodInfo = type.GetRuntimeMethod(methodName, types);
			#else
			var methodInfo = type.GetMethod(methodName, types);
			#endif
			if (methodInfo == null) Debug.ThrowError("Disposable", "Invalid method name.");

			var perameters = methodInfo.GetParameters();
			if (methodParameters.Length != perameters.Length) Debug.ThrowError("Disposable", "MethodParameters count do not match MethedInfo.");

			var dictionary = new Dictionary<string,object>();
			for (int i = 0; i != methodParameters.Length; ++i)
			{
				dictionary.Add(perameters[i].Name, methodParameters[i]);
			}

			return dictionary;
		}

		public T FindChild<T>(string methodName, params ConstructorParam[] methodParameters) where T : Disposable
		{
			return findChild<T>(createDictionaryFromMethod(typeof(T), methodName, methodParameters));
		}

		private T findChild<T>(Dictionary<string,object> constructorParameters) where T : Disposable
		{
			if ((this as T != null) && this.constructorParameters != null && this.constructorParameters.Count == constructorParameters.Count)
			{
				bool pass = true;
				foreach (var item in constructorParameters)
				{
					if (this.constructorParameters.ContainsKey(item.Key))
					{
						if (!this.constructorParameters[item.Key].Equals(item.Value))
						{
							pass = false;
							break;
						}
					}
					else
					{
						pass = false;
						break;
					}
				}

				if (pass) return (T)this;
			}

			foreach (var child in children)
			{
				var found = child.findChild<T>(constructorParameters);
				if (found != null) return found;
			}

			return null;
		}

		public T FindParent<T>() where T : Disposable
		{
			if (parent == null) return null;
			if (parent.GetType() == typeof(T)) return (T)parent;
			return parent.FindParent<T>();
		}

		public T FindParentOrSelf<T>() where T : Disposable
		{
			if (this.GetType() == typeof(T)) return (T)this;
			return FindParent<T>();
		}

		public T FindParentOrSelfWithException<T>() where T : Disposable
		{
			if (this.GetType() == typeof(T)) return (T)this;
			var parent = FindParent<T>();
			if (parent == null)
			{
				Debug.ThrowError("Disposable", "Failed to find disposable parent of type: " + typeof(T).ToString());
				return null;
			}
			else
			{
				return parent;
			}
		}

		public void RemoveFromParent()
		{
			if (parent != null)
			{
				parent.children.Remove(this);
				parent = null;
			}
		}

		public void Transplant(DisposableI newParent)
		{
			RemoveFromParent();
			((Disposable)newParent).children.Add(this);
		}
		#endregion
	}
}