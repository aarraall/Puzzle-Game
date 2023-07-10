using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Main.Scripts.Quest_System
{
    [CreateAssetMenu]
    public class QuestConfig : ScriptableObject
    {
        public List<CollectionQuestData> CollectionQuests = new List<CollectionQuestData>();
        public List<UseQuestData> UseQuests = new List<UseQuestData>();
        public List<BuildQuestData> BuildQuests = new List<BuildQuestData>();

        [SerializeReference] private List<QuestData> _quests = new List<QuestData>();
        public List<QuestData> Quests => _quests;

        [Button("PopulateQuestsList")]
        public void PopulateQuestsList()
        {
            _quests.AddRange(CollectionQuests);
            _quests.AddRange(UseQuests);
            _quests.AddRange(BuildQuests);
        }

        private void Reset()
        {
            _quests.AddRange(CollectionQuests);
            _quests.AddRange(UseQuests);
            _quests.AddRange(BuildQuests);
        }
    }
}
