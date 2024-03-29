﻿//  Copyright 2014 Invex Games http://invexgames.com
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
    public class NavDrawerConfig : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Image backgroundImage;
        public Image ShadowImage;
        public bool darkenBackground = true;
        public bool tapBackgroundToClose = true;
        public float animationDuration = 0.5f;
        private float animDeltaTime;
        private float animStartTime;
        private CanvasGroup backgroundCanvasGroup;
        private RectTransform backgroundRectTransform;
        private float currentBackgroundAlpha;

        private Vector2 currentPos;
        private float currentShadowAlpha;
        private float maxPosition;
        private float minPosition;
        private float positionCompensation;
        private CanvasGroup shadowCanvasGroup;

        private byte state;

        private Vector2 tempVector2;
        private RectTransform thisRectTransform;

        private void Awake()
        {
            thisRectTransform = gameObject.GetComponent<RectTransform>();
            backgroundRectTransform = backgroundImage.GetComponent<RectTransform>();
            backgroundCanvasGroup = backgroundImage.GetComponent<CanvasGroup>();
            shadowCanvasGroup = ShadowImage.GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            maxPosition = thisRectTransform.rect.width / 2;
            minPosition = -maxPosition;

            backgroundRectTransform.sizeDelta = new Vector2(Screen.width, backgroundRectTransform.sizeDelta.y);
        }

        private void Update()
        {
            if (state == 1)
            {
                animDeltaTime = Time.realtimeSinceStartup - animStartTime;

                if (animDeltaTime <= animationDuration)
                {
                    thisRectTransform.anchoredPosition = Anim.Quint.Out(currentPos,
                        new Vector2(maxPosition, thisRectTransform.anchoredPosition.y), animDeltaTime,
                        animationDuration);

                    backgroundCanvasGroup.alpha =
                        Anim.Quint.Out(currentBackgroundAlpha, 1f, animDeltaTime, animationDuration);
                    shadowCanvasGroup.alpha =
                        Anim.Quint.In(currentShadowAlpha, 1f, animDeltaTime, animationDuration / 2f);
                }
                else
                {
                    thisRectTransform.anchoredPosition = new Vector2(maxPosition, thisRectTransform.anchoredPosition.y);
                    backgroundCanvasGroup.alpha = 1f;
                    state = 0;
                }
            }
            else if (state == 2)
            {
                animDeltaTime = Time.realtimeSinceStartup - animStartTime;

                if (animDeltaTime <= animationDuration)
                {
                    thisRectTransform.anchoredPosition = Anim.Quint.Out(currentPos,
                        new Vector2(minPosition, thisRectTransform.anchoredPosition.y), animDeltaTime,
                        animationDuration);

                    backgroundCanvasGroup.alpha =
                        Anim.Quint.Out(currentBackgroundAlpha, 0f, animDeltaTime, animationDuration);
                    shadowCanvasGroup.alpha = Anim.Quint.In(currentShadowAlpha, 0f, animDeltaTime, animationDuration);
                }
                else
                {
                    thisRectTransform.anchoredPosition = new Vector2(minPosition, thisRectTransform.anchoredPosition.y);
                    backgroundCanvasGroup.alpha = 0f;
                    state = 0;
                }
            }

            thisRectTransform.anchoredPosition =
                new Vector2(Mathf.Clamp(thisRectTransform.anchoredPosition.x, minPosition, maxPosition),
                    thisRectTransform.anchoredPosition.y);
        }

        public void OnBeginDrag(PointerEventData data)
        {
            state = 0;
        }

        public void OnDrag(PointerEventData data)
        {
            tempVector2 = thisRectTransform.position;
            tempVector2.x += data.delta.x;

            thisRectTransform.position = tempVector2;

            backgroundCanvasGroup.alpha =
                1 - (maxPosition - thisRectTransform.anchoredPosition.x) / (maxPosition - minPosition);
            shadowCanvasGroup.alpha = 1 - (maxPosition - thisRectTransform.anchoredPosition.x) /
                ((maxPosition - minPosition) * 2);
        }

        public void OnEndDrag(PointerEventData data)
        {
            if (Mathf.Abs(data.delta.x) >= 0.5f)
            {
                if (data.delta.x > 0.5f)
                    Open();
                else
                    Close();
            }
            else
            {
                if (thisRectTransform.anchoredPosition.x - minPosition >
                    maxPosition - thisRectTransform.anchoredPosition.x)
                    Open();
                else
                    Close();
            }
        }

        public void BackgroundTap()
        {
            if (tapBackgroundToClose)
                Close();
        }

        public void Open()
        {
            currentPos = thisRectTransform.anchoredPosition;
            currentBackgroundAlpha = backgroundCanvasGroup.alpha;
            currentShadowAlpha = shadowCanvasGroup.alpha;
            backgroundCanvasGroup.blocksRaycasts = true;
            animStartTime = Time.realtimeSinceStartup;
            state = 1;
        }

        public void Close()
        {
            currentPos = thisRectTransform.anchoredPosition;
            currentBackgroundAlpha = backgroundCanvasGroup.alpha;
            currentShadowAlpha = shadowCanvasGroup.alpha;
            backgroundCanvasGroup.blocksRaycasts = false;
            animStartTime = Time.realtimeSinceStartup;
            state = 2;
        }
    }
}