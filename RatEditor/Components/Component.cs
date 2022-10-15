using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RatEditor.Components
{
    public class Component : ViewModelBase
    {
        public GameEntity Owner { get; private set; }

        public Component(GameEntity owner)
        {
            Debug.Assert(owner != null);
            Owner = owner;
        }
    }
}
