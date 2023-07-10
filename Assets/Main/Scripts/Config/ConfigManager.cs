using Main.Scripts.Config;
using Main.Scripts.Home;
using Main.Scripts.Util.Generics;

public class ConfigManager : MonoSingleton<ConfigManager>
{
    public LevelConfig LevelConfig;
    public MatchableObjectConfig MatchableObjectConfig;
    public MatchableObjectConfig BoosterConfig;
}
