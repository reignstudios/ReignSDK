// ReignGC.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <vector>
using namespace std;

//--------------------------------
class ReignGC_TypeInfo
{
	public: string Name;
	public: bool IsValueType;

	public: int* InfoOffsets;//, *ReferenceInfoOffsets;
	public: ReignGC_TypeInfo** Infos;//, **ReferenceInfos;
	public: int InfosCount;//, ReferenceInfosCount;

	public: ReignGC_TypeInfo(string name, bool isValueType)
	{
		this->Name = name;
		this->IsValueType = isValueType;

		this->InfoOffsets = 0;
		this->Infos = 0;
		this->InfosCount = 0;

		/*this->ReferenceInfoOffsets = 0;
		this->ReferenceInfos = 0;
		this->ReferenceInfosCount = 0;*/
	}
};

class ReignGC_TypeInfo_Int : public ReignGC_TypeInfo
{
	public: int DefaultValue;

	public: ReignGC_TypeInfo_Int(int defaultValue, string name) : ReignGC_TypeInfo(name, true)
	{
		this->DefaultValue = defaultValue;
	}
};

ReignGC_TypeInfo_Int* ReignGC_TypeInfo_IntObj = new ReignGC_TypeInfo_Int(128, "System.Int32");

class ReignGC_HeapNode
{
	public: bool Alive;
	public: ReignGC_HeapNode* Next;
	public: ReignGC_TypeInfo* TypeInfo;
	public: void* Data;

	public: ReignGC_HeapNode(ReignGC_TypeInfo* typeInfo, void* data)
	{
		Alive = false;
		this->Next = 0;
		this->TypeInfo = typeInfo;
		this->Data = data;
	}

	public: ~ReignGC_HeapNode()
	{
		if (Data != NULL)
		{
			free(Data);
			Data = NULL;
		}
	}
};

class ReignGC_RootNode
{
	public: void** Data;

	public: ReignGC_RootNode(void** data)
	{
		this->Data = data;
	}
};

class ReignGC
{
	public: ReignGC_HeapNode* Heap, *LastHeapNode;
	public: unsigned int HeapSize;
	public: vector<ReignGC_RootNode> Roots;

	public: ReignGC()
	{
		Heap = NULL;
		LastHeapNode = NULL;
		HeapSize = 0;
	}

	public: void AddHeapObject(ReignGC_TypeInfo* type, void* data)
	{
		auto obj = new ReignGC_HeapNode(type, data);
		if (Heap == NULL) Heap = obj;
		else LastHeapNode->Next = obj;
		LastHeapNode = obj;
		++HeapSize;
	}

	public: void AddRootObjectPtrToLastHeapNode(void** data)
	{
		Roots.push_back(ReignGC_RootNode(data));
	}

	private: ReignGC_HeapNode* findRootHeap(void* data)
	{
		if (data == NULL) return NULL;
		for (ReignGC_HeapNode* node = Heap; node != NULL; node = node->Next)
		{
			if (data == node->Data)
			{
				return node;
			}
		}

		return NULL;
	}

	private: void mark()
	{
		for (int i = 0; i != Roots.size(); ++i)
		{
			auto node = findRootHeap(*Roots[i].Data);
			if (node == NULL) continue;

			node->Alive = true;
			markHeapStack(node);
		}
	}

	private: void markHeapStack(ReignGC_HeapNode* startNode)
	{
		auto info = startNode->TypeInfo;
		for (int i = 0; i != info->InfosCount; ++i)
		{
			unsigned char* data = reinterpret_cast<unsigned char*>(startNode->Data) + info->InfoOffsets[i];
			void* ptr = (void*)*reinterpret_cast<size_t*>(data);
			for (ReignGC_HeapNode* node = Heap; node != NULL; node = node->Next)
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

	private: void sweep()
	{
		ReignGC_HeapNode* node = Heap;
		ReignGC_HeapNode* prevNode = NULL;
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

	public: void Collect()
	{
		mark();
		sweep();
	}
};

ReignGC* gc = new ReignGC();

//--------------------------------
class TestC_TypeInfo : public ReignGC_TypeInfo
{
	public: ReignGC_TypeInfo_Int* c;
	public: TestC_TypeInfo* Ref;

	public: TestC_TypeInfo() : ReignGC_TypeInfo("TestC", false)
	{
		c = ReignGC_TypeInfo_IntObj;
		Ref = this;

		InfosCount = 2;
		InfoOffsets = new int[InfosCount];
		Infos = new ReignGC_TypeInfo*[InfosCount];
		Infos[0] = c; InfoOffsets[0] = 0;
		Infos[1] = Ref; InfoOffsets[1] = sizeof(int);

		/*ReferenceInfosCount = 1;
		ReferenceInfoOffsets = new int[InfosCount];
		ReferenceInfos = new ReignGC_TypeInfo*[InfosCount];
		ReferenceInfos[0] = Ref; ReferenceInfoOffsets[0] = sizeof(int);*/
	}
};

TestC_TypeInfo* TestC_TypeInfoObj = new TestC_TypeInfo();

//#pragma pack(1)
class TestC
{
	public: int c = 128;
	public: TestC* Ref;

	public: void* operator new(size_t size)
	{
		void* data = malloc(size);
		if(data == NULL) throw "Allocation Failed: No free memory";
		memset(data, 0, size);
		gc->AddHeapObject(TestC_TypeInfoObj, data);
		return data;
	}

	public: ReignGC_TypeInfo* GetType()
	{
		return TestC_TypeInfoObj;
	}

	public: TestC(TestC* ref)
	{
		this->Ref = ref;
	}
};

//--------------------------------
int _tmain(int argc, _TCHAR* argv[])
{
	cout << "Starting..." << endl;
	auto obj = new TestC(NULL); gc->AddRootObjectPtrToLastHeapNode((void**)&obj);
	auto obj2 = new TestC(NULL); gc->AddRootObjectPtrToLastHeapNode((void**)&obj2);

	obj->Ref = new TestC(NULL);
	obj->Ref->Ref = new TestC(NULL);
	obj->Ref->Ref->Ref = new TestC(NULL);

	obj2->Ref = new TestC(NULL);
	obj2->Ref->Ref = new TestC(NULL);
	obj2->Ref->Ref->Ref = new TestC(NULL);

	obj->Ref->Ref->Ref->Ref = obj2->Ref;
	//obj2->Ref->Ref->Ref->Ref = obj->Ref;

	obj->Ref = NULL;

	//obj->Ref = obj2;
	//obj2->Ref = o;

	//obj = NULL;
	//obj2 = NULL;

	//obj->Ref = obj2;
	//obj->Ref = new TestC(NULL);

	//obj = new TestC(NULL);
	//obj = NULL;

	//obj2->Ref = new TestC(NULL);
	//obj2 = obj->Ref;
	
	gc->Collect();
	cout << "Value: " << gc->HeapSize;

	return 0;
}

