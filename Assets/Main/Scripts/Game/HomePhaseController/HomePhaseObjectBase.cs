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
            transform.DOScale(Vector3.one * 1.2f, .5f)
                .OnComplete(() => transform.DOScale(Vector3.one, .5f));
        }

        public void OnRelease()
        {
            
            switch (GameManager.Instance.GameState)
            {
                case GameManager.State.Home :
                    GameManager.Instance.SetState(GameManager.State.Level);
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
