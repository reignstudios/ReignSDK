#include "GC.h"

namespace System
{
	// ======================================
	// GC_HeapNode
	// ======================================
	GC_HeapNode::GC_HeapNode(Type* typeInfo, void* data)
	{
		Alive = false;
		this->Next = NULL;
		this->TypeInfo = typeInfo;
		this->Data = data;
	}

	GC_HeapNode::~GC_HeapNode()
	{
		if (Data != NULL)
		{
			free(Data);
			Data = NULL;
		}
	}

	// ======================================
	// GC_RootNode
	// ======================================
	GC_RootNode::GC_RootNode(void** data)
	{
		this->Next = NULL;
		this->Data = data;
	}

	// ======================================
	// GC
	// ======================================
	void GC::Init()
	{
		Heap = NULL;
		LastHeapNode = NULL;
		HeapSize = 0;
		Root = NULL;
	}

	void GC::AddHeapObject(Type* type, void* data)
	{
		auto obj = new GC_HeapNode(type, data);
		if (Heap == NULL) Heap = obj;
		else LastHeapNode->Next = obj;
		LastHeapNode = obj;
		++HeapSize;
	}

	void GC::AddRootObjectPtrToLastHeapNode(void** data)
	{
		auto obj = new GC_RootNode(data);
		if (Root == NULL) Root = obj;
		else LastRootNode->Next = obj;
		LastRootNode = obj;
	}

	GC_HeapNode* GC::findRootHeap(void* data)
	{
		if (data == NULL) return NULL;
		for (GC_HeapNode* node = Heap; node != NULL; node = node->Next)
		{
			if (data == node->Data)
			{
				return node;
			}
		}

		return NULL;
	}

	void GC::mark()
	{
		GC_RootNode* root = Root;
		GC_RootNode* prevNode = NULL;
		while (root != NULL)
		{
			auto nextNode = root->Next;
			auto node = findRootHeap(root->Data);
			if (node == NULL)
			{
				prevNode->Next = root->Next;
				if (LastRootNode == root) LastRootNode = prevNode;// if end root obj, set last node to prev
				delete root;
				root = nextNode;
				continue;
			}

			node->Alive = true;
			markHeapStack(node);

			prevNode = root;
			root = nextNode;
		}
	}

	void GC::markHeapStack(GC_HeapNode* startNode)
	{
		auto info = startNode->TypeInfo;
		for (int i = 0; i != info->TypeInfosCount; ++i)
		{
			unsigned char* data = reinterpret_cast<unsigned char*>(startNode->Data) + info->TypeInfoOffsets[i];
			void* ptr = (void*)*reinterpret_cast<size_t*>(data);
			for (GC_HeapNode* node = Heap; node != NULL; node = node->Next)
			{
				if (node->Alive || node == startNode) continue;

				if (ptr == node->Data)
				{
					node->Alive = true;
					markHeapStack(node);
				}
			}
		}
	}

	void GC::sweep()
	{
		GC_HeapNode* node = Heap;
		GC_HeapNode* prevNode = NULL;
		while (node != NULL)
		{
			auto nextNode = node->Next;
			if (!node->Alive)// if obj not referenced, free memory
			{
				if (prevNode != NULL) prevNode->Next = node->Next;// set prev node to next obj
				if (LastHeapNode == node) LastHeapNode = prevNode;// if end heap obj, set last node to prev
				--HeapSize;
				delete node;
			}
			else
			{
				prevNode = node;
				node->Alive = false;// reset mark value
			}

			node = nextNode;
		}
	}

	void GC::Collect()
	{
		mark();
		sweep();
	}
}