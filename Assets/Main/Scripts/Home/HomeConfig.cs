using System.Collections.Generic;
using Main.Scripts.Game.HomePhaseController;
using UnityEngine;

namespace Main.Scripts.Home
{
    [CreateAssetMenu]
    public class HomeConfig : ScriptableObject
    {
        public List<HomePhaseData> Data;
    }
}