using System;
using Main.Scripts.Game.MatchableObject;
using Main.Scripts.Util;
using UnityEngine;

namespace Main.Scripts.Game.Input
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
        event Action<TSelectedObject> OnObjectDrag;
        event Action OnObjectUp;
        void Raycast();
    }
    public class RaycastObjectSelector : IRaycastHandler<MatchableObjectBase>
    {
        public Camera Camera { get; }
        public MatchableObjectBase SelectedObject { get; set; }
        public event Action<MatchableObjectBase> OnObjectDown;
        public event Action<MatchableObjectBase> OnObjectDrag;
        public event Action OnObjectUp;
        

        public RaycastObjectSelector()
        {
            
        }
        public RaycastObjectSelector(Camera cam)
        {
            Camera = cam;
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
            var layer = LayerMask.NameToLayer(Constants.k_layer_matchable_object);
            var hit = Physics2D.Raycast(mousePos, Vector2.zero, 1000,
                1 << layer);

            if (hit.rigidbody == null)
            {
                return;
            }
            
            if (!hit.rigidbody.TryGetComponent<MatchableObjectBase>(out var selectedObject))
            {
                return;
            }

            SelectedObject = selectedObject;
            
            OnObjectDown?.Invoke(SelectedObject);
        }
    }
}