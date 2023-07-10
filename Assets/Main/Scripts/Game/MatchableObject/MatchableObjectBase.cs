using System;
using DG.Tweening;
using Main.Scripts.Board;
using Main.Scripts.Config;
using Main.Scripts.Core;
using NaughtyAttributes;
using UnityEngine;

namespace Main.Scripts.Game.MatchableObject
{
    public class MatchableObjectBase : MonoBehaviour
    {
        public enum Type
        {
            Letter,
            Booster
        }
        public enum State
        {
            Idle,
            Blocked,
            Whooping,
            Dragging,
            Merging,
        }

        [SerializeField] protected SpriteRenderer _renderer;

        [SerializeField] protected Transform _view;

        [SerializeField] protected GameObject _blockedVisual;

        [SerializeField] protected MatchableObjectConfig _config;

        public Type ObjectType;

        public State ObjectState = State.Idle;

        public int ChainPosition = 0;

        public Vector3Int CurrentTilePos;
        

        protected Vector3 _initialScale;

        protected Tween _selectionTween;
        protected int _initialSortOrder;
        protected Transform _transform;
        
        
        protected BoardHandler _board;
        public SpriteRenderer SpriteRenderer => _renderer;

        protected void Awake()
        {
            Cache();
        }
        
        protected void Cache()
        {
            _transform = transform;
            _initialScale = _view.localScale;
            _initialSortOrder = _renderer.sortingOrder;
        }

        public void Initialize(BoardHandler board)
        {
            _board = board;
            
            if (ObjectState == State.Blocked)
            {
                _blockedVisual.SetActive(true);
            }
        }

        public virtual void OnSelect()
        {
            if (_selectionTween != null)
            {
                _selectionTween.Kill();
                _selectionTween = null;
            }

            if (ObjectState == State.Whooping)
            {
                return;
            }

            _renderer.sortingOrder = 10;
            
            _selectionTween = _view.DOScale(_initialScale * 1.5f, 1f);
        }
        
        public virtual void OnDeselect()
        {
            if (_selectionTween != null)
            {
                _selectionTween.Kill();
                _selectionTween = null;
            }
            
            _renderer.sortingOrder = _initialSortOrder;
            
            _selectionTween = _view.DOScale(_initialScale, .2f);
        }
        

        public virtual void OnDrag(Vector2 movePos)
        {
            ObjectState = State.Dragging;
            
            transform.position = Vector2.MoveTowards(transform.position, movePos, Time.deltaTime * 10);
        }

        public virtual void OnDragEnd(bool isOnTile = false, Vector3Int cellPos = default)
        {
            if (isOnTile)
            {
                _board.OnMatchableObjectChangeTile(cellPos, this);
            }

            if (ObjectState is State.Merging or State.Whooping)
            {
                return;
            }

            _view.localScale = _initialScale;
            WhoopToTile(CurrentTilePos);
        }

        public virtual void WhoopToTile(Vector3Int tilePos, Action onComplete = null)
        {
            ObjectState = State.Whooping;
            var centerTilePos = _board.Tilemap.GetCellCenterWorld(tilePos);
            
            transform.DOKill();
            _view.DOKill();
            _view.transform.localScale = _initialScale;
            
            var whoopSequence = DOTween.Sequence();
            whoopSequence.Append(transform.DOMove(centerTilePos, .35f))
                .AppendCallback(() => ObjectState = State.Idle)
                .AppendCallback(() => onComplete?.Invoke());
        }
        
        public virtual void WhoopToTileOnMerge(Vector3Int tilePos, Action onComplete = null)
        {
            ObjectState = State.Whooping;
            var centerTilePos = _board.Tilemap.GetCellCenterWorld(tilePos);
            
            transform.DOKill();
            _view.DOKill();
            _view.transform.localScale = _initialScale;
            
            var whoopSequence = DOTween.Sequence();
            whoopSequence.Append(transform.DOMove(centerTilePos, .35f))
                .Insert(0,_view.DOScale(0, .35f))
                .AppendCallback(() => ObjectState = State.Idle)
                .AppendCallback(() => onComplete?.Invoke());
        }
        
        public virtual void MergeOut(Vector3Int tilePos)
        {
            var centerOfCell = _board.Tilemap.GetCellCenterWorld(tilePos);
            
            CurrentTilePos = tilePos;
            _transform.position = centerOfCell;
            _transform.rotation = Quaternion.identity;
            _view.localScale = Vector3.zero;
            _view.DOKill();
            _view.DOScale(_initialScale, .25f).SetEase(Ease.OutBack);

        }

        public virtual void WhoopToTileOnBoost(Vector3Int tilePos, Action onComplete = null)
        {
            ObjectState = State.Whooping;
            var centerTilePos = _board.Tilemap.GetCellCenterWorld(tilePos);
            
            transform.DOKill();
            _view.DOKill();
            _initialScale = _view.transform.localScale;
            _view.transform.localScale = Vector3.zero;
            
            var whoopSequence = DOTween.Sequence();
            whoopSequence.Append(transform.DOMove(centerTilePos, .25f).SetEase(Ease.InOutBack))
                .Insert(0,_view.DOScale(_initialScale, .25f).SetEase(Ease.InOutBack))
                .AppendCallback(() => ObjectState = State.Idle)
                .AppendCallback(() => onComplete?.Invoke());
        }

        protected virtual void OnDestroy()
        {
            _transform.DOKill();
            _view.DOKill();
        }

        public bool Equals(MatchableObjectBase other)
        {
            return ObjectType == other.ObjectType && ChainPosition == other.ChainPosition;
        }

        public virtual bool CanMerge(MatchableObjectBase other)
        {
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), (int)ObjectType, ChainPosition);
        }

        #region Test

        [Button("SetCurrentTilePos")]
        public void SetCurrentTilePos()
        {
            var board = BoardHandler.Instance;
            var cellPos = board.Grid.WorldToCell(transform.position);

            if (!board.Tilemap.HasTile(cellPos))
            {
                return;
            }

            CurrentTilePos = cellPos;
            
            var cellCenter = board.Grid.GetCellCenterWorld(cellPos);
            
            transform.position = cellCenter;
        }

        #endregion
    }
}