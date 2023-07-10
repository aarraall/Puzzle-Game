using System;
using UnityEngine;

namespace Main.Scripts.Input
{
    public interface IInputHandler
    {
        void Update();
    }

    public interface IRaycastHandler<TSelectedObject> : IInputHandler where TSelectedObject : MonoBehaviour
    {
        Camera Camera { get; }
        TSelectedObject SelectedObject { get; set; }
        event Action<TSelectedObject> OnObjectDown;
        event Action OnObjectUp;
        bool IsDragging { get; }
        void Raycast();
    }
    public class RaycastObjectSelector<TSelectedObject> : IRaycastHandler<TSelectedObject> where TSelectedObject : MonoBehaviour
    {
        public Camera Camera { get; }
        private string LayerName { get; }
        public TSelectedObject SelectedObject { get; set; }
        public event Action<TSelectedObject> OnObjectDown;
        public event Action OnObjectUp;
        
        private Vector3 _initialMousePos;
        private bool _isDragging;
        private bool _isObjectSelected;

        public bool IsDragging => _isDragging;

        public RaycastObjectSelector()
        {
            
        }
        public RaycastObjectSelector(Camera cam, string layer)
        {
            Camera = cam;
            LayerName = layer;
        }

        public void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Raycast();
            }

            if (UnityEngine.Input.GetMouseButton(0)&&_isObjectSelected)
            {
                if (UnityEngine.Input.mousePosition != _initialMousePos)
                {
                    _isDragging = true;
                }
            }

            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                _initialMousePos = Vector3.zero;
                OnObjectUp?.Invoke();
                _isDragging = false;
                SelectedObject = null;
                _isObjectSelected = false;
            }
        }

        public void Raycast()
        {
            _initialMousePos = UnityEngine.Input.mousePosition;
            var worldPos = Camera.ScreenToWorldPoint(_initialMousePos);
            var layer = LayerMask.NameToLayer(LayerName);
            var hit = Physics2D.Raycast(worldPos, Vector2.zero, 100,
                1 << layer);

            if (hit.rigidbody == null)
            {
                return;
            }
            
            if (!hit.rigidbody.TryGetComponent<TSelectedObject>(out var selectedObject))
            {
                return;
            }

            SelectedObject = selectedObject;
            _isObjectSelected = true;
            
            OnObjectDown?.Invoke(SelectedObject);
        }
    }
}