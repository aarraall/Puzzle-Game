using System;
using DG.Tweening;
using Main.Scripts.Core;
using NaughtyAttributes;
using UnityEngine;

namespace Main.Scripts.Game.HomePhaseController
{
    public class HomePhaseView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _interactableSpriteRenderer;
        [SerializeField] private SpriteRenderer _decorativeSpriteRenderer;
        [SerializeField] private SpriteRenderer _phaseDoneSpriteRenderer;

        private Sequence _onPhaseDoneSequence;
        private Sequence _onReadySequence;
        private Tween _selectionTween;

        private Transform _interactableTransform;
        private Transform _decorativeTransform;
        private Transform _phaseDoneTransform;
        
        private Vector3 _initialInteractablePos, _initialDecorativePos, _initialPhaseDonePos;
        private Vector3 _initialInteractableScale, _initialDecorativeScale, _initialPhaseDoneScale;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            Cache();
            AlignInitialState();
        }
        
        private void Cache()
        {
            _interactableTransform = _interactableSpriteRenderer.transform;
            _initialInteractablePos = _interactableTransform.position;
            _initialInteractableScale = _interactableTransform.localScale;

            _decorativeTransform = _decorativeSpriteRenderer.transform;
            _initialDecorativePos = _decorativeTransform.position;
            _initialDecorativeScale = _decorativeTransform.localScale;

            _phaseDoneTransform = _phaseDoneSpriteRenderer.transform;
            _initialPhaseDonePos = _phaseDoneTransform.position;
            _initialPhaseDoneScale = _phaseDoneTransform.localScale;
        }

        private void AlignInitialState()
        {
            var state = GameManager.Instance.GameState;
            switch (state)
            {
                case GameManager.State.Home :
                    //show only interactable and decorative. but set interactable's color a bit darker
                    SetVisualState(false);
                    break;
                case GameManager.State.Finish :
                    // scale up animation loop
                    
                    SetVisualState(false);

                    _onReadySequence = DOTween.Sequence();
                    _onReadySequence
                        .Append(_interactableTransform.DOScale(_initialInteractableScale * 1.2f, 1f))
                        .Append(_decorativeTransform.DOScale(_initialDecorativeScale * 1.2f, 1f))
                        .SetLoops(-1, LoopType.Yoyo);
                    
                    break;
                default:
                    break;
            }
        }

        private void SetVisualState(bool isPhaseDoneEnabled)
        {
            _interactableSpriteRenderer.gameObject.SetActive(!isPhaseDoneEnabled);
            _decorativeSpriteRenderer.gameObject.SetActive(!isPhaseDoneEnabled);
            _phaseDoneSpriteRenderer.gameObject.SetActive(isPhaseDoneEnabled);
        }


        public void OnSelect()
        {
            var state = GameManager.Instance.GameState;
            
            _selectionTween?.Kill();
            _selectionTween = null;

            switch (state)
            {
                case GameManager.State.Home :
                    _selectionTween = _interactableTransform.DOScale(_initialInteractableScale * 1.2f, 1f)
                        .SetLoops(-1, LoopType.Yoyo);
                    break;
                case GameManager.State.Finish :
                    DOTween.Sequence()
                        .Append(_interactableTransform.DOScale(.2f, 1))
                        .Insert(0, _interactableTransform.DOMove(Vector3.right * 1.2f + Vector3.up * 3f, 1))
                        .Append(_interactableTransform.DOScale(0, .35f))
                        .Insert(1, _interactableTransform.DOMove(Vector3.right * 1.5f + Vector3.down * .7f, .35f));

                    break;
                default:
                    break;
            }
        }

        public void OnRelease()
        {
            var state = GameManager.Instance.GameState;

            if (_selectionTween != null)
            {
                _selectionTween.Kill();
                _selectionTween = null;
            }

            switch (state)
            {
                case GameManager.State.Home :
                    _selectionTween = _interactableTransform.DOScale(_initialInteractableScale * 1.2f, 1f)
                        .SetLoops(-1, LoopType.Yoyo);
                    break;
                case GameManager.State.Finish :
                    DOTween.Sequence()
                        .Append(_interactableTransform.DOScale(0, 3))
                        .Insert(0, _interactableTransform.DOMove(Vector3.right * 1.5f + Vector3.up * 3f, 3).SetEase(Ease.InCirc))
                        .AppendCallback(() => _interactableTransform.gameObject.SetActive(false)); 
                    
                    DOTween.Sequence()
                        .Append(_decorativeTransform.DOScale(0, 3))
                        .Insert(0, _decorativeTransform.DOMove(Vector3.left * 1.5f + Vector3.up * 3f, 3).SetEase(Ease.InCirc))
                        .AppendCallback(() => _decorativeTransform.gameObject.SetActive(false));

                    DOTween.Sequence()
                        .AppendCallback(() => _phaseDoneTransform.gameObject.SetActive(true))
                        .AppendCallback(() => _phaseDoneTransform.localScale = Vector3.zero)
                        .Append(_phaseDoneSpriteRenderer.DOFade(1, 2f))
                        .Insert(1, _phaseDoneTransform.DOScale(.2f, 2));
                    break;
                default:
                    break;
            }
        }
    }
}
