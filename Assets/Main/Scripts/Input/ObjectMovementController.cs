using Main.Scripts.Board;
using Main.Scripts.Core;
using Main.Scripts.Game.EventHandler;
using Main.Scripts.Game.MatchableObject;
using UnityEngine;

namespace Main.Scripts.Input
{
    public class ObjectMovementController : MonoSingleton<ObjectMovementController>
    {
        [SerializeField] private string _layerName;
        private IRaycastHandler<MatchableObjectBase> _inputHandler;

        private Camera _cam;

        private BoardHandler _boardHandler;

        public override void Init()
        {
            base.Init();
            _inputHandler = new RaycastObjectSelector<MatchableObjectBase>(Camera.main,_layerName);
            _inputHandler.OnObjectDown += InputHandlerOnOnObjectDown;
            _inputHandler.OnObjectUp += InputHandlerOnOnObjectUp;
            _boardHandler = BoardHandler.Instance;
            _cam = Camera.main;
        }

        protected override void Dispose()
        {
            base.Dispose();
            
            _inputHandler.OnObjectDown -= InputHandlerOnOnObjectDown;
            _inputHandler.OnObjectUp -= InputHandlerOnOnObjectUp;
            _inputHandler = null;
            _boardHandler = null;
            _cam = null;
        }

        private void InputHandlerOnOnObjectDown(MatchableObjectBase obj)
        {
            obj.OnSelect();
            GameManager.Instance.EventHandler.Notify(GameEvent.OnTapItem, obj);
        }

        private void InputHandlerOnOnObjectUp()
        {
            if (_inputHandler.SelectedObject != null && _inputHandler.SelectedObject.ObjectState == MatchableObjectBase.State.Whooping)
            {
                _inputHandler.SelectedObject = null;
                return;
            }

            if (_inputHandler.SelectedObject == null)
            {
                return;
            }

            _inputHandler.SelectedObject.OnDeselect();
            _inputHandler.SelectedObject = null;
            
            GameManager.Instance.EventHandler.Notify(GameEvent.OnReleaseItem);
        }
        

        private void DragObject()
        {
            if (_inputHandler.SelectedObject == null)
            {
                return;
            }

            if (_inputHandler.SelectedObject.ObjectState != MatchableObjectBase.State.Dragging)
            {
                return;
            }
            
            var mousePos = _cam.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            Vector2 movePos;
            var cellPos = _boardHandler.Grid.WorldToCell(mousePos);

            // Check if the tile exists in the Tilemap
            if (_boardHandler.Tilemap.HasTile(cellPos))
            {
                movePos = _boardHandler.Tilemap.GetCellCenterWorld(cellPos);
                _inputHandler.SelectedObject.OnDrag(movePos, true, cellPos);
            }
            else // if there's no available tile, then follow mouse/finger
            { 
                movePos = mousePos;
                _inputHandler.SelectedObject.OnDrag(movePos);
            }
        }
        
        private void Update()
        {
            _inputHandler.Update();
            DragObject();
        }
    }
}