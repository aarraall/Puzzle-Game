using System.Collections.Generic;
using Main.Scripts.Board;
using Main.Scripts.Game.MatchableObject;
using UnityEngine;

namespace Main.Scripts.Config
{
    [CreateAssetMenu]
    public class LevelConfig : ScriptableObject
    {
        [System.Serializable]
        public struct ItemMap
        {
            public Vector3Int Coord;
            public MatchableObjectBase.Type ObjectType;
            public int ChainIndex;

            public ItemMap(Vector3Int coord, MatchableObjectBase.Type objectType, int chainIndex)
            {
                Coord = coord;
                ObjectType = objectType;
                ChainIndex = chainIndex;
            }
        }

        public string LevelSceneID;

        public BoardHandler boardHandler;
        
        public List<ItemMap> ItemMapList = new List<ItemMap>();
    }
}
