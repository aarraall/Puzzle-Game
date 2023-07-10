using DG.Tweening;
using Main.Scripts.Core;
using Main.Scripts.EventHandler;
using Main.Scripts.Game.MatchableObject;
using Main.Scripts.Quest_System;
using TMPro;
using UnityEngine.UI;

namespace Main.Scripts.UI
{
    public class CollectibleQuestController : QuestController<CollectionQuestData>
    {
        public Image Image, SuccessImg;

        public TMP_Text CollectAmountText;

        public int TargetCollectAmount;

        public override void Initialize(CollectionQuestData questData)
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
            GameManager.Instance.EventHandler.Subscribe(GameEvent.OnMergeItem, OnMerge);
        }


        private void OnDestroy()
        {
            if (GameManager.Instance == null || GameManager.Instance.EventHandler == null)
            {
                return;
            }

            GameManager.Instance.EventHandler.Unsubscribe(GameEvent.OnMergeItem, OnMerge);
        }

        private void OnMerge(object obj)
        {
            var matchableObject = obj as MatchableObjectBase;
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
            GameManager.Instance.EventHandler.Notify(QuestData.OnDoneEvent);
            
            OnQuestEnd();
        }
    }
}
