using System;
using Main.Scripts.Board;
using Main.Scripts.Game;
using Main.Scripts.Game.HomePhaseController;
using UnityEngine;

namespace Main.Scripts.Input
{
    public class HomeInputController : MonoSingleton<HomeInputController>
    {
        [SerializeField] private string _layerName;
        private IRaycastHandler<HomePhaseObjectBase> _inputHandler;
        private Camera _cam;
        private HomePhaseObjectBase _currentObject;
        public override void Init()
        {
            base.Init();
            _inputHandler = new RaycastObjectSelector<HomePhaseObjectBase>(Camera.main, _layerName);
            _inputHandler.OnObjectDown += InputHandlerOnOnObjectDown;
            _inputHandler.OnObjectUp += InputHandlerOnOnObjectUp;
            _cam = Camera.main;
        }

        protected override void Dispose()
        {
            base.Dispose();
            
            _inputHandler.OnObjectDown -= InputHandlerOnOnObjectDown;
            _inputHandler.OnObjectUp -= InputHandlerOnOnObjectUp;
            _inputHandler = null;
            _cam = null;
        }
        
        private void InputHandlerOnOnObjectDown(HomePhaseObjectBase obj)
        {
            _currentObject = obj;
            obj.OnSelect();
        }
        
        private void InputHandlerOnOnObjectUp()
        {
            _currentObject.OnRelease();
            _currentObject = null;
        }

        private void Update()
        {
            _inputHandler.Update();
        }
    }
}
