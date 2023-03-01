//  Copyright 2014 Invex Games http://invexgames.com
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.

using UnityEngine;

namespace MaterialUI
{
    [ExecuteInEditMode]
    public class SnapButtonToText : MonoBehaviour
    {
        [SerializeField] private RectTransform buttonRectTransform;

        public bool snapEveryFrame = true;

        [SerializeField] private Vector2 basePadding = new(30f, 18f);
        [SerializeField] private Vector2 buttonPadding = new(32f, 32f);
        private Vector2 buttonSize;
        private Vector2 finalSize;

        private Vector2 textSize;

        private RectTransform thisRectTransform;

        public void Update()
        {
            if (!snapEveryFrame) return;

            if (thisRectTransform.sizeDelta != textSize)
            {
                textSize = thisRectTransform.sizeDelta;
                Snap();
            }
        }

        private void OnEnable()
        {
            thisRectTransform = gameObject.GetComponent<RectTransform>();
        }

        public void Snap()
        {
            buttonSize = textSize + basePadding;

            finalSize = buttonSize + buttonPadding;

            if (finalSize.x < 96f)
                finalSize.x = 96f;

            buttonRectTransform.sizeDelta = finalSize;
        }
    }
}