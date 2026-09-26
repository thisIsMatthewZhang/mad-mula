using Godot;

namespace MadMula.LevelManager
{
    public partial class LevelManager : Node
    {
        private const string _levelsPath = "res://scenes/levels/";

        public Node CurrentLevel { get; set; }

        public void GoToScene(string levelScene)
        {
            CallDeferred(MethodName.DeferredGoToScene, levelScene);
        }

        public void DeferredGoToScene(string levelScene)
        {
            CurrentLevel.Free();

            var newLevel = GD.Load<PackedScene>(_levelsPath + levelScene);

            CurrentLevel = newLevel.Instantiate();

            // GetTree().Root.AddChild(CurrentLevel);
            GetNode("/root/Main/LevelContainer").AddChild(CurrentLevel); // LevelManager & "master" node are siblings

            GetTree().CurrentScene = CurrentLevel;
        }

    }
}