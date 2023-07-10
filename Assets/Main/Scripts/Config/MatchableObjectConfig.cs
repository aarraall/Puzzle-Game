using Main.Scripts.Game.MatchableObject;
using UnityEngine;

namespace Main.Scripts.Config
{
    [CreateAssetMenu]
    public class MatchableObjectConfig : ScriptableObject
    {
        public MatchableObjectBase[] MatchableObjects;
    }
}
