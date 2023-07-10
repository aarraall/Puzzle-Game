using System;
using Main.Scripts.Game.HomePhaseController;
using Main.Scripts.Util.Generics;
using UnityEngine;

namespace Main.Scripts.Input
{
    public class HomeInputController : MonoSingleton<HomeInputController>
    {
        [SerializeField] private string _layerName;
        private IRaycastHandler<HomePhaseObjectBase> _inputHandler;
        private Camera _cam;
        private HomePhaseObjectBase _currentObject;

        private void Start()
        {
            _cam = Camera.main;
            _inputHandler = new RaycastObjectSelector<HomePhaseObjectBase>(_cam, _layerName);
            _inputHandler.OnObjectDown += InputHandlerOnOnObjectDown;
            _inputHandler.OnObjectUp += InputHandlerOnOnObjectUp;        }

        private void OnDestroy()
        {
            _cam = null;

            if (_inputHandler == null)
            {
                return;
            }
            
            _inputHandler.OnObjectDown -= InputHandlerOnOnObjectDown;
            _inputHandler.OnObjectUp -= InputHandlerOnOnObjectUp;
            _inputHandler = null;        }

        
        private void InputHandlerOnOnObjectDown(HomePhaseObjectBase obj)
        {
            _currentObject = obj;
            obj.OnSelect();
        }
        
        private void InputHandlerOnOnObjectUp()
        {
            if (_currentObject == null)
            {
                return;
            }
            
            _currentObject.OnRelease();
            _currentObject = null;
        }

        private void Update()
        {
            _inputHandler?.Update();
        }
    }
}
