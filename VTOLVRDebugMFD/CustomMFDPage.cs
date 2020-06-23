namespace VTOLVRDebug
{
    /// <summary>
    /// CustomMFDPage is an interface for custom DebugMFD pages. Inherit this interface after MonoBehavior.
    /// Please look at/copy the empty example page to make your custom page, as the required dictionaries cannot be declared in an interface.
    /// </summary>
    public interface CustomMFDPage
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
}
