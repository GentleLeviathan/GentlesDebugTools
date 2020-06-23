using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace VTOLVRDebug
{
    class InGameLog : MonoBehaviour
    {
        private DebugMFD mfd;

        private DebugMFDPage LogPage;
        private Dictionary<DebugMFDInfoTexts, string> logPageInfoTexts;
        private Dictionary<DebugMFDButtons, DebugMFDButton> logPageButtons;
        private Texture2D logPageBackground;

        private DebugMFDButton HomeButton;
        private DebugMFDButton logPageButton;

        private List<string> orderedMessages;
        private int MaxCharacters = 1160;


        private void Awake()
        {
            mfd = DebugMFD.instance;

            logPageBackground = DebugMFDUtilities.FileLoader.LoadImageFromFile(mfd.debugtoolsModDirectory + "Backgrounds/default.png");
            logPageButtons = new Dictionary<DebugMFDButtons, DebugMFDButton>(11);
            logPageInfoTexts = new Dictionary<DebugMFDInfoTexts, string>(10);

            HomeButton = new DebugMFDButton(DebugMFDButtons.TopMiddle, "HOME", mfd.GoHome);
            logPageButton = new DebugMFDButton(DebugMFDButtons.TopRight, "LOG", this.SwitchTo);

            orderedMessages = new List<string>();

            InitButtons();
            InitPage();

            Application.logMessageReceived += logListener;
        }

        private void logListener(string condition, string stackTrace, LogType type)
        {
            orderedMessages.Add(condition + ": " + stackTrace + " - TYPE: " + type.ToString());
            StartCoroutine(OutOfRangeHandler());
        }

        private IEnumerator OutOfRangeHandler()
        {
            int totalNumbers = 0;
            for (int i = 0; i < orderedMessages.Count; i++)
            {
                foreach(char c in orderedMessages[i])
                {
                    totalNumbers++;
                }
                if (orderedMessages[i].Contains("\n"))
                {
                    totalNumbers += 58;
                }
                //yield return new WaitForSeconds(0.001f);
            }
            if(totalNumbers > MaxCharacters - 58)
            {
                //we need to shift the entire list up to get rid of the first entry and replace it with the second.
                Array temp = orderedMessages.ToArray();
                Array.Copy(temp, 1, temp, 0, orderedMessages.Count - 1);
                for(int i = 0; i < temp.Length; i++)
                {
                    orderedMessages[i] = (string)temp.GetValue(i);
                    //yield return new WaitForSeconds(0.001f);
                }
            }
            yield return new WaitForSeconds(0.001f);
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            logPageInfoTexts[DebugMFDInfoTexts.FS1] = "";
            for (int i = 0; i < orderedMessages.Count; i++)
            {
                logPageInfoTexts[DebugMFDInfoTexts.FS1] += orderedMessages[i];
            }
            UpdateThisPage();
        }

        private void InitButtons()
        {
            //Initialization
            foreach (DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons)))
            {
                logPageButtons[item] = new DebugMFDButton(item, "");
            }

            logPageButtons[DebugMFDButtons.Left1] = new DebugMFDButton(DebugMFDButtons.Left1, "SELFTEST", this.SelfTest);

            //Dictionary setup
            logPageButtons[HomeButton.ButtonPosition] = HomeButton;
        }

        private void SelfTest()
        {
            Debug.LogWarning("DebugTools: InGameLog: Log Page: SELF-TEST\n");
            Debug.LogWarning("DebugTools: If you can read this, and it is properly formatted, successful self-test.");
        }

        private void InitPage()
        {
            foreach (DebugMFDInfoTexts item in Enum.GetValues(typeof(DebugMFDInfoTexts)))
            {
                logPageInfoTexts[item] = "";
            }

            logPageInfoTexts[DebugMFDInfoTexts.FS1] = "LOG INITIALIZED.";


            LogPage = new DebugMFDPage("Log Page", "Built-In", logPageButtons, logPageInfoTexts, logPageBackground);

            if (mfd != null)
            {
                mfd.home.ReplaceHomePageButton(logPageButton);
                //If we are already the active page OR the active page is currently null
                if (mfd.activePage.Equals(this.LogPage) || !mfd.activePage.Equals(null))
                {
                    mfd.SetPage(LogPage);
                }
            }
        }

        private void UpdateThisPage()
        {
            LogPage.Update(logPageButtons, logPageInfoTexts);
            if (mfd.activePage.Equals(this.LogPage) || !mfd.activePage.Equals(null))
            {
                mfd.SetPage(LogPage);
            }
        }

        //Switch to this page!
        public void SwitchTo()
        {
            if (mfd != null && !this.LogPage.Equals(new DebugMFDPage()))
            {
                mfd.SetPage(LogPage);
            }
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= logListener;
        }
    }
}
