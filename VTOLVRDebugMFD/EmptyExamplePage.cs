using UnityEngine;
using System.Collections.Generic;
using System;

namespace GentlesDebugTools
{
    /// <summary>
    /// This is an empty example custom DebugMFD page. To make custom pages, copy this page into a class in your own mod solution.
    /// To add this page to the DebugMFD, simply add the MonoBehaviour to a GameObject (any will do) in the scene AFTER the DebugMFD has been setup.
    /// If you add it before the DebugMFD is completely setup, it will destroy itself.
    /// For safe handling of this case, subscribe to the event: 'DebugSet.instance._finished' in "Start" in your mod handler, and add your page as a component when it is fired.
    /// This will happen roughly 5-6 seconds after scene load.
    /// </summary>
    class EmptyExamplePage : MonoBehaviour, CustomMFDPage
    {
        //The DebugMFD instance
        private DebugMFD mfd;

        public Dictionary<DebugMFDInfoTexts, string> InfoTexts;
        public Dictionary<DebugMFDButtons, DebugMFDButton> PageButtons;
        private Texture2D PageBackground;
        private DebugMFDPage _thisPage;

        public string PageButtonName => "Empty Example Page";
        public DebugMFDButton homeButton { get => new DebugMFDButton(DebugMFDButtons.TopMiddle, "HOME", mfd.GoHome); }
        public DebugMFDButton thisPageButton { get => new DebugMFDButton(DebugMFDButtons.TopRight, PageButtonName, this.SwitchTo); }

        public string PageName => "EXAMPLE";
        public string ModName => "DebugMFD";
        public DebugMFDPage thisPage { get => new DebugMFDPage(PageName, ModName, PageButtons, InfoTexts, LostFocus); set => new DebugMFDPage(_thisPage.PageName, _thisPage.ModName, _thisPage.PageButtons, _thisPage.InfoTexts, LostFocus); }

        private void Awake()
        {
            //Get the DebugMFD itself. If it is null, log the error and destroy this page to avoid further exceptions.
            mfd = DebugMFD.instance ? DebugMFD.instance : null;
            if (!DebugSet.instance.DebugMFD_Setup_Finished || mfd == null) { Debug.LogError(ModName + ": DebugMFD instance was null! DebugTools mod was not loaded/initialized first, or there was an issue with the DebugMFD's creation. Please check your player log."); Destroy(this); }

            //Page 'wallpaper'. Can be loaded with the included synchronous static FileLoader utility, or via another method.
            PageBackground = DebugMFDUtilities.FileLoader.LoadImageFromFile(mfd.debugtoolsModDirectory + "Backgrounds/default.png");

            //REQUIRED DICTIONARIES! Don't omit these!
            PageButtons = new Dictionary<DebugMFDButtons, DebugMFDButton>(11);
            InfoTexts = new Dictionary<DebugMFDInfoTexts, string>(10);

            //Here we are using the 'Copy' method on DebugMFDPage.
            //If you do not desire to use optional parameters (such as the PageBackground), you can simply do this as:  _thisPage = thisPage;
            //However, by default 'thisPage' returns a DebugMFDPage with no page background, so you must use the copy method to insert one.
            _thisPage = thisPage.Copy(PageBackground);

            //Init methods.
            InitButtons();
            InitTexts();
            InitPage();
        }

        public void InitButtons()
        {
            //This initializes the DebugMFDButtons at each position on the page.
            foreach (DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons)))
            {
                PageButtons[item] = new DebugMFDButton(item, "-");
            }

            //Here you can change the values of buttons at a specified position, or simply replace them with a new DebugMFDButton.
            //In this example, we setup a button to call our "MyCustomFunction" method.
            PageButtons[DebugMFDButtons.Left1] = new DebugMFDButton(DebugMFDButtons.Left1, "Custom Function", MyCustomFunction);

            //Assign the REQUIRED Home button to be able to get back to the DebugMFD's Home page.
            //Don't forget to do this!
            PageButtons[DebugMFDButtons.TopMiddle] = homeButton;
        }

        public void InitTexts()
        {
            //This initializes the DebugMFDInfoTexts at each position on the page.
            foreach (DebugMFDInfoTexts item in Enum.GetValues(typeof(DebugMFDInfoTexts)))
            {
                //Place dashes in the info texts because they are empty!
                InfoTexts[item] = "-";
            }

            //Here you can change the values of info texts at a specified position, or simply replace them with a new string.
            //In this example, we just make one of them say "Example"
            InfoTexts[DebugMFDInfoTexts.Middle3] = "Example";
        }

        public void InitPage()
        {
            //Here we replace a button on the DebugMFD's Home page to allow us to get to our custom page from the home screen.
            mfd.home.ReplaceHomePageButton(thisPageButton);

            //Here we set our page as the active page upon being initialized. You can omit this if you do not desire this functionality.
            //If we are already the active page OR the active page is currently null
            if (mfd.activePage.Equals(this._thisPage) || !mfd.activePage.Equals(null))
            {
                mfd.SetPage(_thisPage);
            }
        }

        private void UpdateThisPage()
        {
            //For updating the page itself, and pushing the changes to the MFD.
            thisPage.Update(PageButtons, InfoTexts);
            mfd.UpdatePage();
        }

        //Switch to this page!
        public void SwitchTo()
        {
            if (!this.thisPage.Equals(null))
            {
                mfd.SetPage(_thisPage);
            }
        }

        //Called when this page loses focus (the DebugMFD displays a different page).
        //For this to work, you must pass it as the final argument in your DebugMFDPage constructor.
        //Can be omitted if this functionality is not required.
        public void LostFocus()
        {

        }






        //Now we get to do our custom stuff!

        //Here is a simple example method that will be called when a button ('Left1', Top Left) is pressed.
        //You could obviously use it for anything you wish. Switching a bool, instantiating an object, adding components, whatever you like.
        //In this case, we'll just toggle one of the info texts to indicate that we pressed it!
        private void MyCustomFunction()
        {
            if(InfoTexts[DebugMFDInfoTexts.Left1] == "-")
            {
                InfoTexts[DebugMFDInfoTexts.Left1] = "Pressed!";
            }
            else
            {
                InfoTexts[DebugMFDInfoTexts.Left1] = "-";
            }
            UpdateThisPage();
        }
    }
}

