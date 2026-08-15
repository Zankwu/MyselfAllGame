using Godot;
using static Character;

public partial class EnemyData : Resource
{
    public CharacterType character_type;
    public Vector2 global_position;

    public EnemyData(CharacterType tYPE,Vector2 position)
    {
        character_type = tYPE;
        global_position = position;
    }
}