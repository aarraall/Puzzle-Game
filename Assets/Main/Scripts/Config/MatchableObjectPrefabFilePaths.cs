using System.Collections.Generic;
using System.Text;
using Main.Scripts.Game.MatchableObject;

namespace Main.Scripts.Config
{
    public static class MatchableObjectPrefabFilePaths
    {
        private const string k_path_prefix = "Prefabs/MatchableObjects";

        private static Dictionary<MatchableObjectBase.Type, string[]> _map =
            new()
            {
                {
                    MatchableObjectBase.Type.Classic, new[]
                    {
                        "/Classic/MatchableObject1",
                        "/Classic/MatchableObject2",
                        "/Classic/MatchableObject3",
                    }
                }, 
                {
                    MatchableObjectBase.Type.Letter_I, new[]
                    {
                        "/Letter_I/MatchableObject1",
                        "/Letter_I/MatchableObject2",
                        "/Letter_I/MatchableObject3",
                    }
                },
                {
                    MatchableObjectBase.Type.Letter_L, new[]
                    {
                        "/Letter_L/MatchableObject1",
                        "/Letter_L/MatchableObject2",
                        "/Letter_L/MatchableObject3",
                    }
                },
                {
                    MatchableObjectBase.Type.Letter_U, new[]
                    {
                        "/Letter_U/MatchableObject1",
                        "/Letter_U/MatchableObject2",
                        "/Letter_U/MatchableObject3",
                    }
                },
                
                
            };
        
        public static string GetPathByType(MatchableObjectBase.Type type, int chainIndex)
        {
            var pathString = new StringBuilder();
            pathString.Append(k_path_prefix);
            //add exact path
            pathString.Append(_map[type][chainIndex]);

            return pathString.ToString();
        }
    }
}
