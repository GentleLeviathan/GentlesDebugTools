using UnityEngine;
using System.Collections.Generic;
using System;

namespace GentlesDebugTools
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

        public string PageButtonName = "Empty Example Page";
        public DebugMFDButton homeButton { get => new DebugMFDButton(DebugMFDButtons.TopMiddle, "HOME", mfd.GoHome); }
        public DebugMFDButton thisPageButton { get => new DebugMFDButton(DebugMFDButtons.TopRight, PageButtonName, this.SwitchTo); }

        public string PageName = "EXAMPLE";
        public string ModName = "DebugMFD";

        public virtual void Awake()
        {
            //Get the DebugMFD itself. If it is null, log the error and destroy this page to avoid further exceptions.
            mfd = DebugMFD.instance ? DebugMFD.instance : null;
            if (!DebugSet.instance.DebugMFD_Setup_Finished || mfd == null) { Debug.LogError(ModName + ": DebugMFD instance was null! DebugTools mod was not loaded/initialized first, or there was an issue with the DebugMFD's creation. Please check your player log."); Destroy(this); }

            PageButtons = new Dictionary<DebugMFDButtons, DebugMFDButton>(11);
            InfoTexts = new Dictionary<DebugMFDInfoTexts, string>(10);
        }

        public virtual void Start()
        {
            _thisPage = new DebugMFDPage(PageName, ModName, PageButtons, InfoTexts, PageBackground, LostFocus);

            //Init methods.
            InitButtons();
            InitTexts();
            InitPage();
        }

        public virtual void InitButtons()
        {
            //This initializes the DebugMFDButtons at each position on the page.
            foreach (DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons)))
            {
                PageButtons[item] = new DebugMFDButton(item, "-");
            }

            //Assign the REQUIRED Home button to be able to get back to the DebugMFD's Home page.
            //Don't forget to do this!
            PageButtons[DebugMFDButtons.TopMiddle] = homeButton;
        }

        public virtual void InitTexts()
        {
            //This initializes the DebugMFDInfoTexts at each position on the page.
            foreach (DebugMFDInfoTexts item in Enum.GetValues(typeof(DebugMFDInfoTexts)))
            {
                //Place dashes in the info texts because they are empty!
                InfoTexts[item] = "-";
            }
        }

        public virtual void InitPage()
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

        public virtual void UpdateThisPage()
        {
            //For updating the page itself, and pushing the changes to the MFD.
            _thisPage.Update(PageButtons, InfoTexts);
            mfd.UpdatePage();
        }

        //Switch to this page!
        public virtual void SwitchTo()
        {
            if (!this._thisPage.Equals(null))
            {
                mfd.SetPage(_thisPage);
            }
        }

        //Called when this page loses focus (the DebugMFD displays a different page).
        //For this to work, you must pass it as the final argument in your DebugMFDPage constructor.
        //Can be omitted if this functionality is not required.
        public virtual void LostFocus()
        {

        }
    }
}
