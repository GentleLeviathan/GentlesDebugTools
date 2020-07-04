using UnityEngine;
using System.Collections.Generic;

namespace GentlesDebugTools.MFD
{
    class DebugLog : CustomMFDPage
    {
        private List<string> orderedMessages;
        private List<string> errorMessages;
        private List<string> warningMessages;
        private List<string> assertionMessages;

        private List<string> displayedMessages;

        private LogType chosenType;

        public override void Awake()
        {
            PageButtonName = "LOG";
            PageButtonPosition = DebugMFDButtons.TopRight;

            base.Awake();
        }

        public override void Start()
        {
            PageName = "DebugLog";
            ModName = "Built-In";
            PageBackground = mfd.defaultPageBackground;

            orderedMessages = new List<string>();
            errorMessages = new List<string>();
            warningMessages = new List<string>();
            assertionMessages = new List<string>();
            displayedMessages = orderedMessages;

            Application.logMessageReceived += logListener;

            base.Start();
        }

        private void logListener(string condition, string stackTrace, LogType type)
        {
            if (mfd.IsTextOverflowing(DebugMFDInfoTexts.FS1))
            {
                orderedMessages.Clear();
                assertionMessages.Clear();
                errorMessages.Clear();
                warningMessages.Clear();
            }
            switch (type)
            {
                case LogType.Assert:
                    assertionMessages.Add(condition + ": " + stackTrace);
                    break;
                case LogType.Error:
                    errorMessages.Add(condition + ": " + stackTrace);
                    break;
                case LogType.Warning:
                    warningMessages.Add(condition + ": " + stackTrace);
                    break;
            }
            orderedMessages.Add(condition + ": " + stackTrace + " - TYPE: " + type.ToString());
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            switch (chosenType)
            {
                case LogType.Assert:
                    displayedMessages = assertionMessages;
                    break;
                case LogType.Error:
                    displayedMessages = errorMessages;
                    break;
                case LogType.Warning:
                    displayedMessages = warningMessages;
                    break;
                case LogType.Log:
                    displayedMessages = orderedMessages;
                    break;
            }

            InfoTexts[DebugMFDInfoTexts.FS1] = "";
            for (int i = 0; i < displayedMessages.Count; i++)
            {
                InfoTexts[DebugMFDInfoTexts.FS1] += displayedMessages[i] + "\n";
            }
            UpdateThisPage();
        }

        public override void InitButtons()
        {
            base.InitButtons();

            PageButtons[DebugMFDButtons.Left1] = new DebugMFDButton(DebugMFDButtons.Left1, "SELFTEST", this.SelfTest);
            PageButtons[DebugMFDButtons.Right1] = new DebugMFDButton(DebugMFDButtons.Right1, "ERRORS", this.DisplayErrors);
            PageButtons[DebugMFDButtons.Right2] = new DebugMFDButton(DebugMFDButtons.Right2, "WARNINGS", this.DisplayWarnings);
            PageButtons[DebugMFDButtons.Right3] = new DebugMFDButton(DebugMFDButtons.Right3, "ASSERTIONS", this.DisplayAssertions);
            PageButtons[DebugMFDButtons.Right4] = new DebugMFDButton(DebugMFDButtons.Right4, "ALL", this.DisplayAll);
        }

        private void SelfTest()
        {
            Debug.LogAssertion("DebugMFD: DebugLog: SELF TEST - If you can read this, and it is formatted properly, self-test successful.");
        }

        public override void InitTexts()
        {
            base.InitTexts();
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

        //Switch to this page!
        public override void SwitchTo()
        {
            base.SwitchTo();
        }

        public override void LostFocus()
        {
            base.LostFocus();
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= logListener;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= logListener;
        }

        public void DisplayErrors()
        {
            chosenType = LogType.Error;
            UpdateDisplay();
        }

        public void DisplayWarnings()
        {
            chosenType = LogType.Warning;
            UpdateDisplay();
        }

        public void DisplayAssertions()
        {
            chosenType = LogType.Assert;
            UpdateDisplay();
        }

        public void DisplayAll()
        {
            chosenType = LogType.Log;
            UpdateDisplay();
        }
    }
}