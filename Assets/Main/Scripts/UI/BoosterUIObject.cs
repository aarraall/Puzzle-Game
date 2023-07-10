using DG.Tweening;
using Main.Scripts.Board;
using Main.Scripts.Core;
using Main.Scripts.EventHandler;
using Main.Scripts.Game.MatchableObject;
using UnityEngine;

namespace Main.Scripts.UI
{
    public class BoosterUIObject : MonoBehaviour
    {
        [SerializeField] private BoosterObject _boosterSample;
        public int ChargeAmount;
        private BoardHandler _board;
        private void Start()
        {
            _board = BoardHandler.Instance;
        }

        public void OnButtonClick()
        {
            DOTween.Sequence().Append(transform.DOScale(Vector2.one * 1.2f, .25f))
                .Append(transform.DOScale(Vector2.one,.25f));
            _board.CreateNewBooster(_boosterSample, UnityEngine.Input.mousePosition);
            ChargeAmount--;
            
            GameManager.EventHandler.Notify(GameEvent.OnUseItem, _boosterSample);

            if (ChargeAmount <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
