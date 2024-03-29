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

namespace MaterialUI
{
    public class Toaster : MonoBehaviour
    {
        public string text = "This is a toast";
        public float duration = 1.5f;
        public Color panelColor = new(1f, 1f, 1f);
        public Color textColor = new(0.15f, 0.15f, 0.15f);
        public int fontSize = 16;

        private void Start()
        {
            ToastControl.InitToastSystem(gameObject.GetComponentInParent<Canvas>());
        }

        public void PopupToast()
        {
            ToastControl.MakeToast(text, duration, panelColor, textColor, fontSize);
        }
    }
}