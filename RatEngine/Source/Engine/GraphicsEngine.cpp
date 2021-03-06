#include "GraphicsEngine.h"
#include "RenderSystem.h"
#include <exception>

#pragma warning(disable: 26812)

GraphicsEngine* GraphicsEngine::m_GraphicsEngine = NULL;

GraphicsEngine::GraphicsEngine() : m_RenderSystem(NULL), m_TextureManager(NULL)
{
	try
	{
		m_RenderSystem = new RenderSystem();
	}
	catch (...) { throw std::exception("GraphicsEngine::RenderSystem not created successfully"); }

	try
	{
		m_TextureManager = new TextureManager();
	}
	catch (...) { throw std::exception("GraphicsEngine::TextureManager not created successfully"); }

	try
	{
		m_MeshManager = new MeshManager();
	}
	catch (...) { throw std::exception("GraphicsEngine::MeshManager not created successfully"); }

	try
	{
		m_MaterialManager = new MaterialManager();
	}
	catch (...) { throw std::exception("GraphicsEngine::MaterialManager not created successfully"); }
}

GraphicsEngine::~GraphicsEngine()
{
	GraphicsEngine::m_GraphicsEngine = NULL;
	delete m_MaterialManager;
	delete m_MeshManager;
	delete m_TextureManager;
	delete m_RenderSystem;
}

GraphicsEngine* GraphicsEngine::get()
{
	return m_GraphicsEngine;
}

void GraphicsEngine::create()
{
	if (GraphicsEngine::m_GraphicsEngine) throw std::exception("Graphics Engine is a singleton. create() should only be called once.");
	GraphicsEngine::m_GraphicsEngine = new GraphicsEngine();
}

void GraphicsEngine::release()
{
	if (!GraphicsEngine::m_GraphicsEngine) return;
	delete GraphicsEngine::m_GraphicsEngine;
}
