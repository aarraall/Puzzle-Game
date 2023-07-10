using System;
using DG.Tweening;
using Main.Scripts.Core;
using Main.Scripts.Home;
using UnityEngine;

namespace Main.Scripts.Game.HomePhaseController
{

    public class HomePhaseObjectBase : MonoBehaviour
    {
        public HomePhaseView View;

        public void OnSelect()
        {
            View.OnSelect();
        }

        public void OnRelease()
        {

            switch (GameManager.Instance.GameState)
            {
                case GameManager.State.Home :
                    View.OnRelease(() =>GameManager.Instance.SetState(GameManager.State.Level));
                    break; 
                case GameManager.State.Finish :
                    //remove branches and tree 
                    View.OnRelease();
                    break;
                default:
                    break;
            }
            
            
        }
        
        
    }
}
