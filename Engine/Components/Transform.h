#pragma once
#include "ComponentsCommon.h"

namespace rat::transform
{
	struct init_info
	{
		f32 position[3]{};
		f32 rotation[4]{};
		f32 scale[3]{1.0f, 1.0f, 1.0f};
	};

	component create_transform(const init_info& info, game_entity::entity ent);
	void remove_transform(component comp);
}