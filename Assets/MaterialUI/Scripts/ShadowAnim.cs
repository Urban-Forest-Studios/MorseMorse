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
using UnityEngine.UI;

namespace MaterialUI
{
    public class ShadowAnim : MonoBehaviour
    {
        public bool isOn;
        public bool anim;
        private Image[] shadows;
        private CanvasGroup thisGroup;

        private void Awake()
        {
            thisGroup = gameObject.GetComponent<CanvasGroup>();
            shadows = gameObject.GetComponentsInChildren<Image>();
        }

        private void Update()
        {
            if (anim)
            {
                if (isOn)
                {
                    if (thisGroup.alpha < 1f)
                    {
                        thisGroup.alpha = Mathf.Lerp(thisGroup.alpha, 1.1f, Time.deltaTime * 6);
                    }
                    else
                    {
                        thisGroup.alpha = 1f;
                        anim = false;
                    }
                }
                else
                {
                    if (thisGroup.alpha > 0f)
                    {
                        thisGroup.alpha = Mathf.Lerp(thisGroup.alpha, -0.1f, Time.deltaTime * 6);
                    }
                    else
                    {
                        thisGroup.alpha = 0f;
                        anim = false;
                        foreach (var shadow in shadows)
                            shadow.enabled = false;
                    }
                }
            }
        }

        public void SetShadow(bool set)
        {
            isOn = set;
            anim = true;
            foreach (var shadow in shadows)
                shadow.enabled = true;
        }
    }
}