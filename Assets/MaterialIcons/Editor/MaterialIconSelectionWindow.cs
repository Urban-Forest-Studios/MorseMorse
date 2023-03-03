using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Google.MaterialDesign.Icons
{
    public class MaterialIconSelectionWindow : EditorWindow
    {
        private static readonly Color darkColor = new(0.196f, 0.196f, 0.196f);
        private static readonly Color lightColor = new(0.804f, 0.804f, 0.804f);

        private readonly int iconSize = 58;
        private readonly int labelHeight = 24;
        private readonly int spacing = 10;
        private CodepointData[] codepointsCollection;
        private CodepointData[] filteredCollection;
        private bool filterGotFocus;
        private string filterText = string.Empty;
        private GUIStyle iconImageStyle;
        private GUIStyle iconLabelStyle;
        private GUIStyle iconSelectionStyle;

        private Font MaterialIconsRegular;
        private Action<string> onSelectionChanged;
        private Vector2 scrollPos = Vector2.zero;

        private string selected;
        private string selectedName;
        private bool selectionKeep;
        private bool showNames = true;
        private GUIStyle toolbarLabelStyle;
        private GUIStyle toolbarSeachCancelButtonEmptyStyle;
        private GUIStyle toolbarSeachCancelButtonStyle;

        private GUIStyle toolbarSeachTextFieldStyle;

        private void OnEnable()
        {
            titleContent = new GUIContent("Material Icon Selection");
            minSize = new Vector2((iconSize + labelHeight + spacing) * 5f + GUI.skin.verticalScrollbar.fixedWidth + 1f,
                (iconSize + labelHeight + spacing) * 6f + EditorStyles.toolbar.fixedHeight);
            selectionKeep = true;
        }

        private void OnGUI()
        {
            if (toolbarSeachTextFieldStyle == null || iconImageStyle == null)
            {
                toolbarSeachTextFieldStyle = new GUIStyle("ToolbarSeachTextField");
                toolbarSeachCancelButtonStyle = new GUIStyle("ToolbarSeachCancelButton");
                toolbarSeachCancelButtonEmptyStyle = new GUIStyle("ToolbarSeachCancelButtonEmpty");
                toolbarLabelStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
                iconSelectionStyle = new GUIStyle("selectionrect");
                iconImageStyle = new GUIStyle
                {
                    font = MaterialIconsRegular, fontSize = iconSize - spacing - 10, alignment = TextAnchor.MiddleCenter
                };
                iconLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                    { alignment = TextAnchor.UpperCenter, wordWrap = true };
                iconImageStyle.padding = iconLabelStyle.padding = new RectOffset();
                iconImageStyle.normal.textColor = iconLabelStyle.normal.textColor =
                    EditorGUIUtility.isProSkin ? lightColor : darkColor;
            }

            if (MaterialIconsRegular == null)
            {
                EditorGUILayout.HelpBox("Could not find \"MaterialIcons-Regular\" font data.", MessageType.Error);
                return;
            }

            if (codepointsCollection == null || codepointsCollection.Length == 0)
            {
                EditorGUILayout.HelpBox("Could not find \"codepoints\" font data.", MessageType.Error);
                return;
            }

            OnHeaderGUI();
            OnBodyGUI();
        }

        public void LoadDependencies(Font MaterialIconsRegular)
        {
            showNames = EditorPrefs.GetBool(typeof(MaterialIconSelectionWindow) + ".showNames", true);

            if (MaterialIconsRegular == null)
                return;

            this.MaterialIconsRegular = MaterialIconsRegular;

            var fontPath = AssetDatabase.GetAssetPath(MaterialIconsRegular);
            var codepointsPath = Path.GetDirectoryName(fontPath) + "/codepoints";

            var tempList = new List<CodepointData>();

            foreach (var codepoint in File.ReadAllLines(codepointsPath))
            {
                var data = codepoint.Split(' ');
                tempList.Add(new CodepointData(data[0], data[1]));
            }

            codepointsCollection = tempList.ToArray();
            filteredCollection = codepointsCollection;

            var temp = filteredCollection.FirstOrDefault(data => data.codeGUIContent.text == selected);
            if (temp != null)
                selectedName = temp.name;
        }

        public static void Init(Font MaterialIconsRegular, string preSelect, Action<string> callback)
        {
            var window = GetWindow<MaterialIconSelectionWindow>(true);
            window.selected = preSelect;
            window.onSelectionChanged = callback;
            window.LoadDependencies(MaterialIconsRegular);
        }

        private void OnHeaderGUI()
        {
            var groupRect = new Rect(0f, 0f, position.width, EditorStyles.toolbar.fixedHeight);
            GUI.BeginGroup(groupRect);

            if (Event.current.type == EventType.Repaint)
                EditorStyles.toolbar.Draw(groupRect, false, false, false, false);

            var filterRect = new Rect(6f, 2f, groupRect.width - 6f - 20f - 64f - 6f, groupRect.height - 2f);
            var clearRect = new Rect(filterRect.x + filterRect.width, filterRect.y, 20f, filterRect.height);

            EditorGUI.BeginChangeCheck();

            GUI.SetNextControlName(typeof(MaterialIconSelectionWindow) + ".filterText");
            filterText = EditorGUI.TextField(filterRect, filterText, toolbarSeachTextFieldStyle);

            if (GUI.Button(clearRect, GUIContent.none,
                    string.IsNullOrEmpty(filterText)
                        ? toolbarSeachCancelButtonEmptyStyle
                        : toolbarSeachCancelButtonStyle))
            {
                filterText = string.Empty;
                GUI.FocusControl(null);
            }

            if (!filterGotFocus)
            {
                EditorGUI.FocusTextInControl(typeof(MaterialIconSelectionWindow) + ".filterText");
                filterGotFocus = true;
            }

            if (EditorGUI.EndChangeCheck())
            {
                filteredCollection = codepointsCollection.Where(data =>
                    string.IsNullOrEmpty(filterText) ||
                    data.nameGUIContent.text.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
                selectionKeep = true;
            }

            var nameRect = new Rect(clearRect.x + clearRect.width, groupRect.y, 64f, groupRect.height);
            EditorGUI.BeginChangeCheck();
            showNames = EditorGUI.Toggle(nameRect, showNames, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
            {
                GUI.FocusControl(null);
                EditorPrefs.SetBool(typeof(MaterialIconSelectionWindow) + ".showNames", showNames);
            }

            EditorGUI.LabelField(nameRect, "Names", toolbarLabelStyle);

            GUI.EndGroup();
        }

        private void OnBodyGUI()
        {
            var iconRect = new Rect(0f, 0f, iconSize + labelHeight, iconSize);
            var labelRect = new Rect(0f, 0f, iconRect.width, labelHeight);
            if (!showNames)
            {
                iconRect.width -= labelHeight;
                labelRect.height = 0f;
            }

            var buttonRect = new Rect(0f, 0f, iconRect.width + spacing, iconRect.height + labelRect.height + spacing);

            var groupRect = new Rect(0f, EditorStyles.toolbar.fixedHeight, position.width,
                position.height - EditorStyles.toolbar.fixedHeight);
            GUI.BeginGroup(groupRect);

            var scrollRect = new Rect(0f, 0f, groupRect.width, groupRect.height);
            var columns = Mathf.FloorToInt((scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth) /
                                           (iconRect.width + spacing));
            var viewRect = new Rect(0f, 0f, scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth,
                Mathf.Ceil(filteredCollection.Length / (float)columns) *
                (iconRect.height + labelRect.height + spacing));

            scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, viewRect);

            for (var i = 0; i < filteredCollection.Length; i += columns)
            for (var j = 0; j < columns; j++)
            {
                if (i + j >= filteredCollection.Length)
                    break;

                var data = filteredCollection[i + j];

                iconRect.x = j * (iconRect.width + spacing) + spacing / 2f;
                iconRect.y = i / (float)columns * (iconRect.height + labelRect.height + spacing) + spacing / 2f;

                labelRect.x = iconRect.x;
                labelRect.y = iconRect.y + iconRect.height;

                buttonRect.x = iconRect.x - spacing / 2f;
                buttonRect.y = iconRect.y - spacing / 2f;

                if (data.name == selectedName)
                {
                    if (Event.current.type == EventType.Repaint)
                        iconSelectionStyle.Draw(buttonRect, false, true, true, true);

                    if (selectionKeep)
                    {
                        if (buttonRect.y + buttonRect.height > scrollPos.y + scrollRect.height)
                            scrollPos.y = buttonRect.y + buttonRect.height - scrollRect.height;
                        else if (buttonRect.y < scrollPos.y)
                            scrollPos.y = buttonRect.y;

                        selectionKeep = false;
                        Repaint();
                    }
                }

                GUI.Label(iconRect, data.codeGUIContent, iconImageStyle);
                if (showNames)
                    GUI.Label(labelRect, data.nameGUIContent, iconLabelStyle);

                if (GUI.Button(buttonRect, GUIContent.none, GUIStyle.none))
                {
                    GUI.FocusControl(null);
                    var shouldClose = data.codeGUIContent.text == selected;
                    selected = data.codeGUIContent.text;
                    selectedName = data.name;
                    onSelectionChanged.Invoke(selected);
                    if (shouldClose)
                        Close();
                }
            }

            GUI.EndScrollView();
            GUI.EndGroup();

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.LeftArrow)
                {
                    SelectRelative(-1);
                    Event.current.Use();
                }

                if (Event.current.keyCode == KeyCode.RightArrow)
                {
                    SelectRelative(+1);
                    Event.current.Use();
                }

                if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    SelectRelative(-columns);
                    Event.current.Use();
                }

                if (Event.current.keyCode == KeyCode.DownArrow)
                {
                    SelectRelative(+columns);
                    Event.current.Use();
                }

                if (Event.current.keyCode == KeyCode.PageUp)
                {
                    SelectRelative(-(columns * 6));
                    Event.current.Use();
                }

                if (Event.current.keyCode == KeyCode.PageDown)
                {
                    SelectRelative(+(columns * 6));
                    Event.current.Use();
                }

                if (Event.current.keyCode == KeyCode.Home)
                {
                    SelectAbsolute(0);
                    Event.current.Use();
                }

                if (Event.current.keyCode == KeyCode.End)
                {
                    SelectAbsolute(filteredCollection.Length - 1);
                    Event.current.Use();
                }
            }
        }

        private void SelectRelative(int delta)
        {
            SelectAbsolute(Array.FindIndex(filteredCollection, data => data.name == selectedName) + delta);
        }

        private void SelectAbsolute(int index)
        {
            index = Mathf.Clamp(index, 0, filteredCollection.Length - 1);

            selected = filteredCollection[index].codeGUIContent.text;
            selectedName = filteredCollection[index].name;
            onSelectionChanged.Invoke(selected);
            selectionKeep = true;
            Repaint();
        }

        [Serializable]
        public class CodepointData
        {
            public CodepointData(string name, string code)
            {
                this.name = name;
                this.code = code;
                nameGUIContent =
                    new GUIContent(string.Format("{0} ({1})", name.ToLowerInvariant().Replace('_', ' '), code));
                codeGUIContent = new GUIContent(char.ConvertFromUtf32(Convert.ToInt32(this.code, 16)),
                    nameGUIContent.text);
            }

            public string name { get; private set; }
            public string code { get; private set; }
            public GUIContent nameGUIContent { get; private set; }
            public GUIContent codeGUIContent { get; private set; }
        }
    }
}