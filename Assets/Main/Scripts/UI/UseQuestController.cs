using DG.Tweening;
using Main.Scripts.Core;
using Main.Scripts.EventHandler;
using Main.Scripts.Game.MatchableObject;
using Main.Scripts.Quest_System;
using TMPro;
using UnityEngine.UI;

namespace Main.Scripts.UI
{
    public class UseQuestController : QuestController<UseQuestData>
    {
        public Image Image, SuccessImg;

        public TMP_Text CollectAmountText;

        public int TargetCollectAmount;
        

        public override void Initialize(UseQuestData questData)
        {
            base.Initialize(questData);
            TargetCollectAmount = questData.CollectAmount;
        }

        protected override void SetupVisuals()
        {
            Image.sprite = QuestData.TargetObjectSample.SpriteRenderer.sprite;
            CollectAmountText.text = QuestData.CollectAmount.ToString();        
        }

        public override void StartQuest()
        {
            base.StartQuest();
            GameManager.EventHandler.Subscribe(GameEvent.OnUseItem, OnMerge);
        }


        private void OnDestroy()
        {
            GameManager.EventHandler.Unsubscribe(GameEvent.OnUseItem, OnMerge);
        }

        private void OnMerge(object obj)
        {
            var matchableObject = obj as BoosterObject;
            if (matchableObject == null)
            {
                return;
            }

            if (!matchableObject.Equals(QuestData.TargetObjectSample))
            {
                return;
            }

            TargetCollectAmount--;
            CollectAmountText.text = TargetCollectAmount.ToString();

            if (TargetCollectAmount > 0)
            {
                return;
            }
            
            SuccessImg.gameObject.SetActive(true);
            GameManager.EventHandler.Notify(QuestData.OnDoneEvent);
            OnQuestEnd();
            
        }
    
    }
}
