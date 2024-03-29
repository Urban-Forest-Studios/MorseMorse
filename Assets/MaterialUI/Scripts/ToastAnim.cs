﻿//  Copyright 2014 Invex Games http://invexgames.com
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    public class ToastAnim : MonoBehaviour
    {
        public Text text;
        public RectTransform thisRect;
        public Image panelImage;
        public Image shadowImage;
        public CanvasGroup canvasGroup;
        private readonly float animSpeed = 6f;
        private Vector2 offPos;
        private Vector2 onPos;
        private int state;
        private Color tempColor;
        private Vector2 tempVec2;
        private Vector3 tempVec3;
        private float timeToWait;

        private void Start()
        {
            onPos = new Vector2(Screen.width / 2, Screen.height / 8);
            offPos = new Vector2(Screen.width / 2, Screen.height / 10);
            thisRect.position = offPos;

            timeToWait = ToastControl.toastDuration;
            text.text = ToastControl.toastText;
            panelImage.color = ToastControl.toastPanelColor;
            text.color = ToastControl.toastTextColor;
            text.fontSize = ToastControl.toastFontSize;

            transform.SetParent(ToastControl.parentCanvas.transform);
            transform.localScale = new Vector3(1, 1, 1);

            canvasGroup.alpha = 0;
            state = 1;
        }

        private void Update()
        {
            if (state == 1)
            {
                if (thisRect.position.y < onPos.y)
                {
                    tempVec2 = thisRect.position;
                    tempVec2.y = Mathf.Lerp(tempVec2.y, onPos.y * 1.01f, Time.deltaTime * animSpeed);
                    thisRect.position = tempVec2;
                    canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1.01f, Time.deltaTime * animSpeed);
                }
                else
                {
                    thisRect.position = onPos;
                    StartCoroutine(WaitTime());
                }
            }
            else if (state == 2)
            {
                if (thisRect.position.y > offPos.y)
                {
                    tempVec2 = thisRect.position;
                    tempVec2.y = Mathf.Lerp(tempVec2.y, offPos.y * 0.99f, Time.deltaTime * animSpeed);
                    thisRect.position = tempVec2;
                    canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, -0.01f, Time.deltaTime * animSpeed);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        private IEnumerator WaitTime()
        {
            yield return new WaitForSeconds(timeToWait);
            state = 2;
        }
    }
}