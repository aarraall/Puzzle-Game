using System;
using DG.Tweening;
using UnityEngine;
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
            whoopSequence.Append(transform.DOMove(centerTilePos, 1).SetEase(Ease.InOutBack))
                .Insert(0,_view.DOScale(_initialScale, 1).SetEase(Ease.InOutBack))
                .AppendCallback(() => ObjectState = State.Idle)
                .AppendCallback(() => onComplete?.Invoke());
        }

        public override void OnSelect()
        {
            _renderer.sortingOrder = 10;
        }

        public override void OnDrag(Vector2 movePos)
        {
            base.OnDrag(movePos);
        }

        public override void OnDeselect()
        {
            _renderer.sortingOrder = _initialSortOrder;
            if (ChargeAmount <= 0)
            {
                return;
            }
            
            //shake and create items
            _view.DOKill();
            DOTween.Sequence().Append(_view.transform.DOScale(_initialScale * 1.2f, .125f))
                .Append(_view.transform.DOScale(_initialScale, 0.125f))
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

        public override bool CanMerge(MatchableObjectBase other)
        {
            //boosters shouldn't be merged
            return false;
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
