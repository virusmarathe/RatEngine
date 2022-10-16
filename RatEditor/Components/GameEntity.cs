using RatEditor.GameProject;
using RatEditor.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;

namespace RatEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    class GameEntity : ViewModelBase
    {
        private bool _isEnabled = true;
        [DataMember]
        public bool IsEnabled {
            get => _isEnabled;
            set {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }


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
            _components.Add(new Transform(this));
            OnDeserialized(new StreamingContext());
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components));
            }
        }
    }

    abstract class MSEntity : ViewModelBase
    {        
        private bool _enableUpdates = true;

        private bool? _isEnabled;
        public bool? IsEnabled {
            get => _isEnabled;
            set {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        private string _name;
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

        private ObservableCollection<IMSComponent> _components = new ObservableCollection<IMSComponent>();
        public ReadOnlyObservableCollection<IMSComponent> Components { get; private set; }
        public List<GameEntity> SelectedEntities { get; }

        public MSEntity(List<GameEntity> entities)
        {
            Debug.Assert(entities?.Any() == true);
            Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
            SelectedEntities = entities;
            PropertyChanged += (s, e) => 
            {
                if (_enableUpdates)
                {
                    UpdateGameEntities(e.PropertyName);
                }
            };
        }
        protected virtual bool UpdateGameEntities(string propertyName)
        {
            switch(propertyName)
            {
                case nameof(IsEnabled): SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value); return true;
                case nameof(Name): SelectedEntities.ForEach(x => x.Name = Name); return true;
            }
            return false;
        }

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSGameEntity();
            _enableUpdates = true;
        }
        protected virtual bool UpdateMSGameEntity()
        {
            IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnabled));
            Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));

            return true;
        }

        public static bool? GetMixedValue(List<GameEntity> selectedEntities, Func<GameEntity, bool> func)
        {
            var value = func(selectedEntities.First());
            foreach (var entity in selectedEntities.Skip(1))
            {
                if (value != (func(entity))) return null;
            }

            return value;
        }
        public static string GetMixedValue(List<GameEntity> selectedEntities, Func<GameEntity, string> func)
        {
            var value = func(selectedEntities.First());
            foreach (var entity in selectedEntities.Skip(1))
            {
                if (value != (func(entity))) return null;
            }

            return value;
        }
        public static float? GetMixedValue(List<GameEntity> selectedEntities, Func<GameEntity, float> func)
        {
            var value = func(selectedEntities.First());
            foreach(var entity in selectedEntities.Skip(1))
            {
                if (!value.IsTheSameAs(func(entity))) return null;
            }

            return value;
        }

    }

    class MSGameEntity : MSEntity
    {
        public MSGameEntity(List<GameEntity> entities) : base(entities)
        {
            Refresh();
        }
    }
}
