using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using RatEditor.Utils;

namespace RatEditor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }

        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
        public string IconFilePath { get; set; }
        public string ScreenshotFilePath { get; set; }
        public string ProjectFilePath { get; set; }
    }

    class CreateProject : ViewModelBase
    {
        // TODO: get the path from the installation location
        private readonly string _templatePath = @"..\..\RatEditor\ProjectTemplates\";
        private string _projectName = "NewProject";

        public string ProjectName {
            get => _projectName;
            set { 
                if (_projectName != value)
                {
                    _projectName = value;
                    OnPropertyChanged(nameof(ProjectName));
                }
            }
        }

        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\RatEngineProjects\";
        public string ProjectPath {
            get => _projectPath;
            set {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        public CreateProject()
        {
            // CreateTemplates(); // uncomment this line if needing to update project template format
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);

            try
            {
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                foreach (var file in templateFiles)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));
                    _projectTemplates.Add(template);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODO: log error
            }
        }

        private void CreateTemplates()
        {
            try
            {
                string[] dirs = Directory.GetDirectories(_templatePath, "*", SearchOption.TopDirectoryOnly);

                foreach(string dir in dirs)
                {
                    string pathString = Path.Combine(dir, "template.xml");
                    Debug.WriteLine(pathString);
                    var template = new ProjectTemplate()
                    {
                        ProjectType = new DirectoryInfo(dir).Name,
                        ProjectFile = "project.rat",
                        Folders = new List<string>() { ".Rat", "Content", "GameCode" }
                    };

                    Serializer.ToFile(template, pathString);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODO: log error
            }
        }
    }
}
