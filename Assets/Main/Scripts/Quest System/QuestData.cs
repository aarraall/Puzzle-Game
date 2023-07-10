using Main.Scripts.EventHandler;
using Main.Scripts.Game.MatchableObject;
using Main.Scripts.UI;
using UnityEngine;

namespace Main.Scripts.Quest_System
{
    [System.Serializable]
    public class QuestData
    {
        public enum Type
        {
            Collect,
            Use,
            Build
        }
        public enum State
        {
            Enabled,
            Disabled,
            Started,
            Finished
        }

        public string QuestID;
        public GameEvent OnDoneEvent;
        public State QuestState;
        public Type QuestType;
        public int CollectAmount;
    }

    [System.Serializable]
    public class CollectionQuestData : QuestData
    {
        public MatchableObjectBase TargetObjectSample;
        public CollectQuestRule QuestRule;
        public CollectibleQuestController controllerPrefab;
    }
    
    
    [System.Serializable]
    public class UseQuestData : QuestData
    {
        public BoosterObject TargetObjectSample;
        public UseQuestRule QuestRule;
        public UseQuestController controllerPrefab;
    }
    
    [System.Serializable]
    public class BuildQuestData : QuestData
    {
        public MatchableObjectBase[] TargetObjectSample;
        public BuildQuestRule QuestRule;
        public BuildQuestController controllerPrefab;
    }



    public interface IQuestRule<in TObjectType> where TObjectType : MatchableObjectBase
    {
        public bool CheckIfPhaseAchieved(TObjectType matchableObject, int successAmount, out int totalSuccess);
    }

    public class CollectQuestRule : IQuestRule<MatchableObjectBase>
    {
        private MatchableObjectBase _objectBase;
        
        public CollectQuestRule(MatchableObjectBase objectBase)
        {
            _objectBase = objectBase;
        }

        public bool CheckIfPhaseAchieved(MatchableObjectBase matchableObject, int successAmount, out int totalSuccess)
        {
            totalSuccess = 0;
            if (!matchableObject.Equals(_objectBase))
            {
                return false;
            }

            totalSuccess += successAmount + 1;

            return true;
        }
    }
    
    public class UseQuestRule : IQuestRule<BoosterObject>
    {
        private BoosterObject _objectBase;

        public UseQuestRule(BoosterObject objectBase)
        {
            _objectBase = objectBase;
        }

        public bool CheckIfPhaseAchieved(BoosterObject usedBoosterObject, int successAmount, out int totalSuccess)
        {
            totalSuccess = 0;

            if (!usedBoosterObject.Equals(_objectBase))
            {
                return false;
            }

            totalSuccess += successAmount + 1;

            return true;
        }
    }

    public class BuildQuestRule : IQuestRule<MatchableObjectBase>
    {
        private readonly MatchableObjectBase[] _pattern;

        public BuildQuestRule(MatchableObjectBase[] pattern)
        {
            _pattern = pattern;
        }
        
        public bool CheckIfPhaseAchieved(MatchableObjectBase matchableObject, int successAmount, out int totalSuccess)
        {
            totalSuccess = 0;
            if (successAmount >= _pattern.Length - 1)
            {
                //you've already succeeded
                //check if quest's termination logic works correctly
                Debug.LogError($"You already reached quest goal long time ago pal!");
                return false;
            }

            var nextItem = _pattern[successAmount + 1];
            if (!nextItem.Equals(matchableObject))
            {
                //it's not quest item
                return false;
            }

            totalSuccess += successAmount + 1;

            return true;
        }
    }
}
