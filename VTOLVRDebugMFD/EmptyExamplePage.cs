namespace GentlesDebugTools.MFD
{
    /// <summary>
    /// This is an empty example custom DebugMFD page. To make custom pages, copy this code into a class in your own mod solution and reference this mod.
    /// To add this page to the DebugMFD, simply add it as a component to a GameObject (any will do) in the scene AFTER the DebugMFD has been setup.
    /// If you add it before the DebugMFD is completely setup, it will destroy itself.
    /// For safe handling of this case, subscribe to the event: 'DebugSet._instance.finished' in "Start" in your mod handler, and add your page as a component when it is fired.
    /// Note: For now, this will happen roughly 5-6 seconds after scene load.
    /// </summary>
    class EmptyExamplePage : CustomMFDPage
    {
        public override void Awake()
        {
            //Here we set what we want the button to get to this page will be named, and where it will be on the Home screen.
            //Note: Button Position "TopMiddle" is always reserved for the home button, INCLUDING on the home page.
            //If you attempt to assign a button to the "TopMiddle" position, it will be replaced by the MFD.
            PageButtonName = "EXAMPLE";
            PageButtonPosition = DebugMFDButtons.Right1;

            base.Awake();
        }

        public override void Start()
        {
            //Here you can set the PageBackground (the page 'wallpaper') to a Texture2D. By default, it is set to the default background.
            //You can load a custom image using the synchronous or asynchronous static DebugMFDUtilities.FileLoader class, or via another method.
            //Built in backgrounds are at path:     DebugMFD.instance.debugtoolsModDirectory + "Backgrounds/"
            PageBackground = mfd.defaultPageBackground;

            base.Start();
        }

        public override void MFDUpdate()
        {
            base.MFDUpdate();

            //Here is where you would execute logic that is called every frame, but subject to being paused when focus is lost.
            //If this is a desired functionality, simple use in place of Update().
        }

        public override void InitButtons()
        {
            base.InitButtons();

            //Here you can change the values of buttons at a specified position, or simply replace them with a new DebugMFDButton.
            //In this example, we setup a button to call our "MyCustomFunction" method.
            PageButtons[DebugMFDButtons.Left1] = new DebugMFDButton(DebugMFDButtons.Left1, "Custom Function", MyCustomFunction);
            //Note: Button Position "TopMiddle" is always reserved for the home button, INCLUDING on the home page.
            //If you attempt to assign a button to the "TopMiddle" position, it will be replaced by the MFD.
        }

        public override void InitTexts()
        {
            base.InitTexts();

            //Here you can change the values of info texts at a specified position, or simply replace them with a new string.
            //In this example, we just make one of them say "Example"
            InfoTexts[DebugMFDInfoTexts.Middle3] = "Example";
        }

        public override void InitPage()
        {
            base.InitPage();
        }

        //For updating the page itself, and pushing the changes to the MFD.
        public override void UpdateThisPage()
        {
            base.UpdateThisPage();
        }

        //Called when this page regains focus (the DebugMFD displays this page).
        //Can be omitted if this functionality is not required.
        public override void SwitchTo()
        {
            base.SwitchTo();
        }

        //Called when this page loses focus (the DebugMFD displays a different page).
        //Can be omitted if this functionality is not required.
        public override void LostFocus()
        {
            base.LostFocus();
        }

        //Now we get to do our custom stuff!

        //Here is a simple example method that will be called when a button ('Left1', Top Left) is pressed.
        //You could obviously use it for anything you wish. Switching a bool, instantiating an object, adding components, whatever you like.
        //In this case, we'll just toggle one of the info texts to indicate that we pressed it!
        private void MyCustomFunction()
        {
            if (InfoTexts[DebugMFDInfoTexts.Left1] == "-")
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

