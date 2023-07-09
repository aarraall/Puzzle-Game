using System.Collections.Generic;
using Main.Scripts.Config;
using Main.Scripts.Core;
using Main.Scripts.Game.EventHandler;
using Main.Scripts.Game.MatchableObject;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Main.Scripts.Board
{
    public class BoardHandler : MonoSingleton<BoardHandler>
    {
        public Tilemap Tilemap;
        public Grid Grid;
        public LevelConfig LevelConfig;
        public Dictionary<MatchableObjectBase, Vector3Int> MatchableObjects = new Dictionary<MatchableObjectBase, Vector3Int>();

        public override void Init()
        {
            base.Init();
            
            Subscribe();
            
            //Initialize map here, create items if needed
            CreateMatchableObjects();
        }

        protected override void Dispose()
        {
            base.Dispose();
            
            Unsubscribe();
        }
        
        public void Subscribe()
        {
            GameManager.Instance.EventHandler.Subscribe(GameEvent.OnTapItem, OnObjectTap);
            GameManager.Instance.EventHandler.Subscribe(GameEvent.OnDragItem, OnObjectHold);
            GameManager.Instance.EventHandler.Subscribe(GameEvent.OnReleaseItem, OnObjectRelease);
        }

        public void Unsubscribe()
        {
            GameManager.Instance.EventHandler.Unsubscribe(GameEvent.OnTapItem, OnObjectTap);
            GameManager.Instance.EventHandler.Unsubscribe(GameEvent.OnDragItem, OnObjectHold);
            GameManager.Instance.EventHandler.Unsubscribe(GameEvent.OnReleaseItem, OnObjectRelease);
        }


        private void CreateMatchableObjects()
        {
            foreach (var itemMap in LevelConfig.ItemMapList)
            {
                var itemPrefabPath = MatchableObjectPrefabFilePaths.GetPathByType(itemMap.ObjectType, itemMap.ChainIndex);
                var itemPrefab = Resources.Load(itemPrefabPath) as GameObject;
                if (itemPrefab == null)
                {
                    continue;
                }
                var createdItemInstance = Instantiate(itemPrefab.GetComponent<MatchableObjectBase>());
                createdItemInstance.Initialize(this);
                createdItemInstance.transform.position = Tilemap.GetCellCenterWorld(itemMap.Coord);
                MatchableObjects.Add(createdItemInstance, itemMap.Coord);
            }
        }


        [Button("Read tiles and print names of objects on top of them")]
        public void ReadTiles()
        {
            var tileMapBound = Tilemap.cellBounds.allPositionsWithin;

            foreach (var pos in tileMapBound)
            {
                var coords = new Vector3Int(pos.x, pos.y, pos.z);

                if (!Tilemap.HasTile(coords))
                {
                    continue;
                }

            }
        }
        public void OnObjectTap(object eventData)
        {
            
        }

        public void OnObjectHold(object eventData)
        {
            
        }

        public void OnObjectRelease(object eventData)
        {
            
        }

        public void StartMatch(MatchableObjectBase matchOrigin)
        {
            
        }


        private void Reset()
        {
            Tilemap = GetComponentInChildren<Tilemap>();
            Grid = GetComponent<Grid>();
        }
    }
}
