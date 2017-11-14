﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Prototype.NetworkLobby
{
    public class LobbyTopPanel : MonoBehaviour
    {
        public bool isInGame = false;
		public bool isAllowEsc = true;
        protected bool isDisplayed = true;
        protected RawImage panelImage;
		public GameObject Background;

        void Start()
        {
			panelImage = GetComponent<RawImage>();
        }


        void Update()
        {
			Background.gameObject.SetActive (!isInGame);
            if (!isInGame)
                return;
			if (isAllowEsc && Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleVisibility(!isDisplayed);
            }

        }

       public void ToggleVisibility(bool visible)
        {
            isDisplayed = visible;
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(isDisplayed);
            }

            if (panelImage != null)
            {
                panelImage.enabled = isDisplayed;
            }
        }
    }
}