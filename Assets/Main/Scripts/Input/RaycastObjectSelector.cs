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
        void Raycast();
    }
    public class RaycastObjectSelector<TSelectedObject> : IRaycastHandler<TSelectedObject> where TSelectedObject : MonoBehaviour
    {
        public Camera Camera { get; }
        private string LayerName { get; }
        public TSelectedObject SelectedObject { get; set; }
        public event Action<TSelectedObject> OnObjectDown;
        public event Action OnObjectUp;
        

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
            
            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                OnObjectUp?.Invoke();
            }
        }

        public void Raycast()
        {
            var mousePos = Camera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            var layer = LayerMask.NameToLayer(LayerName);
            var hit = Physics2D.Raycast(mousePos, Vector2.zero, 1000,
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
            
            OnObjectDown?.Invoke(SelectedObject);
        }
    }
}