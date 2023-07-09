using DG.Tweening;
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

        public void Initialize(HomePhaseObjectBase.State state)
        {
            Cache();
            AlignInitialState(state);
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

        private void AlignInitialState(HomePhaseObjectBase.State state)
        {
            switch (state)
            {
                case HomePhaseObjectBase.State.Disabled :
                    //show only interactable and decorative. but set interactable's color a bit darker
                    
                    SetVisualState(false);
                    
                    break;
                case HomePhaseObjectBase.State.Enabled :
                    //show only interactable and decorative.
                    
                    SetVisualState(false);
                    
                    break;
                case HomePhaseObjectBase.State.EnabledAndReadyForAction :
                    // scale up animation loop
                    
                    SetVisualState(false);

                    _onReadySequence = DOTween.Sequence();
                    _onReadySequence
                        .Append(_interactableTransform.DOScale(_initialInteractableScale * 1.2f, 1f))
                        .Append(_decorativeTransform.DOScale(_initialDecorativeScale * 1.2f, 1f))
                        .SetLoops(-1, LoopType.Yoyo);
                    
                    break;
                case HomePhaseObjectBase.State.Done :
                    // Lift the obstacle, animate it etc.
                    
                    SetVisualState(true);
                    
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


        public void OnSelect(HomePhaseObjectBase.State state)
        {
            switch (state)
            {
                case HomePhaseObjectBase.State.Disabled :
                    _selectionTween = _interactableTransform.DOScale(_initialInteractableScale * 1.2f, 1f)
                        .SetLoops(-1, LoopType.Yoyo);
                    break;
                case HomePhaseObjectBase.State.Enabled :
                    _selectionTween = _interactableTransform.DOScale(_initialInteractableScale * 1.2f, 1f);
                    break;
                case HomePhaseObjectBase.State.EnabledAndReadyForAction :
                    // Lift the obstacle, animate it etc.
                    break;
                case HomePhaseObjectBase.State.Done :
                    _selectionTween = _phaseDoneTransform.DOScale(_initialPhaseDoneScale * 1.2f, 1f)
                        .SetLoops(-1, LoopType.Yoyo);
                    break;
                default:
                    break;
            }
        }

        public void OnRelease(HomePhaseObjectBase.State state)
        {
            if (_selectionTween != null)
            {
                _selectionTween.Kill();
                _selectionTween = null;
            }
            
            switch (state)
            {
                case HomePhaseObjectBase.State.Disabled :
                    _selectionTween = _interactableTransform.DOScale(_initialInteractableScale, .2f);
                    break;
                case HomePhaseObjectBase.State.Enabled :
                    // carry user to level since goal's not accomplished yet
                    break;
                case HomePhaseObjectBase.State.EnabledAndReadyForAction :
                    // Lift the obstacle, animate it etc.
                    break;
                case HomePhaseObjectBase.State.Done :
                    _selectionTween = _phaseDoneTransform.DOScale(_initialPhaseDoneScale, .2f);
                    break;
                default:
                    break;
            }
        }
    }
}
