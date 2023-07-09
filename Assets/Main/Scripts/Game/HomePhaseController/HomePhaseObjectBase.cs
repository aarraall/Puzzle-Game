using Main.Scripts.Home;
using UnityEngine;

namespace Main.Scripts.Game.HomePhaseController
{
    [System.Serializable]
    public struct HomePhaseData
    {
        public int PhaseIndex;
        public HomePhaseObjectBase.State PhaseState;
    }
    
    public class HomePhaseObjectBase : MonoBehaviour
    {
        public enum State
        {
            Enabled,
            EnabledAndReadyForAction,
            Disabled,
            Done
        }
        //this one is persistent config data
        public HomeConfig HomePhaseConfig;
        
        //this one is persistent config data
        public HomePhaseData HomePhaseSaveData { get; private set; }
        
        public State PhaseObjectState { get; private set; }

        private void Awake()
        {
            CacheFundamentals();
            SetState(State.Disabled);
        }

        public void Initialize(HomePhaseData homePhaseSaveData)
        {
            HomePhaseSaveData = homePhaseSaveData;
            
            SetState(homePhaseSaveData.PhaseState);
        }

        private void CacheFundamentals()
        {
            
        }

        public void SetState(State state)
        {
            PhaseObjectState = state;

            switch (PhaseObjectState)
            {
                case State.Disabled :
                    break;
                case State.Enabled :
                    break;
                case State.EnabledAndReadyForAction :
                    break;
            }
        }

        public void OnSelect()
        {
           
        }

        public void OnRelease()
        {
            switch (PhaseObjectState)
            {
                case State.Disabled :

                    break;
                case State.Enabled :
                    // carry user to level since goal's not accomplished yet
                    break;
                case State.EnabledAndReadyForAction :
                    // Lift the obstacle, bring win sprite
                    break;
                case State.Done :
                    // Lift the obstacle, bring win sprite
                    break;
                default:
                    break;
            }
            
            
        }
        
        
    }
}
