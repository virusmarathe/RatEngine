﻿using RatEditor.GameProject;
using RatEditor.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;

namespace RatEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    public class GameEntity : ViewModelBase
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

        public ICommand RenameCommand { get; private set; }
        public ICommand EnableCommand { get; private set; }

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

            RenameCommand = new RelayCommand<string>((x)=> 
            {
                string oldName = _name;
                Name = x;

                Project.UndoRedo.Add(new UndoRedoAction($"Rename {oldName} to {x}", nameof(Name), this, oldName, x));
            },
            x => x != _name);

            EnableCommand = new RelayCommand<bool>((x) =>
            {
                bool oldVal = _isEnabled;
                IsEnabled = x;
                string enableString = _isEnabled ? "Enabled" : "Disabled";

                Project.UndoRedo.Add(new UndoRedoAction($"{enableString} {Name}", nameof(IsEnabled), this, oldVal, x));
            });
        }
    }
}
