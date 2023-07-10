using DG.Tweening;
using Main.Scripts.Core;
using Main.Scripts.EventHandler;
using UnityEngine;

namespace Main.Scripts.UI
{
    public class FinishPopupController : MonoBehaviour
    {
        public GameObject PopupGameObject;
        private void Awake()
        {
            GameManager.Instance.EventHandler.Subscribe(GameEvent.OnQuest7Reached, OnLevelFinished);
        }

        private void OnLevelFinished(object obj)
        {
            PopupGameObject.SetActive(true);
            DOTween.Sequence().Append(PopupGameObject.transform.DOMoveX(0, 1f).SetEase(Ease.InOutBack))
                .Append(PopupGameObject.transform.DOScale(Vector3.one * .8f, .5f))
                .Append(PopupGameObject.transform.DOScale(Vector3.one, .5f));
        }


        public void OnClickButton()
        {
            GameManager.Instance.SetState(GameManager.State.Finish);
        }
    }
}
