//  Copyright 2014 Invex Games http://invexgames.com
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.

//	Credit to the 'EventTriggerEditor' script at http://www.cratesmith.com/archives/221, which helped immensely

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    [CustomEditor(typeof(EZAnim))]
    [Serializable]
    public class EZAnimEditor : Editor
    {
        private string[] inclusionNames;

        private string[] inclusions;
        private string[] methodNameExclusions;
        private EZAnim scriptTarget;
        private EZStruct tempStruct;

        private void Awake()
        {
            scriptTarget = (EZAnim)target;
            if (scriptTarget.theStructs == null)
            {
                scriptTarget.theStructs = new List<EZStruct>();
                scriptTarget.theStructs.Add(new EZStruct());

                tempStruct = scriptTarget.theStructs[0];
                tempStruct.animationType = AnimType.EaseOutQuint;
                tempStruct.animationDuration = 1f;
                scriptTarget.theStructs[0] = tempStruct;
            }

            inclusions = new[]
            {
                "System.Single", "System.Int32", "UnityEngine.Vector2", "UnityEngine.Vector3", "UnityEngine.Rect",
                "UnityEngine.Color", "UnityEngine.Material"
            };
            inclusionNames = new[] { "Float", "Int", "Vector2", "Vector3", "Rect", "Color", "Material" };
            methodNameExclusions = new[]
                { "Update", "FixedUpdate", "LateUpdate", "Start", "Awake", "OnEnable", "OnDisable" };
        }

        private string[] FindComponentStrings(GameObject targetGameObject)
        {
            var componentStrings = new List<string>();
            var components = targetGameObject.GetComponents<Component>();
            foreach (var component in components) componentStrings.Add(component.GetType().Name);

            componentStrings.Add("--NONE--");
            return componentStrings.ToArray();
        }

        private string[] FindMethodStrings(GameObject targetGameObject, string targetComponent)
        {
            var methodStrigs = new List<string>();

            if (targetGameObject)
                if (targetGameObject.GetComponent(targetComponent))
                {
                    var realComponent = targetGameObject.GetComponent(targetComponent);
                    var behaviorType = realComponent.GetType();

                    var methods = behaviorType.GetMethods();
                    foreach (var method in methods)
                    {
                        // events are currently restrected to public, zero parameter functions, that aren't constructors
                        if (method.GetParameters().Length > 1 || method.IsConstructor || !method.IsPublic)
                            continue;

                        // return type void only
                        if (method.ReturnType != typeof(void))
                            continue;

                        // skip all base class methods from monodevelop, component and UnityEngine.Object
                        if (method.DeclaringType.IsAssignableFrom(typeof(MonoBehaviour)))
                            continue;

                        var skip = false;

                        foreach (var exclusion in methodNameExclusions)
                            if (method.Name == exclusion)
                                skip = true;

                        if (skip)
                            continue;

                        methodStrigs.Add(method.Name);
                    }
                }

            methodStrigs.Add("--NONE--");
            return methodStrigs.ToArray();
        }

        private string[] FindVariableStrings(GameObject targetGameObject, string targetComponent)
        {
            var variableStrings = new List<string>();

            if (targetGameObject)
                if (targetGameObject.GetComponent(targetComponent))
                {
                    var realComponent = targetGameObject.GetComponent(targetComponent);
                    var behaviorType = realComponent.GetType();

                    if (behaviorType.GetProperties().Length > 0)
                    {
                        var properties = behaviorType.GetProperties();
                        foreach (var property in properties)
                        {
                            var propertyType = property.PropertyType.ToString();

                            foreach (var inclusion in inclusions)
                                if (propertyType == inclusion && property.CanWrite)
                                    variableStrings.Add(property.Name);
                        }

                        tempStruct.valueType = ValType.Property;
                    }
                    else if (behaviorType.GetFields().Length > 0)
                    {
                        var fields = behaviorType.GetFields();
                        foreach (var field in fields)
                        {
                            var fieldType = field.FieldType.ToString();
                            foreach (var inclusion in inclusions)
                                if (fieldType == inclusion)
                                    variableStrings.Add(field.Name);
                        }

                        tempStruct.valueType = ValType.Field;
                    }
                }

            variableStrings.Add("--NONE--");
            return variableStrings.ToArray();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (scriptTarget.theStructs.Count == 0)
                scriptTarget.theStructs.Add(new EZStruct());


            for (var i = 0; i < scriptTarget.theStructs.Count; i++)
            {
                EditorGUILayout.Space();

                ConfigureStruct(i);

                EditorGUILayout.Space();

                if (!(i == 0 && scriptTarget.theStructs.Count == 1))
                    if (GUILayout.Button(new GUIContent("Remove")))
                        scriptTarget.theStructs.RemoveAt(i);
                if (i == scriptTarget.theStructs.Count - 1)
                    if (GUILayout.Button(new GUIContent("Add")))
                        scriptTarget.theStructs.Add(scriptTarget.theStructs[scriptTarget.theStructs.Count - 1]);
            }
        }

        private void ConfigureStruct(int index)
        {
            tempStruct = scriptTarget.theStructs[index];

            var compStrings = tempStruct.targetGameObject != null
                ? FindComponentStrings(tempStruct.targetGameObject)
                : new[] { "No object selected" };
            var varStrings = tempStruct.targetGameObject != null
                ? FindVariableStrings(tempStruct.targetGameObject, tempStruct.targetComponent)
                : new[] { "No object selected" };

            tempStruct.animName = EditorGUILayout.TextField("Name", tempStruct.animName);
            tempStruct.animTag = EditorGUILayout.TextField("Tag", tempStruct.animTag);

            EditorGUILayout.Space();

            tempStruct.targetGameObject = (GameObject)EditorGUILayout.ObjectField("Target GameObject",
                tempStruct.targetGameObject, typeof(GameObject), true);

            if (tempStruct.targetGameObject)
                tempStruct.targetComponent = StringPopup("Target Component", compStrings, tempStruct.targetComponent);

            if (tempStruct.targetGameObject && tempStruct.targetGameObject.GetComponent(tempStruct.targetComponent))
                tempStruct.targetVariable = StringPopup("Target Variable", varStrings, tempStruct.targetVariable);

            EditorGUILayout.Space();

            if (tempStruct.valueType == ValType.Field)
            {
                if (tempStruct.targetGameObject != null && tempStruct.targetComponent != null)
                {
                    if (tempStruct.targetGameObject.GetComponent(tempStruct.targetComponent))
                    {
                        if (tempStruct.targetGameObject.GetComponent(tempStruct.targetComponent).GetType()
                                .GetField(tempStruct.targetVariable) != null)
                        {
                            tempStruct.realField = tempStruct.targetGameObject.GetComponent(tempStruct.targetComponent)
                                .GetType().GetField(tempStruct.targetVariable);

                            for (var i = 0; i < inclusions.Length; i++)
                                if (tempStruct.realField.FieldType.ToString() == inclusions[i])
                                {
                                    EditorGUILayout.LabelField("Variable is of type " + inclusionNames[i]);
                                    tempStruct.variableType = inclusions[i];
                                }
                        }
                        else
                        {
                            tempStruct.variableType = "";
                        }
                    }
                    else
                    {
                        tempStruct.variableType = "";
                    }
                }
                else
                {
                    tempStruct.variableType = "";
                }
            }
            else if (tempStruct.valueType == ValType.Property)
            {
                if (tempStruct.targetGameObject != null && tempStruct.targetComponent != null)
                {
                    if (tempStruct.targetGameObject.GetComponent(tempStruct.targetComponent))
                    {
                        if (tempStruct.targetGameObject.GetComponent(tempStruct.targetComponent).GetType()
                                .GetProperty(tempStruct.targetVariable) != null)
                        {
                            tempStruct.realProperty = tempStruct.targetGameObject
                                .GetComponent(tempStruct.targetComponent).GetType()
                                .GetProperty(tempStruct.targetVariable);

                            for (var i = 0; i < inclusions.Length; i++)
                                if (tempStruct.realProperty.PropertyType.ToString() == inclusions[i])
                                {
                                    EditorGUILayout.LabelField("Variable is of type " + inclusionNames[i]);
                                    tempStruct.variableType = inclusions[i];
                                }
                        }
                        else
                        {
                            tempStruct.variableType = "";
                        }
                    }
                    else
                    {
                        tempStruct.variableType = "";
                    }
                }
                else
                {
                    tempStruct.variableType = "";
                }
            }

            EditorGUILayout.Space();

            if (tempStruct.variableType == "System.Single")
            {
                tempStruct.targetFloat = EditorGUILayout.FloatField("Target float", tempStruct.targetFloat);
            }
            else if (tempStruct.variableType == "System.Int32")
            {
                tempStruct.targetInt = EditorGUILayout.IntField("Target int", tempStruct.targetInt);
            }
            else if (tempStruct.variableType == "UnityEngine.Vector2")
            {
                tempStruct.modifyParameter1 = EditorGUILayout.Toggle("Modify x parameter", tempStruct.modifyParameter1);

                tempStruct.modifyParameter2 = EditorGUILayout.Toggle("Modify y parameter", tempStruct.modifyParameter2);

                tempStruct.targetVector2 = EditorGUILayout.Vector2Field("Target Vector 2", tempStruct.targetVector2);
            }
            else if (tempStruct.variableType == "UnityEngine.Vector3")
            {
                tempStruct.modifyParameter1 = EditorGUILayout.Toggle("Modify x parameter", tempStruct.modifyParameter1);

                tempStruct.modifyParameter2 = EditorGUILayout.Toggle("Modify y parameter", tempStruct.modifyParameter2);

                tempStruct.modifyParameter3 = EditorGUILayout.Toggle("Modify z parameter", tempStruct.modifyParameter3);

                tempStruct.targetVector3 = EditorGUILayout.Vector3Field("Target Vector 3", tempStruct.targetVector3);
            }
            else if (tempStruct.variableType == "UnityEngine.Rect")
            {
                tempStruct.modifyParameter1 = EditorGUILayout.Toggle("Modify x parameter", tempStruct.modifyParameter1);

                tempStruct.modifyParameter2 = EditorGUILayout.Toggle("Modify y parameter", tempStruct.modifyParameter2);

                tempStruct.modifyParameter3 =
                    EditorGUILayout.Toggle("Modify width parameter", tempStruct.modifyParameter3);

                tempStruct.modifyParameter4 =
                    EditorGUILayout.Toggle("Modify height parameter", tempStruct.modifyParameter4);

                tempStruct.targetRect = EditorGUILayout.RectField("Target Rect", tempStruct.targetRect);
            }
            else if (tempStruct.variableType == "UnityEngine.Color" ||
                     tempStruct.variableType == "UnityEngine.Material")
            {
                tempStruct.modifyParameter1 =
                    EditorGUILayout.Toggle("Modify red parameter", tempStruct.modifyParameter1);

                tempStruct.modifyParameter2 =
                    EditorGUILayout.Toggle("Modify green parameter", tempStruct.modifyParameter2);

                tempStruct.modifyParameter3 =
                    EditorGUILayout.Toggle("Modify blue parameter", tempStruct.modifyParameter3);

                tempStruct.modifyParameter4 =
                    EditorGUILayout.Toggle("Modify alpha parameter", tempStruct.modifyParameter4);

                tempStruct.targetColor = EditorGUILayout.ColorField("Target Color", tempStruct.targetColor);
            }

            if (tempStruct.targetVariable != "" && tempStruct.targetGameObject != null &&
                tempStruct.targetComponent != null)
            {
                EditorGUILayout.Space();

                tempStruct.animationType =
                    (AnimType)EditorGUILayout.EnumPopup("Animation Type", tempStruct.animationType);

                tempStruct.animationDuration =
                    EditorGUILayout.FloatField("Animation Duration", tempStruct.animationDuration);

                tempStruct.delay = EditorGUILayout.FloatField("Start delay (Seconds)", tempStruct.delay);
            }


            if (tempStruct.targetVariable != "" && tempStruct.targetGameObject != null &&
                tempStruct.targetComponent != null)
            {
                tempStruct.methodOnEnd =
                    EditorGUILayout.Toggle("Call function on animation end", tempStruct.methodOnEnd);

                if (tempStruct.methodOnEnd)
                {
                    EditorGUILayout.Space();

                    var endMethodCompStrings = tempStruct.methodTargetGameObject != null
                        ? FindComponentStrings(tempStruct.methodTargetGameObject)
                        : new[] { "No object selected" };
                    var endMethodStrings = tempStruct.methodTargetGameObject != null
                        ? FindMethodStrings(tempStruct.methodTargetGameObject, tempStruct.methodTargetComponent)
                        : new[] { "No object selected" };

                    tempStruct.methodTargetGameObject =
                        (GameObject)
                        EditorGUILayout.ObjectField("Target GameObject", tempStruct.methodTargetGameObject,
                            typeof(GameObject), true);

                    if (tempStruct.methodTargetGameObject)
                        tempStruct.methodTargetComponent = StringPopup("Target Component", endMethodCompStrings,
                            tempStruct.methodTargetComponent);

                    if (tempStruct.methodTargetGameObject &&
                        tempStruct.methodTargetGameObject.GetComponent(tempStruct.methodTargetComponent))
                        tempStruct.methodTargetMethod = StringPopup("Target Function", endMethodStrings,
                            tempStruct.methodTargetMethod);

                    if (tempStruct.methodTargetGameObject &&
                        tempStruct.methodTargetGameObject.GetComponent(tempStruct.methodTargetComponent) &&
                        tempStruct.methodTargetMethod != "")
                        tempStruct.methodParam = EditorGUILayout.TextField("Function argument", tempStruct.methodParam);
                }
            }
            else
            {
                tempStruct.methodOnEnd = false;
            }

            scriptTarget.theStructs[index] = tempStruct;
        }

        private static string StringPopup(string label, string[] strings, string str)
        {
            var strId = Array.IndexOf(strings, str);

            if (strId == -1)
                strId = strings.Length - 1;

            strId = EditorGUILayout.Popup(label, strId, strings, "popup");

            if (strId == -1 || strId == strings.Length - 1)
                return "";

            return strings[strId];
        }
    }
}