﻿//  Copyright 2014 Invex Games http://invexgames.com
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.

//	Used to automatically snap a shadow to a target (you could position it manually, this just makes it easier)

using UnityEngine;

namespace MaterialUI
{
    [ExecuteInEditMode]
    public class ShadowSnap : MonoBehaviour
    {
        public RectTransform targetRect;

        public float xPadding;
        public float yPadding;

        public bool percentage;

        public float xPercent;
        public float yPercent;
        private Vector3 lastPos;

        private Rect lastRect;
        private RectTransform thisRect;

        private void Start()
        {
            if (!thisRect) thisRect = gameObject.GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (targetRect)
            {
                if (!thisRect) thisRect = gameObject.GetComponent<RectTransform>();

                Vector2 tempVect2;

                if (percentage)
                {
                    tempVect2.x = targetRect.sizeDelta.x * xPercent * 0.01f;
                    tempVect2.y = targetRect.sizeDelta.y * yPercent * 0.01f;
                }
                else
                {
                    tempVect2.x = targetRect.sizeDelta.x + xPadding;
                    tempVect2.y = targetRect.sizeDelta.y + yPadding;
                }

                thisRect.sizeDelta = tempVect2;
            }
        }
    }
}