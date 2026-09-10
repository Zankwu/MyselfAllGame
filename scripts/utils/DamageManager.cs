using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
public partial class DamageManager : Node
{

    [Signal]
    public delegate void HealthBarChangeEventHandler(int type, int max_health, int current_health);

    [Signal]
    public delegate void ComboChangeEventHandler();
    public static DamageManager instance;

    public override void _Ready()
    {
        instance = this;
    }
}
