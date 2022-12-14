#include "InputSystem.h"
#include <Windows.h>

InputSystem* InputSystem::m_InputSystem = NULL;

InputSystem::InputSystem()
{
}

InputSystem::~InputSystem()
{
	InputSystem::m_InputSystem = NULL;
}

void InputSystem::update()
{
	POINT currentMousePos = {};
	GetCursorPos(&currentMousePos);
	if (m_IsFirstFrame)
	{
		m_OldMousePos = Point(currentMousePos.x, currentMousePos.y);
		m_IsFirstFrame = false;
	}

	if (currentMousePos.x != m_OldMousePos.X || currentMousePos.y != m_OldMousePos.Y)
	{
		for (const auto& listener: m_Listeners)
		{
			listener->onMouseMove(Point(currentMousePos.x, currentMousePos.y));
		}
	}
	m_OldMousePos = Point(currentMousePos.x, currentMousePos.y);

	if (GetKeyboardState(m_Keys))
	{
		for (unsigned int i = 0; i < 256; i++)
		{
			if (m_Keys[i] & 0x80) // key is down
			{
				for (const auto& listener : m_Listeners)
				{
					if (m_LastFrameKeys[i] != m_Keys[i])
					{
						if (i == VK_LBUTTON) listener->onLeftMouseDown(Point(currentMousePos.x, currentMousePos.y));
						else if (i == VK_RBUTTON) listener->onRightMouseDown(Point(currentMousePos.x, currentMousePos.y));
						else if (i == VK_MBUTTON) listener->onMiddleMouseDown(Point(currentMousePos.x, currentMousePos.y));
					}
					listener->onKeyDown(i);
				}
			}
			else // key is up
			{
				if (m_LastFrameKeys[i] != m_Keys[i])
				{
					for (const auto& listener : m_Listeners)
					{
						if (m_LastFrameKeys[i] != m_Keys[i])
						{
							if (i == VK_LBUTTON) listener->onLeftMouseUp(Point(currentMousePos.x, currentMousePos.y));
							else if (i == VK_RBUTTON) listener->onRightMouseUp(Point(currentMousePos.x, currentMousePos.y));
							else if (i == VK_MBUTTON) listener->onMiddleMouseUp(Point(currentMousePos.x, currentMousePos.y));
						}
						listener->onKeyUp(i);
					}
				}
			}
		}

		memcpy(m_LastFrameKeys, m_Keys, sizeof(unsigned char) * 256);
	}
}

void InputSystem::addListener(InputListener* listener)
{
	m_Listeners.insert(listener);
}

void InputSystem::removeListener(InputListener* listener)
{
	m_Listeners.erase(listener);
}

void InputSystem::setCursorPosition(const Point& pos)
{
	SetCursorPos(pos.X, pos.Y);
}

void InputSystem::showCursor(bool show)
{
	ShowCursor(show);
}

InputSystem* InputSystem::get()
{
	return InputSystem::m_InputSystem;
}

void InputSystem::create()
{
	if (InputSystem::m_InputSystem) throw std::exception("InputSystem is a singleton. create() should only be called once.");
	InputSystem::m_InputSystem = new InputSystem();
}

void InputSystem::release()
{
	if (!InputSystem::m_InputSystem) return;
	delete InputSystem::m_InputSystem;
}
