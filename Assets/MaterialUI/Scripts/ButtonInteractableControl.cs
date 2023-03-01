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
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasGroup))]
    public class ButtonInteractableControl : MonoBehaviour
    {
        [SerializeField] private CanvasGroup shadows;
        private Button button;
        private CanvasGroup canvasGroup;

        private bool lastInteractableState;

        private void Update()
        {
            if (lastInteractableState != button.interactable)
            {
                lastInteractableState = button.interactable;

                if (lastInteractableState)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.blocksRaycasts = true;

                    if (shadows)
                        shadows.alpha = 1f;
                }
                else
                {
                    canvasGroup.alpha = 0.5f;
                    canvasGroup.blocksRaycasts = false;

                    if (shadows)
                        shadows.alpha = 0f;
                }
            }
        }

        private void OnEnable()
        {
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            button = gameObject.GetComponent<Button>();
        }
    }
}