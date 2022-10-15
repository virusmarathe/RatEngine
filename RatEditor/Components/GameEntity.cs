using RatEditor.GameProject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace RatEditor.Components
{
    [DataContract]
    public class GameEntity : ViewModelBase
    {
        private string _name;
        [DataMember]
        public string Name {
            get => _name;
            set {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        [DataMember]
        public Scene ParentScene { get; private set; }

        [DataMember(Name = nameof(Components))]
        private ObservableCollection<Component> _components = new ObservableCollection<Component>();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }

        public GameEntity(Scene parent)
        {
            Debug.Assert(parent != null);
            ParentScene = parent;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {

        }
    }
}
