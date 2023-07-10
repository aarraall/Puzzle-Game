using System;
using System.Collections.Generic;
using DG.Tweening;
using Main.Scripts.Core;
using Main.Scripts.EventHandler;
using Main.Scripts.UI;
using Main.Scripts.Util.Generics;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Scripts.Quest_System
{
    public class QuestHandler : MonoSingleton<QuestHandler>
    {
        [SerializeField] private TMP_Text _questTypeText;
        
        public QuestConfig QuestConfig;

        public LayoutGroup LayoutGroup;

        private Queue<QuestController> _questQueue = new Queue<QuestController>();
        protected QuestController CurrentQuestController;
        public override void Init()
        {
            base.Init();
            CreateQuestViews();
            StartNextQuest();
            GameManager.EventHandler.Subscribe(GameEvent.OnQuestDone, OnQuestDone);
        }

        private void OnDestroy()
        {
            GameManager.EventHandler.Unsubscribe(GameEvent.OnQuestDone, OnQuestDone);
        }

        private void CreateQuestViews()
        {
            foreach (var questConfigQuest in QuestConfig.Quests)
            {
                switch (questConfigQuest.QuestType)
                {
                    case QuestData.Type.Merge:
                        InitializeCollectQuest(questConfigQuest);
                        break;
                    case QuestData.Type.Use:
                        InitializeUseQuest(questConfigQuest);
                        break;
                    case QuestData.Type.Create:
                        InitializeBuildQuest(questConfigQuest);
                        break;
                }
            }

            void InitializeCollectQuest(QuestData collectQuest) //Generic method can be used here
            {
                if (collectQuest is not CollectionQuestData quest)
                {
                    return;
                }
                quest.QuestRule = new CollectQuestRule(quest.TargetObjectSample);
                var questView = Instantiate(quest.controllerPrefab, LayoutGroup.transform).GetComponent<CollectibleQuestController>();
                _questQueue.Enqueue(questView);
                questView.Initialize(quest);
            }
            
            void InitializeUseQuest(QuestData collectQuest)
            {
                if (collectQuest is not UseQuestData quest)
                {
                    return;
                }
                quest.QuestRule = new UseQuestRule(quest.TargetObjectSample);
                var questView = Instantiate(quest.controllerPrefab, LayoutGroup.transform).GetComponent<UseQuestController>();
                _questQueue.Enqueue(questView);
                questView.Initialize(quest);
            }
            void InitializeBuildQuest(QuestData collectQuest)
            {
                if (collectQuest is not BuildQuestData quest)
                {
                    return;
                }

                quest.QuestRule = new BuildQuestRule(quest.TargetObjectSample);
                var questView = Instantiate(quest.controllerPrefab, LayoutGroup.transform).GetComponent<BuildQuestController>();
                _questQueue.Enqueue(questView);
                questView.Initialize(quest);
            }
        }

        public void StartNextQuest()
        {
            var nextQuest = GetNextQuest();

            if (nextQuest == null)
            {
                return;
            }

            if (CurrentQuestController != null)
            {
                Destroy(CurrentQuestController.gameObject);
            }

            CurrentQuestController = nextQuest;
            _questTypeText.text = CurrentQuestController.QuestType.ToString();

            //send event to the game and start event
            nextQuest.StartQuest();
        }

        public void SetQuestDone(QuestData questData)
        {
            questData.QuestState = QuestData.State.Finished;
            //send event to the game and start quest

        }

        public QuestController GetNextQuest()
        {
            var nextQuest = _questQueue.Dequeue();

            if (nextQuest == null)
            {
                // Level's done
            }

            return nextQuest;
        }
        
        private void OnQuestDone(object obj)
        {
            DOTween.Sequence()
                .Append(CurrentQuestController.CanvasGroup.DOFade(0, 1))
                .AppendCallback(() => Destroy(CurrentQuestController.gameObject))
                .AppendCallback(StartNextQuest);
        }

        [Button("SetDoneCurrentQuest")]
        public void SetDoneCurrentQuest()
        {
            CurrentQuestController.OnQuestEnd();

            switch (CurrentQuestController.QuestType)
            {
                case QuestData.Type.Merge:
                    var collectController = CurrentQuestController as CollectibleQuestController;
                    GameManager.EventHandler.Notify(collectController.QuestData.OnDoneEvent);
                    break;
                case QuestData.Type.Use :
                    var useController = CurrentQuestController as UseQuestController;
                    GameManager.EventHandler.Notify(useController.QuestData.OnDoneEvent);
                    break; 
                case QuestData.Type.Create :
                    var buildController = CurrentQuestController as BuildQuestController;
                    GameManager.EventHandler.Notify(buildController.QuestData.OnDoneEvent);
                    break;
            }
        }
    
    }
}
