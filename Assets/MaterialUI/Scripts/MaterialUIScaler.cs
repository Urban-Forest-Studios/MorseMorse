using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [ExecuteInEditMode]
    public class MaterialUIScaler : MonoBehaviour
    {
        private Vector2 currentResolution;
        private Vector2 referenceResolution;

        private CanvasScaler scaler;

        public float scaleFactor { get; private set; }

        public void Update()
        {
            if (!scaler)
                scaler = gameObject.GetComponent<CanvasScaler>();

            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                referenceResolution = scaler.referenceResolution;
                currentResolution = new Vector2(Screen.width, Screen.height);

                scaleFactor = currentResolution.x * currentResolution.y /
                              (referenceResolution.x * referenceResolution.y);
            }
            else if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
            {
                scaleFactor = scaler.scaleFactor;
            }
        }
    }
}