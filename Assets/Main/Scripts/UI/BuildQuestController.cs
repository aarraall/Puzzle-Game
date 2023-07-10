using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Main.Scripts.Core;
using Main.Scripts.EventHandler;
using Main.Scripts.Game.MatchableObject;
using Main.Scripts.Quest_System;
using TMPro;
using UnityEngine.UI;

namespace Main.Scripts.UI
{
    public class BuildQuestController : QuestController<BuildQuestData>
    {
        class BuildQuestDataContainer
        {
            public Image Image;
            public Image SuccessImage;
            public int TargetAmount;
            public TMP_Text TargetAmountText;
            public MatchableObjectBase TargetObjectSample;
            public bool IsDone;

            public BuildQuestDataContainer(Image image, Image successImage, int targetAmount, MatchableObjectBase targetObjectSample, TMP_Text targetAmountText)
            {
                Image = image;
                SuccessImage = successImage;
                TargetAmount = targetAmount;
                TargetObjectSample = targetObjectSample;
                TargetAmountText = targetAmountText;
            }
        }
        
        public Image[] Images;
        public Image[] SuccessImages;
        public TMP_Text[] Texts;

        private readonly List<BuildQuestDataContainer> _dataContainers = new();
        
        protected override void SetupVisuals()
        {
            for (int i = 0; i < Images.Length; i++)
            {
                var image = Images[i];
                var successImage = SuccessImages[i];
                var sprite = QuestData.TargetObjectSample[i].SpriteRenderer.sprite;
                image.sprite = sprite;

                Texts[i].text = QuestData.CollectAmount.ToString();
                
                _dataContainers.Add(new BuildQuestDataContainer(image, successImage, QuestData.CollectAmount, QuestData.TargetObjectSample[i], Texts[i]));
            }
        }
        
        public override void StartQuest()
        {
            base.StartQuest();
            GameManager.EventHandler.Subscribe(GameEvent.OnCreateItem, OnMerge);
        }


        private void OnDestroy()
        {
            GameManager.EventHandler.Unsubscribe(GameEvent.OnCreateItem, OnMerge);
        }

        private void OnMerge(object obj)
        {
            var matchableObject = obj as MatchableObjectBase;
            if (matchableObject == null)
            {
                return;
            }

            foreach (var container in _dataContainers)
            {
                if (!matchableObject.Equals(container.TargetObjectSample))
                {
                    continue;
                }

                container.TargetAmount--;
                container.TargetAmountText.text = container.TargetAmount.ToString();

                if (container.TargetAmount > 0)
                {
                    continue;
                }

                container.IsDone = true;
                container.SuccessImage.gameObject.SetActive(true);
            }

            if (!_dataContainers.All(container => container.IsDone))
            {
                return;
            }
            
            GameManager.EventHandler.Notify(QuestData.OnDoneEvent);
            OnQuestEnd();
        }

        private void Reset()
        {
            Images = GetComponentsInChildren<Image>();
            Texts = GetComponentsInChildren<TMP_Text>();
        }
    }
}
