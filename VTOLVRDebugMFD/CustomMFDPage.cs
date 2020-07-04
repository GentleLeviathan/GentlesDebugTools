using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace GentlesDebugTools.MFD
{
    /// <summary>
    /// CustomMFDPage is an interface for custom DebugMFD pages. Inherit this interface after MonoBehavior.
    /// Please look at/copy the empty example page to make your custom page, as the required dictionaries cannot be declared in an interface.
    /// </summary>
    public interface CustomMFDPageStruct
    {
        /// <summary>
        /// Name of the DebugMFDButton that will be put on the Home page, used in thisPageButton get;
        /// </summary>
        string PageButtonName { get; }
        /// <summary>
        /// DebugMFDButton used to go back to Home page.
        /// </summary>
        DebugMFDButton homeButton { get; }
        /// <summary>
        /// DebugMFDButton used on the Home page to get to this page.
        /// </summary>
        DebugMFDButton thisPageButton { get; }
        /// <summary>
        /// Name of the page, will be displayed on the page at the bottom. Optional.
        /// </summary>
        string PageName { get; }
        /// <summary>
        /// Name of the mod that is creating this page, will be displayed on the page at the bottom. Optional.
        /// </summary>
        string ModName { get; }
        /// <summary>
        /// DebugMFDPage that will be passed to the MFD.
        /// </summary>
        DebugMFDPage thisPage { get; }
        /// <summary>
        /// Setup the button dictionaries used on this page in this method.
        /// Remember to setup your buttons and texts before your DebugMFDPage.
        /// </summary>
        void InitButtons();
        /// <summary>
        /// Setup the text dictionaries used on this page in this method.
        /// Remember to setup your buttons and texts before your DebugMFDPage.
        /// </summary>
        void InitTexts();
        /// <summary>
        /// Setup the page and text dictionaries in this method.
        /// </summary>
        void InitPage();
        /// <summary>
        /// Implement the code required to switch to this page in this method.
        /// </summary>
        void SwitchTo();
        /// <summary>
        /// Called when this page loses focus and another page becomes the activePage. Optional.
        /// Remember to pass this into your DebugMFDPage constructor if you need this functionality.
        /// </summary>
        void LostFocus();
    }

    public abstract class CustomMFDPage : MonoBehaviour
    {
        //The DebugMFD instance
        public DebugMFD mfd;

        public Dictionary<DebugMFDInfoTexts, string> InfoTexts;
        public Dictionary<DebugMFDButtons, DebugMFDButton> PageButtons;
        public Texture2D PageBackground;
        public DebugMFDPage _thisPage;
        public bool PageExecutionPaused = false;

        public string PageButtonName = "Empty Example Page";
        public DebugMFDButtons PageButtonPosition;
        public DebugMFDButton thisPageButton;

        public string PageName = "EXAMPLE";
        public string ModName = "DebugMFD";

        public virtual void Awake()
        {
            //Get the DebugMFD itself. If it is null, log the error and destroy this page to avoid further exceptions.
            mfd = DebugMFD.instance ? DebugMFD.instance : null;
            if (!DebugSet._instance.DebugMFD_Setup_Finished || mfd == null) { Debug.LogError(ModName + ": DebugMFD instance was null! DebugTools mod was not loaded/initialized first, or there was an issue with the DebugMFD's creation. Please check your player log."); Destroy(this); }

            PageButtons = new Dictionary<DebugMFDButtons, DebugMFDButton>(11);
            InfoTexts = new Dictionary<DebugMFDInfoTexts, string>(10);
            thisPageButton = new DebugMFDButton(PageButtonPosition, PageButtonName, this.SwitchTo);
        }

        public virtual void Start()
        {
            //Init methods.
            InitButtons();
            InitTexts();
            InitPage();
        }

        private void Update()
        {
            if (!PageExecutionPaused)
            {
                MFDUpdate();
            }
        }

        /// <summary>
        /// Called every frame if the page is not paused. Use as substitute for Update() if pause functionality is required. Recommended.
        /// </summary>
        public virtual void MFDUpdate()
        {

        }

        /// <summary>
        /// Initializes the required Page Buttons dictionary and sets them to empty buttons with dashes by default.
        /// </summary>
        public virtual void InitButtons()
        {
            //This initializes the DebugMFDButtons at each position on the page.
            foreach (DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons)))
            {
                PageButtons[item] = new DebugMFDButton(item, "-");
            }

            //Assign the REQUIRED Home button to be able to get back to the DebugMFD's Home page.
            //Don't forget to do this!
            PageButtons[DebugMFDButtons.TopMiddle] = mfd.homeButton;
        }


        /// <summary>
        /// Initializes the required Info Text dictionary and sets them to strings with dashes by default.
        /// </summary>
        public virtual void InitTexts()
        {
            foreach (DebugMFDInfoTexts item in Enum.GetValues(typeof(DebugMFDInfoTexts)))
            {
                InfoTexts[item] = "-";
            }
        }

        /// <summary>
        /// Initializes the page and replaces a button on the home page with a button to get to this page.
        /// </summary>
        public virtual void InitPage()
        {
            _thisPage = new DebugMFDPage(PageName, ModName, PageButtons, InfoTexts, PageBackground, LostFocus);
            //Here we replace a button on the DebugMFD's Home page to allow us to get to our custom page from the home screen.
            mfd.home.ReplaceHomePageButton(thisPageButton);
        }

        /// <summary>
        /// Call when you wish to upate the page and push the changes to the MFD. Can be done every frame if necessary.
        /// </summary>
        public virtual void UpdateThisPage()
        {
            _thisPage.Update(PageButtons, InfoTexts);
            mfd.UpdatePage();
        }

        /// <summary>
        /// This is called when "thisPageButton" on the home page is pressed.
        /// Switches to this page.
        /// </summary>
        public virtual void SwitchTo()
        {
            if (this._thisPage.Equals(null)) { Debug.LogError(this.PageName + ": _thisPage was not initialized. Can not switch to this page. Did you modify or make an error in the DebugMFDPage constructor?"); return; }
            mfd.SetPage(_thisPage);
            UnpausePageExecution();
        }

        /// <summary>
        /// Called when this page loses focus (the DebugMFD displays a different page).
        /// Logic can be omitted if this functionality is not required.
        /// </summary>
        public virtual void LostFocus()
        {
            PausePageExecution();
        }

        /// <summary>
        /// Pauses execution of focus-oriented logic. Used alongside MFDUpdate().
        /// </summary>
        public void PausePageExecution()
        {
            PageExecutionPaused = true;
        }
        /// <summary>
        /// Allows execution of focus-oriented logic. Used alongside MFDUpdate().
        /// </summary>
        public void UnpausePageExecution()
        {
            PageExecutionPaused = false;
        }
    }
}
