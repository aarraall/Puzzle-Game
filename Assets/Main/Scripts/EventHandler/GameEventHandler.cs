using Main.Scripts.Util.Generics;

namespace Main.Scripts.EventHandler
{
    public enum GameEvent
    {
        OnTapItem,
        OnDragItem,
        OnReleaseItem,
        OnPieceChangedTile,
        OnMergeItem,
        OnCreateItem,
        OnUseItem,
        OnQuestDone,
        
        
        
        
        
        
        
        
        
        
        //Quest Events (they can be separated via another event handler
        OnQuest1Reached,
        OnQuest2Reached,
        OnQuest3Reached,
        OnQuest4Reached,
        OnQuest5Reached,
        OnQuest6Reached,
        OnQuest7Reached,
        OnQuest8Reached,
        OnQuest9Reached,
        OnQuest10Reached,
    }
    public class GameEventHandler : EventHandler<GameEvent>
    {
    
    }
}