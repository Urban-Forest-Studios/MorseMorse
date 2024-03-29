﻿//  Copyright 2014 Invex Games http://invexgames.com
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.

using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    [CustomEditor(typeof(ShadowGen))]
    internal class ShadowGenEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myTarget = (ShadowGen)target;

            if (GUILayout.Button("Generate Shadow")) myTarget.GetComponent<ShadowGen>().GenerateShadowFromImage();
        }
    }
}