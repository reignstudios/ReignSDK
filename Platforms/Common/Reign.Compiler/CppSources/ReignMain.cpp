#include "C:\Users\Andrew\Reign\Tests\ReignCompiler\OutputApp\TestApp\Program.h"
#include "ReignMain.h"
#include "GC.h"

#include <tchar.h>
#include <iostream>
//using namespace std;

int _tmain(int argc, _TCHAR* argv[])
{
	System::GC::Init();
	TestApp::Program::Main(NULL);
	//System::GC::Collect();
	std::cout << "HeapSize: " << System::GC::HeapSize;
	return 0;
}

