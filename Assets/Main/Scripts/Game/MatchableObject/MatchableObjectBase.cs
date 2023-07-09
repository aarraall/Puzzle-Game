using DG.Tweening;
using Main.Scripts.Board;
using UnityEngine;

namespace Main.Scripts.Game.MatchableObject
{
    public class MatchableObjectBase : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Dragging,
            Whooping,
        }
        
        public enum Type
        {
            Classic,
            Letter_I,
            Letter_L,
            Letter_U,
        }

        public int ChainIndex = 0;

        public Transform view;
        public Rigidbody2D rb;
        
        public Type MatchableObjectType { get; set; }
        public State ObjectState { get; set; }
        public Vector3Int CurrentCell { get; private set; }

        private Tween _currentWhoop;
        private Tween _selectTween;

        // Cache
        private Vector2 _originalScale;
        private Camera _cam;
        private BoardHandler _boardHandler;
        
        //constants        
        private const float k_movement_speed = 10f;
        private const float k_whoop_back_to_tile_duration = .2f;
        private const float k_scale_tween_modifier = 1.2f;
        private const float k_scale_tween_duration = 1f;
        private const float k_scale_back_tween_duration = .25f;

        public virtual void Initialize(BoardHandler boardHandler)
        {
            _boardHandler = boardHandler;
            CacheVariables();
            ObjectState = State.Idle;
        }

        protected virtual void CacheVariables()
        {
            _originalScale = view.localScale;
            _cam = Camera.main;
        }

        #region Behaviors

        public virtual void OnSelect()
        {
            ObjectState = State.Dragging;
            _selectTween = view.DOScale(view.localScale * Vector2.one * k_scale_tween_modifier, k_scale_tween_duration).SetLoops(-1, LoopType.Yoyo);
        }

        public virtual void OnDrag(Vector2 movePos, bool tileExists = false, Vector3Int cell = default)
        {
            if (tileExists)
                CurrentCell = cell;
            Move(movePos);
        }

        public virtual void OnDeselect()
        {
            _selectTween.Kill();
            view.DOScale(_originalScale, k_scale_back_tween_duration);
            var centerPosOfCell = _boardHandler.Tilemap.GetCellCenterWorld(CurrentCell);
            WhoopToTile(centerPosOfCell);
            _selectTween = null;
            ObjectState = State.Idle;
        }

        protected virtual void WhoopToTile(Vector3 targetTilePos)
        {
            ObjectState = State.Whooping;
            _currentWhoop?.Kill();
            _currentWhoop = transform.DOMove(targetTilePos, k_whoop_back_to_tile_duration)
                .OnComplete(() => ObjectState = State.Idle);
        }

        protected virtual void Move(Vector2 pos)
        {
            transform.position = Vector2.Lerp(transform.position, pos,k_movement_speed * Time.deltaTime);
        }
        
        public virtual void StartMatch()
        {
            _boardHandler.StartMatch(this);
        }
        
        public virtual void SetMatch(MatchableObjectBase matchOrigin)
        {
            if (matchOrigin == this)
            {
                return;
            }

            //Do animation and destroy object. New object creation will be responsibility of Board
        }

        #endregion

        
    }
}