using UnityEngine;
using System.Collections.Generic;
using System;

namespace VTOLVRDebug
{
    class KeepTrack : MonoBehaviour
    {
        private DebugMFD mfd;
        private float Velocity;
        private Vector3 VelocityVector3;

        private Vector3 posLastFrame;

        private DebugMFDPage page;
        private Dictionary<DebugMFDInfoTexts, string> infos;
        private Dictionary<DebugMFDButtons, DebugMFDButton> buttons;

        private DebugMFDButton thisPageButton;

        private void Awake()
        {
            mfd = DebugMFD.instance;
            infos = new Dictionary<DebugMFDInfoTexts, string>(10);
            buttons = new Dictionary<DebugMFDButtons, DebugMFDButton>(11);
            foreach(DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons))){
                buttons[item] = new DebugMFDButton(item, "-");
            }
            buttons[DebugMFDButtons.TopMiddle] = new DebugMFDButton(DebugMFDButtons.TopMiddle, "HOME", mfd.GoHome);
            posLastFrame = transform.position;

            InitPage();
        }

        private void InitPage()
        {
            //Set some static text so we dont have to ask Mason what the numbers mean
            infos[DebugMFDInfoTexts.Left1] = "POSITION:";
            infos[DebugMFDInfoTexts.Left2] = "ROTATION:";
            infos[DebugMFDInfoTexts.Left3] = "VELOCITY:";

            //Set the initial texts so that we don't have the compiler complaining at us
            infos[DebugMFDInfoTexts.Middle1] = "Processing...";
            infos[DebugMFDInfoTexts.Middle2] = "Processing...";
            infos[DebugMFDInfoTexts.Middle3] = "Processing...";
            infos[DebugMFDInfoTexts.Right1] = "";
            infos[DebugMFDInfoTexts.Right2] = "Processing...";
            infos[DebugMFDInfoTexts.Right3] = "Processing...";

            page = new DebugMFDPage("KeepTrack", "Built-In", buttons, infos);

            thisPageButton = new DebugMFDButton(DebugMFDButtons.Right4, "KeepTrack", this.SwitchTo);

            mfd.home.ReplaceHomePageButton(thisPageButton);
        }

        public void SwitchTo()
        {
            mfd.SetPage(page);
        }

        private void FixedUpdate()
        {
            //Make sure we are the page that is currently selected before trying to render to the MFD
            if(mfd.activePage.Equals(page))
            {
                //Displaying some simple transform values for sake of demonstration
                VelocityVector3 = transform.position - posLastFrame;
                Velocity = VelocityVector3.magnitude;

                infos[DebugMFDInfoTexts.Middle1] = transform.position.ToString("F1");
                infos[DebugMFDInfoTexts.Middle2] = transform.rotation.eulerAngles.ToString("F1");
                infos[DebugMFDInfoTexts.Middle3] = VelocityVector3.ToString("F1");

                infos[DebugMFDInfoTexts.Right1] = "NsaD: " + Vector3.SignedAngle(Vector3.forward, -transform.forward, Vector3.up);
                infos[DebugMFDInfoTexts.Right2] = "NaD: " + Vector3.Angle(Vector3.forward, -transform.forward);
                infos[DebugMFDInfoTexts.Right3] = Velocity.ToString("0.0") + " m/s";

                posLastFrame = transform.position;

                page.InfoTexts = infos;
                page.Update(buttons, infos);

                if (mfd != null)
                {
                    mfd.SetPage(page);
                }
            }
        }
    }
}
