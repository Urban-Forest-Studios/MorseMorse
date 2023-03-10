using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Google.MaterialDesign.Icons
{
    internal static class MenuOptions
    {
        [MenuItem("GameObject/Google/Material Icon", false, 10)]
        public static void CreateMaterialIcon(MenuCommand menuCommand)
        {
            var parent = menuCommand.context as GameObject;

            if (parent == null || parent.GetComponentsInParent<Canvas>(true).Length == 0)
            {
                var canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas.layer = LayerMask.NameToLayer("UI");
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                GameObjectUtility.SetParentAndAlign(canvas, parent);
                Undo.RegisterCreatedObjectUndo(canvas, "Create " + canvas.name);

                if (Object.FindObjectOfType<EventSystem>() == null)
                {
                    var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                    GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                    Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
                }

                parent = canvas;
            }

            var gameObject = new GameObject("MaterialIcon", typeof(MaterialIcon));
            gameObject.layer = LayerMask.NameToLayer("UI");
            GameObjectUtility.SetParentAndAlign(gameObject, parent);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
            Selection.activeObject = gameObject;
        }
    }
}