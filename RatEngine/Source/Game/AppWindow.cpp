#include "AppWindow.h"
#include <Windows.h>
#include "Vector3.h"
#include "Vector2.h"
#include "Matrix4x4.h"
#include "InputSystem.h"
#include <iostream>
#include "Debug.h"
#include "Material.h"

AppWindow::AppWindow() : Window(), m_SwapChain(NULL), attachParentSystem(&ecs)
{
}

AppWindow::~AppWindow()
{
}

void AppWindow::onCreate()
{
	Window::onCreate();

	RECT rc = getClientWindowRect();
	m_SwapChain = GraphicsEngine::get()->getRenderSystem()->createSwapChain(m_hwnd, rc.right - rc.left, rc.bottom - rc.top);

	InputSystem::get()->showCursor(false);

	m_World = new InteractionWorld(ecs);
	ecs.addListener(m_World);

	MeshRendererComponent comp;
	TransformComponent trans;
	SimpleMotionComponent motionComp;
	RotateTimerComponent rotComp;
	FlyCamComponent flyCamComp;
	AttachToParentComponent attachParentComp;
	ColliderComponent colliderComp;

	rotComp.speed = 0.707f;
	rotComp.rotateEulerAngles = Vector3(0, 1, 0);
	trans.transform.setIdentity();
	directionalLight = ecs.makeEntity(trans, rotComp);

	trans.transform.setIdentity();
	trans.transform.setTranslation(Vector3(0, 0, -1));
	flyCamComp.speed = 2.0f;
	camera = ecs.makeEntity(trans, flyCamComp);

	comp.mesh = GraphicsEngine::get()->getMeshManager()->createMeshFromFile(L"Assets/Meshes/statue.obj");
	comp.m_Texture = GraphicsEngine::get()->getTextureManager()->createTextureFromFile(L"Assets/Textures/wood.jpg");
	comp.m_Material = GraphicsEngine::get()->getMaterialManager()->createMaterialFromFile(L"Assets/Materials/default.mat");
	trans.transform.setIdentity();
	trans.transform.setTranslation(Vector3(1.0f, -0.1f, -1.5f));
	rotComp.speed = -2.1f;
	statue = ecs.makeEntity(trans, comp, rotComp, colliderComp);

	comp.mesh = GraphicsEngine::get()->getMeshManager()->createMeshFromFile(L"Assets/Meshes/teapot.obj");
	comp.m_Texture = GraphicsEngine::get()->getTextureManager()->createTextureFromFile(L"Assets/Textures/brick.png");
	comp.m_Material = GraphicsEngine::get()->getMaterialManager()->createMaterialFromFile(L"Assets/Materials/default.mat");
	trans.transform.setIdentity();
	trans.transform.setTranslation(Vector3(-1.5f, 0, 0));
	motionComp.velocity = Vector3(1.0f, 0.0f, 0.0f);
	teapot = ecs.makeEntity(trans, comp, motionComp, colliderComp);

	comp.mesh = GraphicsEngine::get()->getMeshManager()->createMeshFromFile(L"Assets/Meshes/sphere.obj");
	comp.m_Texture = GraphicsEngine::get()->getTextureManager()->createTextureFromFile(L"Assets/Textures/sky.jpg");
	comp.m_Material = GraphicsEngine::get()->getMaterialManager()->createMaterialFromFile(L"Assets/Materials/UnlitTexture.mat");
	comp.backFaceCulled = false;
	trans.transform.setIdentity();
	trans.transform.setScale(Vector3(100.0f, 100.0f, 100.0f));
	attachParentComp.parent = camera;
	skybox = ecs.makeEntity(trans, comp, attachParentComp);

	inputSystems.addSystem(flyCamSystem);
	mainSystems.addSystem(simpleMotionSystem);
	mainSystems.addSystem(eulerRotatorSystem);
	mainSystems.addSystem(attachParentSystem);
	renderingSystems.addSystem(meshRendererSystem);

	constant data;
	ecs.getComponent<MeshRendererComponent>(statue)->constantBuffer = GraphicsEngine::get()->getRenderSystem()->createConstantBuffer(&data, sizeof(constant));
	ecs.getComponent<MeshRendererComponent>(teapot)->constantBuffer = GraphicsEngine::get()->getRenderSystem()->createConstantBuffer(&data, sizeof(constant));
	ecs.getComponent<MeshRendererComponent>(skybox)->constantBuffer = GraphicsEngine::get()->getRenderSystem()->createConstantBuffer(&data, sizeof(constant));

	meshRendererSystem.clientWindowRect = rc;
	flyCamSystem.clientWindowRect = rc;

	meshRendererSystem.m_CameraTransform = &ecs.getComponent<TransformComponent>(camera)->transform;
	meshRendererSystem.m_LightTransform = &ecs.getComponent<TransformComponent>(directionalLight)->transform;

	ecs.removeComponent<ColliderComponent>(statue);
}

void AppWindow::onUpdate()
{
	// input
	InputSystem::get()->update();
	ecs.updateSystems(inputSystems, m_DeltaTime);

	// update
	ecs.updateSystems(mainSystems, m_DeltaTime);

	m_World->processInteractions(m_DeltaTime);
	
	// post update test-stuff
	float teapotXPos = ecs.getComponent<TransformComponent>(teapot)->transform.position().x;
	float teapotXVel = ecs.getComponent<SimpleMotionComponent>(teapot)->velocity.x;

	if (teapotXPos > 2.0f && teapotXVel > 0)
	{
		ecs.getComponent<SimpleMotionComponent>(teapot)->velocity = Vector3(-1.0f, 0.0f, 0.0f);
	}
	else if (teapotXPos <= -2.0f && teapotXVel < 0)
	{
		ecs.getComponent<SimpleMotionComponent>(teapot)->velocity = Vector3(1.0f, 0.0f, 0.0f);
	}

	// render
	DeviceContext* context = GraphicsEngine::get()->getRenderSystem()->getImmediateDeviceContext();
	context->clearRenderTargetColor(m_SwapChain, 0.1f, 0.1f, 0.6f, 1);
	RECT rc = this->getClientWindowRect();
	context->setViewportSize((FLOAT)(rc.right - rc.left), (FLOAT)(rc.bottom - rc.top));
	ecs.updateSystems(renderingSystems, m_DeltaTime);
	m_SwapChain->present(false);
}

void AppWindow::onDestroy()
{
	Window::onDestroy();
	delete m_World;
	delete m_SwapChain;
	GraphicsEngine::get()->release();
}

void AppWindow::onFocus()
{
	InputSystem::get()->addListener(&flyCamSystem);
}

void AppWindow::onKillFocus()
{
	InputSystem::get()->removeListener(&flyCamSystem);
}