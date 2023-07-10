using Main.Scripts.EventHandler;
using Main.Scripts.Util.Generics;
using NaughtyAttributes;
using UnityEngine.Device;
using UnityEngine.SceneManagement;

namespace Main.Scripts.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public enum State
        {
            Loading,
            Home,
            Level,
            Finish,
        }

        public GameEventHandler EventHandler;
        public State GameState { get; private set; }
        
        
        public override void Init()
        {
            base.Init();
            EventHandler = new GameEventHandler();
            SetState(State.Loading);
            
            EventHandler.Subscribe(GameEvent.OnQuest7Reached, OnLastQuestReached);
        }

        protected override void Dispose()
        {
            EventHandler.Unsubscribe(GameEvent.OnQuest7Reached, OnLastQuestReached);
            base.Dispose();
        }

        public void SetState(State state)
        {
            GameState = state;
            switch (state)
            {
                case State.Loading:
                    OnLoading();
                    break;
                case State.Home:
                    OnHome();
                    break;
                case State.Level:
                    OnLevel();
                    break;
                case State.Finish:
                    OnFinish();
                    break;
            }
        }

        [Button("Finisj")]
        public void SetFinish()
        {
            SetState(State.Finish);
        }

    

        private void OnLoading()
        {
            SceneManager.LoadScene("Scene_Home");
            //Do stuff
            SetState(State.Home);
        }
        private void OnHome()
        {
            SceneManager.LoadScene("Scene_Home");
            //trigger FTUE and carry user to level without letting stay there
        }
        private void OnLevel()
        {
            SceneManager.LoadScene("Scene_Level");
            //let user play and listen goals, on goal is reached, carry user to home

        }
        
        private void OnFinish()
        {
            SceneManager.LoadScene("Scene_Home");
        }
        
        private void OnLastQuestReached(object obj)
        {
            //Show Popup to carry user to home level
        }


        public void QuitGameInvoke()
        {
            Invoke(nameof(Quit), 5);
        }

        private void Quit()
        {
            Application.Quit();
        }

        private void OnDestroy()
        {
            EventHandler = null;
        }

        private void Reset()
        {
        }
    }
}
