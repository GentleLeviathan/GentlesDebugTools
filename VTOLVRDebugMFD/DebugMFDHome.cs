using UnityEngine;
using System.Collections.Generic;
using System;

namespace GentlesDebugTools
{
    public class DebugMFDHome : MonoBehaviour
    {
        private DebugMFD mfd;

        private DebugMFDPage HomePage;
        private Dictionary<DebugMFDInfoTexts, string> homeInfoTexts;
        private Dictionary<DebugMFDButtons, DebugMFDButton> homeButtons;
        private Texture2D homePageBackground;

        private void Awake()
        {
            mfd = DebugMFD.instance;
            mfd.home = this;

            homePageBackground = DebugMFDUtilities.FileLoader.LoadImageFromFile(mfd.debugtoolsModDirectory + "Backgrounds/home.png");
            homeButtons = new Dictionary<DebugMFDButtons, DebugMFDButton>(11);
            homeInfoTexts = new Dictionary<DebugMFDInfoTexts, string>(10);

            InitButtons();
            InitPage();
        }

        private void InitButtons()
        {
            //InitialSetup
            foreach (DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons)))
            {
                homeButtons[item] = new DebugMFDButton(item, "-");
            }
        }

        private void InitPage()
        {
            foreach (DebugMFDInfoTexts item in Enum.GetValues(typeof(DebugMFDInfoTexts)))
            {
                homeInfoTexts[item] = "";
            }

            HomePage = new DebugMFDPage("Home", "DebugMFD", homeButtons, homeInfoTexts, homePageBackground);

            if (mfd != null)
            {
                //If we are already the active page OR the active page is currently null
                if(mfd.activePage.Equals(this.HomePage) || !mfd.activePage.Equals(null))
                {
                    mfd.SetPage(HomePage);
                }
            }
        }


        //Public methods for custom pages

        public void GoHome()
        {
            if (mfd != null)
            {
                //If we are already the active page OR the active page is currently null
                if (mfd.activePage.Equals(this.HomePage) || !mfd.activePage.Equals(null))
                {
                    mfd.SetPage(HomePage);
                }
            }
        }

        /// <summary>
        /// Replaces a button on the Home Page with a custom one for opening a custom DebugMFDPage.
        /// </summary>
        /// <param name="button"></param>
        public void ReplaceHomePageButton(DebugMFDButton button)
        {
            //If we don't already have the same button somewhere
            if (!homeButtons.ContainsValue(button))
            {
                homeButtons[button.ButtonPosition] = button;
            }
        }
    }
}
