using System.Collections.Generic;
using System.Linq;
using Main.Scripts.Core;
using Main.Scripts.EventHandler;
using Main.Scripts.Game.MatchableObject;
using Main.Scripts.Util.Extensions;
using Main.Scripts.Util.Generics;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Main.Scripts.Board
{
    public class BoardHandler : MonoSingleton<BoardHandler>
    {
        public Tilemap Tilemap;
        public Grid Grid;
        
        [SerializeField] Transform _itemsParent;

        private Camera _cam;

        public List<MatchableObjectBase> MatchableObjects = new List<MatchableObjectBase>();
        public Dictionary<Vector3Int, MatchableObjectBase> MatchableObjectPositionMap = new Dictionary<Vector3Int, MatchableObjectBase>();

        public override void Init()
        {
            base.Init();
            
            _cam = Camera.main;
            
            //Initialize map here, create items if needed
            CreateMatchableObjectPositionMap();
        }



        private void CreateMatchableObjectPositionMap()
        {
            var boardSize = Tilemap.cellBounds;
            foreach (var pos in boardSize.allPositionsWithin)
            {
                MatchableObjectPositionMap.Add(pos, null);
            }
            
            foreach (var objectBase in MatchableObjects)
            {
                var cellPos = Grid.WorldToCell(objectBase.transform.position);

                if (!Tilemap.HasTile(cellPos))
                {
                    return;
                }

                objectBase.CurrentTilePos = cellPos;
            
                var cellCenter = Grid.GetCellCenterWorld(cellPos);
            
                objectBase.transform.position = cellCenter;
                objectBase.Initialize(Instance);
                MatchableObjectPositionMap[cellPos] = objectBase;

            }
        }

        public void OnMatchableObjectChangeTile(Vector3Int targetTilePos, MatchableObjectBase movingObject)
        {
            if (!MatchableObjectPositionMap.TryGetValue(targetTilePos, out var residentObject))
            {
                return;
            }

            if (residentObject == null)
            {
                // change tile map data of object moved by user
                MatchableObjectPositionMap[movingObject.CurrentTilePos] = null;
                MatchableObjectPositionMap[targetTilePos] = movingObject;
                movingObject.CurrentTilePos = targetTilePos;
                return;
            }

            if (residentObject.CanMerge(movingObject))
            {
                //merge them
                StartMerge(movingObject, residentObject);
                return;
            }
            
            if (!FindClosestEmptyTile(targetTilePos,out var closestTilePos))
            {
                return;
            }

            // set current tiles null
            MatchableObjectPositionMap[movingObject.CurrentTilePos] = null;
            MatchableObjectPositionMap[residentObject.CurrentTilePos] = null;

            // change tile map data of object moved by user
            MatchableObjectPositionMap[targetTilePos] = movingObject;
            movingObject.CurrentTilePos = targetTilePos;
            
            // find closest empty tile for old resident object and move there
            MatchableObjectPositionMap[closestTilePos] = residentObject;
            residentObject.CurrentTilePos = closestTilePos;
            residentObject.WhoopToTile(closestTilePos);
        }

        private void StartMerge(MatchableObjectBase movingObject, MatchableObjectBase residentObject)
        {
            movingObject.ObjectState = MatchableObjectBase.State.Merging;
            residentObject.ObjectState = MatchableObjectBase.State.Merging;

            var newItemTilePos = residentObject.CurrentTilePos;
            var mergingItemsChainIndex = residentObject.ChainPosition;
            var newGeneratedItemChainIndex = mergingItemsChainIndex + 1;
            
            MatchableObjectPositionMap[residentObject.CurrentTilePos] = null;
            MatchableObjectPositionMap[movingObject.CurrentTilePos] = null;
            
            GameManager.EventHandler.Notify(GameEvent.OnMergeItem, movingObject);

            
            Destroy(residentObject.gameObject);
            Destroy(movingObject.gameObject);
            
            var newGeneratedItemPrefab = ConfigManager.Instance.MatchableObjectConfig.MatchableObjects[newGeneratedItemChainIndex];

            var indexCount = ConfigManager.Instance.MatchableObjectConfig.MatchableObjects.Length -1;

            if (newGeneratedItemChainIndex > indexCount)
            {
                //Reached end of chain
                //Win Screen

                return;
            }
            

            if (newGeneratedItemPrefab == null)
            {
                //Reached end of chain
                //Win Screen
                return;
            }
            
            CreateNewItem(newGeneratedItemPrefab, newItemTilePos);
        }

        public bool IsTileAvailable(Vector3Int tilePos)
        {
            var obj = MatchableObjectPositionMap[tilePos];
            return obj.ObjectState != MatchableObjectBase.State.Blocked;
        }
        
        public bool FindClosestEmptyTile(Vector3Int originPos, out Vector3Int closestTilePos)
        {
            closestTilePos = default;
            
            List<Vector3Int> availableTiles = new List<Vector3Int>();

            foreach (var pair in MatchableObjectPositionMap)
            {
                if (pair.Value == null)
                {
                    availableTiles.Add(pair.Key);
                }
            }

            // No available tile found
            if (availableTiles.Count == 0)
            {
                return false; // or any appropriate value
            }

            // Step 4: Find closest available tile
            var closestTile = Vector3Int.zero;
            var shortestDistance = float.MaxValue;
            foreach (var availableTile in availableTiles)
            {
                if(availableTile == originPos)
                    continue;
                var distance = Vector3.Distance(originPos, Tilemap.GetCellCenterWorld(availableTile));
                if (!(distance < shortestDistance)) 
                    continue;
                shortestDistance = distance;
                closestTilePos = availableTile;
            }

            // Step 5: Return closest available tile
            return true;
        }

        private void CreateNewItem(MatchableObjectBase objectPrefab, Vector3Int tilePos)
        {
            var objectInstance = Instantiate(objectPrefab, _itemsParent);
            objectInstance.Initialize(Instance);
            objectInstance.MergeOut(tilePos);
            MatchableObjectPositionMap[tilePos] = objectInstance;
            objectInstance.CurrentTilePos = tilePos;
            GameManager.EventHandler.Notify(GameEvent.OnCreateItem, objectInstance);
        }

        public void CreateNewBooster(BoosterObject objectPrefab, Vector2 startPos)
        {
            var worldPos = _cam.ScreenToWorldPoint(startPos);
            if (!GetFirstEmptyTile(out var availableTile))
            {
                // no available tile 
                return;
            }
            var objectInstance = Instantiate(objectPrefab, _itemsParent);
            MatchableObjectPositionMap[availableTile] = objectInstance;
            objectInstance.CurrentTilePos = availableTile;
            objectInstance.Initialize(Instance);
            objectInstance.transform.position = worldPos;
            
            objectInstance.WhoopOnCreate(availableTile);
        }

        public void CreateObjectFromBooster(MatchableObjectBase objectPrefab, Vector2 startingPoint, Vector3Int targetTilePos)
        {
            var objectInstance = Instantiate(objectPrefab, _itemsParent);
            objectInstance.transform.position = startingPoint;
            objectInstance.Initialize(Instance);
            
            MatchableObjectPositionMap[targetTilePos] = objectInstance;
            objectInstance.CurrentTilePos = targetTilePos;
            
            objectInstance.WhoopToTileOnBoost(targetTilePos);

        }

        public bool GetEmptyTiles(out List<Vector3Int> emptyTiles)
        {
            emptyTiles = new List<Vector3Int>();
            var anyEmptyTile = false;
            foreach (var pair in MatchableObjectPositionMap)
            {
                if (pair.Value == null)
                {
                    emptyTiles.Add(pair.Key);
                    anyEmptyTile = true;
                }
            }

            return anyEmptyTile;
        }

        public bool GetFirstEmptyTile(out Vector3Int emptyTile)
        {
            emptyTile = new Vector3Int();
            
            foreach (var pair in MatchableObjectPositionMap)
            {
                if (pair.Value == null)
                {
                    emptyTile = pair.Key;
                    return true;
                }
            }
            
            return false;
        }

        
        #region Helper

        [Button("PutLevelItemsRandomly")]
        private void PutLevelItemsRandomly()
        {
            var randomItemList = new List<MatchableObjectBase>(MatchableObjects);
            randomItemList.Shuffle();
            var positionList = new List<Vector3Int>();
            
            var boardSize = Tilemap.cellBounds;
            foreach (var pos in boardSize.allPositionsWithin)
            {
                positionList.Add(pos);                
            }


            for (int i = 0; i < randomItemList.Count; i++)
            {
                var item = randomItemList[i];
                if (i >= positionList.Count)
                {
                    MatchableObjects.Remove(item);
                }
                var centerOfTile = Tilemap.GetCellCenterWorld(positionList[i]);

                item.transform.position = centerOfTile;
            }
        }

        public void DestroyObject(MatchableObjectBase obj)
        {
            MatchableObjects.Remove(obj);
            MatchableObjectPositionMap[obj.CurrentTilePos] = null;
            Destroy(obj.gameObject);
        }
        
        private void Reset()
        {
            Tilemap = GetComponentInChildren<Tilemap>();
            Grid = GetComponentInChildren<Grid>();
            MatchableObjects = FindObjectsOfType<MatchableObjectBase>().ToList();
        }

        #endregion


        
    }
}
