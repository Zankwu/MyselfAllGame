using Godot;
using System.Collections.Generic;

public partial class MusicManager : Node
{
    public static MusicManager instance;

    [Export]
    public AudioStreamPlayer2D audioStreamPlayer2D;
    public enum MUSIC_TYPE
    {
        INTRO, MENU, STAGE1, STAGE2
    }
    public Dictionary<MUSIC_TYPE, AudioStream> MUSIC_MENU = new()
    {
        {MUSIC_TYPE.INTRO,GD.Load<AudioStream>("res://accetss/music/intro.mp3")},
        {MUSIC_TYPE.MENU,GD.Load<AudioStream>("res://accetss/music/menu.mp3")},
        {MUSIC_TYPE.STAGE1,GD.Load<AudioStream>("res://accetss/music/stage-01.mp3")},
        {MUSIC_TYPE.STAGE2,GD.Load<AudioStream>("res://accetss/music/stage-02.mp3")},
    };

    public override void _Ready()
    {
        instance = this;
    }

    public void PlayMusic(MUSIC_TYPE type)
    {
        // MusicManager 是 Autoload, 其子节点 AudioStreamPlayer2D 必然早于任何场景的 _Ready 就绪,
        // 因此这里直接播放即可, 无需延迟/缓存逻辑。
        if (audioStreamPlayer2D == null)
        {
            GD.PushError("MusicManager: audioStreamPlayer2D 未绑定");
            return;
        }

        audioStreamPlayer2D.Stream = MUSIC_MENU[type];
        audioStreamPlayer2D.Play();
    }
}

