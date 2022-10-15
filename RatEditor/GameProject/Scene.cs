using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using RatEditor.Components;
using RatEditor.Utils;
using System.Windows.Input;

namespace RatEditor.GameProject
{
    [DataContract]
    public class Scene : ViewModelBase
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
        public Project Project { get; private set; }

        private bool _isActive;
        [DataMember]
        public bool IsActive {
            get => _isActive;
            set {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        [DataMember(Name = nameof(GameEntities))]
        private ObservableCollection<GameEntity> _gameEntities = new ObservableCollection<GameEntity>();
        public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set; }

        public ICommand AddGameEntityCommand { get; private set; }
        public ICommand RemoveGameEntityCommand { get; private set; }

        public Scene(Project project, string name)
        {
            Debug.Assert(project != null);
            Project = project;
            Name = name;
            OnDeserialized(new StreamingContext());
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_gameEntities != null)
            {
                GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
                OnPropertyChanged(nameof(GameEntities));
            }

            AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                AddGameEntity(x);
                var entityIndex = _gameEntities.Count - 1;

                Project.UndoRedo.Add(new UndoRedoAction(
                    $"Add {x.Name} to {Name}",
                    () => RemoveGameEntity(x),
                    () => _gameEntities.Insert(entityIndex, x)
                ));
            });

            RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                var entityIndex = _gameEntities.IndexOf(x);
                RemoveGameEntity(x);

                Project.UndoRedo.Add(new UndoRedoAction(
                    $"Remove {x.Name} from {Name}",
                    () => _gameEntities.Insert(entityIndex, x),
                    () => RemoveGameEntity(x)
                ));
            });
        }

        private void AddGameEntity(GameEntity entity)
        {
            Debug.Assert(!_gameEntities.Contains(entity));
            _gameEntities.Add(entity);
        }

        private void RemoveGameEntity(GameEntity entity)
        {
            Debug.Assert(_gameEntities.Contains(entity));
            _gameEntities.Remove(entity);
        }
    }
}
