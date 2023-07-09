using Main.Scripts.Util.Helpers;
using UnityEngine;

public class SpriteScaler : MonoBehaviour
{
    public bool Width, Height;
    private void Start()
    {
        if (Width)
        {
            var width = ScreenSize.GetScreenToWorldWidth;
            transform.localScale = Vector3.right * width;
        }

        if (Height)
        {
            var width = ScreenSize.GetScreenToWorldWidth;
            transform.localScale = Vector3.up * width;
        }
        
    }

}
