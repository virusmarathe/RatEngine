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
                    ValidateProjectPath();
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
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private bool _isValid;
        public bool IsValid {
            get => _isValid;
            set {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        private string _errorMsg;
        public string ErrorMsg {
            get => _errorMsg;
            set {
                if (_errorMsg != value)
                {
                    _errorMsg = value;
                    OnPropertyChanged(nameof(ErrorMsg));
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
                ValidateProjectPath();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Logger.Log(MessageType.Error, $"Failed to load in project templates");
                throw;
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
                Logger.Log(MessageType.Error, $"Failed to create project templates");
                throw;
            }
        }

        private bool ValidateProjectPath()
        {
            var path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path)) path += @"\";
            path += $@"{ProjectName}\";

            IsValid = false;
            ErrorMsg = "";

            if (string.IsNullOrWhiteSpace(ProjectName.Trim())) ErrorMsg = "Type in a project name.";
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) ErrorMsg = "Invalid character(s) used in project name.";
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim())) ErrorMsg = "Select a valid project folder.";
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1) ErrorMsg = "Invalid character(s) used in project path.";
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any()) ErrorMsg = "Selected project folder already exists and is not empty";
            else IsValid = true;

            return IsValid;
        }

        public string CreateProjectFiles(ProjectTemplate template)
        {
            ValidateProjectPath();
            if (!IsValid) return string.Empty;

            if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += @"\";
            var path = $@"{ProjectPath}{ProjectName}\";

            try
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                foreach(var folder in template.Folders)
                {
                    Directory.CreateDirectory(Path.Combine(path, folder));
                }
                var dirInfo = new DirectoryInfo(path + @".Rat\");
                dirInfo.Attributes |= FileAttributes.Hidden;
                File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
                File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

                // uncomment these when re-creating project templates
                //var project = new Project(ProjectName, path);
                //Serializer.ToFile(project, project.FullPath);

                var projectXML = File.ReadAllText(template.ProjectFilePath);
                projectXML = string.Format(projectXML, ProjectName, path);
                var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                File.WriteAllText(projectPath, projectXML);

                return path;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to create project templates");
                throw;
            }
        }
    }
}
