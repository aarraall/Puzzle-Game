using Main.Scripts.Util.Generic_Helpers;
using Main.Scripts.Util.Generics;

namespace Main.Scripts.Game.EventHandler
{
    public enum GameEvent
    {
        OnTapItem,
        OnDragItem,
        OnReleaseItem,
        OnBoardLoaded,
    }
    public class GameEventHandler : EventHandler<GameEvent>
    {
    
    }
}