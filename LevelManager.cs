using System.Collections.Generic;
using Godot;

namespace MadMula.LevelManager
{
    public partial class LevelManager : Node
    {
        private static readonly string _levelsDirectoryPath = "res://scenes/levels/";

        private static readonly string _levelSelectionButtonAbsolutePath = "/root/Main/UI/LevelSelection/";

        public Node CurrentLevel { get; set; }

        public readonly Dictionary<NodePath, string> LevelRepository = new([
            new KeyValuePair<NodePath, string>(_levelSelectionButtonAbsolutePath + "BasicLevel", _levelsDirectoryPath + "basic_level.tscn"),
            new KeyValuePair<NodePath, string>(_levelSelectionButtonAbsolutePath + "SecondLevel", _levelsDirectoryPath + "second_level.tscn"),

        ]);

        public override void _Ready()
        {
            // CurrentLevel = GetNode("/root/Main/LevelContainer").GetChild(-1);
            // AttachButtonPressedSignalCallbacks();
        }

        public void GoToScene(string path)
        {
            CallDeferred(MethodName.DeferredGoToScene, path);
        }

        public void DeferredGoToScene(string path)
        {
            // CurrentLevel?.Free();

            var newLevel = GD.Load<PackedScene>(path);

            CurrentLevel = newLevel.Instantiate();

            // GetTree().Root.AddChild(CurrentLevel);
            GetNode("/root/Main/LevelContainer").AddChild(CurrentLevel); // LevelManager & "master" node are siblings

            // GetTree().CurrentScene = CurrentLevel;
        }

        public void AttachButtonPressedSignalCallbacks()
        {
            // consider putting UI control outside of master node in the future
            GetNode<Button>("/root/Main/UI/LevelSelection/BasicLevel").Pressed += () =>
            {
                LevelRepository.TryGetValue("/root/Main/UI/LevelSelection/BasicLevel", out string path);
                GetNode<HBoxContainer>("/root/Main/UI/LevelSelection").Hide();
                GoToScene(path);
            };
            GetNode<Button>("/root/Main/UI/LevelSelection/SecondLevel").Pressed += () =>
            {
                LevelRepository.TryGetValue("/root/Main/UI/LevelSelection/SecondLevel", out string path);
                GetNode<HBoxContainer>("/root/Main/UI/LevelSelection").Hide();
                GoToScene(path);
            };
        }

    }
}