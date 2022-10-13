using RatEditor.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RatEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        public static string Extension { get; } = ".rat";
        [DataMember]
        public string Name { get; private set; } = "New Project";
        [DataMember]
        public string Path { get; private set; }
        public string FullPath => $"{Path}{Name}{Extension}";

        [DataMember(Name ="Scenes")]

        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        public Scene ActiveScene { get; set; }

        public static Project Current => Application.Current.MainWindow.DataContext as Project;

        public ICommand AddScene { get; private set; }
        public ICommand RemoveScene { get; private set; }

        public ICommand Undo { get; private set; }
        public ICommand Redo { get; private set; }

        public static UndoRedo UndoRedo { get; } = new UndoRedo();

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            _scenes.Add(new Scene(this, "Default Scene"));
            _scenes[0].IsActive = true;
            OnDeserialized(new StreamingContext());
        }

        public Project() { } // empty constructor for design views

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

            AddScene = new RelayCommand<object>(x =>
            {
                AddSceneInternal($"New Scene {_scenes.Count}");
                Scene addedScene = _scenes.Last();
                int sceneIndex = _scenes.Count - 1;

                UndoRedo.Add(new UndoRedoAction(
                    $"Add {addedScene.Name}",
                    () =>  RemoveSceneInternal(addedScene),
                    () =>  _scenes.Insert(sceneIndex, addedScene)
                ));
            });

            RemoveScene = new RelayCommand<Scene>(x =>
            {
                int sceneIndex = _scenes.IndexOf(x);
                RemoveSceneInternal(x);

                UndoRedo.Add(new UndoRedoAction(
                    $"Remove {x.Name}",
                    () => _scenes.Insert(sceneIndex, x),
                    () => RemoveSceneInternal(x)
                ));
            }, x => !x.IsActive);

            Undo = new RelayCommand<object>(x => UndoRedo.Undo());
            Redo = new RelayCommand<object>(x => UndoRedo.Redo());
        }

        public void Unload() { }

        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }

        public static void Save(Project project)
        {
            Serializer.ToFile(project, project.FullPath);
        }

        private void AddSceneInternal(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(this, sceneName));
        }

        private void RemoveSceneInternal(Scene scene)
        {
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
        }
    }
}
