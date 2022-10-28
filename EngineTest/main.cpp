// Project to test out game engine

#pragma comment(lib, "engine.lib")

#define TEST_ENTITY_COMPONENTS 1

#if TEST_ENTITY_COMPONENTS
#include "EntityComponentIDTest.h"
#else
#error One of the tests need to be enabled
#endif

int main()
{
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif

	entity_id_test test{};

	if (test.init())
	{
		test.run();
	}
	test.shutdown();
}