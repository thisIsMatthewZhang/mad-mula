namespace MadMula.RandomNames;

using Godot.Collections;

public static class Names
{
    private static readonly Array<string> AllNames = new Array<string>()
    {
        "Emma",
        "Khristian",
        "Ellen",
        "Lil' Tim Tim"
    };

    public static string GiveRandomName()
    {
        return AllNames.PickRandom();
        // return AllNames[(int) (GD.Randi() % AllNames.Count)];
    }
}