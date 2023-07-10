using System;
using DG.Tweening;
using Main.Scripts.Core;
using Main.Scripts.EventHandler;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Main.Scripts.Game.MatchableObject
{
    public class BoosterObject : MatchableObjectBase
    {
        public int ChargeAmount = 3;
        public int MaxLevelChainOutput = 2;
        public void WhoopOnCreate(Vector3Int tilePos, Action onComplete = null)
        {
            ObjectState = State.Whooping;
            var centerTilePos = _board.Tilemap.GetCellCenterWorld(tilePos);
            
            transform.DOKill();
            _view.DOKill();
            var viewTransform = _view.transform;
            _initialScale = viewTransform.localScale;
            viewTransform.localScale = Vector3.zero; 
            
            var whoopSequence = DOTween.Sequence();
            whoopSequence.Append(transform.DOMove(centerTilePos, 2).SetEase(Ease.InOutBack))
                .Insert(0,_view.DOScale(_initialScale, 2).SetEase(Ease.InOutBack))
                .AppendCallback(() => ObjectState = State.Idle)
                .AppendCallback(() => onComplete?.Invoke());
        }

        public override void OnSelect()
        {
        }

        public override void OnDrag(Vector2 movePos)
        {
            base.OnDrag(movePos);
        }

        public override void OnDeselect()
        {
            if (ChargeAmount <= 0)
            {
                return;
            }
            
            //shake and create items
            _view.DOKill();
            DOTween.Sequence().Append(_view.transform.DOScale(_initialScale * 1.2f, 0.25f))
                .Append(_view.transform.DOScale(_initialScale, 0.25f))
                .AppendCallback(CreateMatchableObjects);
        }

        private void CreateMatchableObjects()
        {
            var randomInt = Random.Range(0, MaxLevelChainOutput);
            var spawnItem = _config.MatchableObjects[randomInt];
            
            if (!_board.GetFirstEmptyTile(out var availableTile))
            {
                // no available tile 
                return;
            }
            
            _board.CreateObjectFromBooster(spawnItem, transform.position, availableTile);
            
            ChargeAmount--;
            if (ChargeAmount <= 0)
            {
                Destroy(gameObject);
            }
        }

        protected override void OnDestroy()
        {
            _view.DOKill();
            transform.DOKill();
            _transform.DOKill();
            base.OnDestroy();
            
        }
    }
}
