using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Google.MaterialDesign.Icons
{
    public class MaterialIcon : Text
    {
        public string iconUnicode
        {
            get => Convert.ToString(char.ConvertToUtf32(base.text, 0), 16);
            set => base.text = char.ConvertFromUtf32(Convert.ToInt32(value, 16));
        }

        protected override void Start()
        {
            base.Start();

            if (string.IsNullOrEmpty(base.text)) Init();

#if UNITY_EDITOR
            if (font == null) LoadFont();
#endif
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            fontSize = Mathf.FloorToInt(Mathf.Min(rectTransform.rect.width, rectTransform.rect.height));
        }

        /// <summary> Properly initializes base Text class. </summary>
        public void Init()
        {
            base.text = "\ue84d";
            font = null;
            base.color = new Color(0.196f, 0.196f, 0.196f, 1.000f);
            base.material = null;
            alignment = TextAnchor.MiddleCenter;
            supportRichText = false;
            horizontalOverflow = HorizontalWrapMode.Overflow;
            verticalOverflow = VerticalWrapMode.Overflow;
            fontSize = Mathf.FloorToInt(Mathf.Min(rectTransform.rect.width, rectTransform.rect.height));
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            Init();
            LoadFont();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            base.SetLayoutDirty();
        }

        /// <summary> Searches for the \"MaterialIcons-Regular\" font inside the project. </summary>
        public void LoadFont()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Font MaterialIcons-Regular"))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if (assetPath.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) &&
                    File.Exists(Path.GetDirectoryName(assetPath) + "/codepoints"))
                {
                    font = AssetDatabase.LoadAssetAtPath<Font>(assetPath);
                    break;
                }
            }
        }
#endif
    }
}