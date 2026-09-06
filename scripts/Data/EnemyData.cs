using Godot;
using static Character;

public partial class EnemyData : Resource
{
    public CharacterType character_type;
    public Vector2 global_position;
    public State currentState;
    public int height;
    public int DROP_HEIGHT = 50;
    public int Door_Index;
    public EnemyData(CharacterType tYPE, Vector2 position,int assigin_door_index = -1)
    {
        character_type = tYPE;
        Door_Index = assigin_door_index;
        if (position.Y < 0)
        {
            height = DROP_HEIGHT;
            global_position = position + Vector2.Down * DROP_HEIGHT;
            currentState = State.DROP;
        }
        else
        {
            currentState = State.IDLE;
            global_position = position;
        }
    }
}