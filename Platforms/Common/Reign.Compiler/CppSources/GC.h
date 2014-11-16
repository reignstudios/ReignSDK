#pragma once
#include "System.h"
using namespace System;

#include <iostream>
#include <vector>

namespace System
{
	// ======================================
	// GC_HeapNode
	// ======================================
	class GC_HeapNode
	{
		// fiels
		public: bool Alive;
		public: GC_HeapNode* Next;
		public: Type* TypeInfo;
		public: void* Data;

		// constructors
		public: GC_HeapNode(Type* typeInfo, void* data);
		public: ~GC_HeapNode();
	};

	// ======================================
	// GC_RootNode
	// ======================================
	class GC_RootNode
	{
		// fields
		public: GC_RootNode* Next;
		public: void** Data;

		// constructors
		public: GC_RootNode(void** data);
	};

	// ======================================
	// gc
	// ======================================
	class GC
	{
		// fields
		public: static GC_HeapNode* Heap, *LastHeapNode;
		public: static unsigned int HeapSize;
		public: static GC_RootNode* Root, *LastRootNode;

		// constructors
		public: static void Init();

		// methods
		public: static void AddHeapObject(Type* type, void* data);
		public: static void AddRootObjectPtrToLastHeapNode(void** data);
		private: static GC_HeapNode* findRootHeap(void* data);
		private: static void mark();
		private: static void markHeapStack(GC_HeapNode* startNode);
		private: static void sweep();
		public: static void Collect();
	};
}