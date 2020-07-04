using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace GentlesDebugTools.MFD
{
    class VehicleFunPage : MonoBehaviour
    {
        private DebugMFD mfd;

        private DebugMFDPage FunPage;
        private Dictionary<DebugMFDInfoTexts, string> funPageInfoTexts;
        private Dictionary<DebugMFDButtons, DebugMFDButton> funPageButtons;
        private Texture2D funPageBackground;

        private DebugMFDButton HomeButton;
        private DebugMFDButton vehicleFunPageButton;

        //Fun Page Buttons
        private DebugMFDButton IncreaseThrustButton;
        private DebugMFDButton DecreaseThrustButton;
        private DebugMFDButton UnlimitedCannonAmmoButton;
        private DebugMFDButton ToggleTimeSlowButton;

        private VehicleMaster master;
        private WeaponManager weapManager;
        private Gun cannon;

        private GameObject vehicle;
        private bool infAmmo = false;


        private float previousThrustMult;
        private int previousAmmo;
        private int previousMaxAmmo;

        private float previousTimeScale;


        private void Awake()
        {
            mfd = DebugMFD.instance;

            funPageBackground = DebugMFDUtilities.FileLoader.LoadImageFromFile(mfd.debugtoolsModDirectory + "Backgrounds/funpage.png");
            funPageButtons = new Dictionary<DebugMFDButtons, DebugMFDButton>(11);
            funPageInfoTexts = new Dictionary<DebugMFDInfoTexts, string>(10);

            HomeButton = new DebugMFDButton(DebugMFDButtons.TopMiddle, "HOME", mfd.GoHome);
            vehicleFunPageButton = new DebugMFDButton(DebugMFDButtons.Right2, "FUN", this.SwitchTo);

            previousTimeScale = Time.timeScale;

            SetupVehicle();
            InitButtons();
            InitTexts();
            InitPage();
        }

        private void SetupVehicle()
        {
            vehicle = VTOLAPI.GetPlayersVehicleGameObject();
            master = vehicle.GetComponent<VehicleMaster>();
            previousThrustMult = master.engines[0].abThrustMult;
            weapManager = vehicle.GetComponent<WeaponManager>();
            if (weapManager.equippedGun)
            {
                cannon = vehicle.GetComponentInChildren<Gun>();
            }
        }

        private void InitButtons()
        {
            //Initialization
            foreach (DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons)))
            {
                funPageButtons[item] = new DebugMFDButton(item, "-");
            }

            //Button creation
            IncreaseThrustButton = new DebugMFDButton(DebugMFDButtons.Left2, "Incr. Thrust", this.IncreaseThrust);
            DecreaseThrustButton = new DebugMFDButton(DebugMFDButtons.Left3, "Decr. Thrust", this.DecreaseThrust);
            UnlimitedCannonAmmoButton = new DebugMFDButton(DebugMFDButtons.Right4, "Inf. Ammo", this.ToggleInfiniteAmmo);
            ToggleTimeSlowButton = new DebugMFDButton(DebugMFDButtons.Left4, "Time Slow", this.ToggleTimeSlow);


            //Button assignment
            funPageButtons[HomeButton.ButtonPosition] = HomeButton;
            funPageButtons[IncreaseThrustButton.ButtonPosition] = IncreaseThrustButton;
            funPageButtons[DecreaseThrustButton.ButtonPosition] = DecreaseThrustButton;
            funPageButtons[UnlimitedCannonAmmoButton.ButtonPosition] = UnlimitedCannonAmmoButton;
            funPageButtons[ToggleTimeSlowButton.ButtonPosition] = ToggleTimeSlowButton;
        }

        private void InitTexts()
        {
            foreach (DebugMFDInfoTexts item in Enum.GetValues(typeof(DebugMFDInfoTexts)))
            {
                funPageInfoTexts[item] = "";
            }

            funPageInfoTexts[DebugMFDInfoTexts.Left1] = "Thrust Mult.";
            funPageInfoTexts[DebugMFDInfoTexts.Right1] = previousThrustMult.ToString();
            funPageInfoTexts[DebugMFDInfoTexts.Right3] = "Cannon Ammo";
        }

        private void InitPage()
        {
            FunPage = new DebugMFDPage("FUN PAGE", "Built-In", funPageButtons, funPageInfoTexts, funPageBackground);

            if (mfd != null)
            {
                mfd.home.ReplaceHomePageButton(vehicleFunPageButton);
                //If we are already the active page OR the active page is currently null
                if (mfd.activePage.Equals(this.FunPage) || !mfd.activePage.Equals(null))
                {
                    mfd.SetPage(FunPage);
                }
            }
        }

        public void IncreaseThrust()
        {
            for (int i = 0; i < master.engines.Length; i++)
            {
                master.engines[i].abThrustMult += (master.engines[i].abThrustMult * 0.25f);
                FunPage.InfoTexts[DebugMFDInfoTexts.Middle1] = master.engines[i].abThrustMult.ToString();
            }
            UpdateThisPage();
        }

        public void DecreaseThrust()
        {
            for (int i = 0; i < master.engines.Length; i++)
            {
                master.engines[i].abThrustMult -= (master.engines[i].abThrustMult * 0.25f);
                FunPage.InfoTexts[DebugMFDInfoTexts.Middle1] = master.engines[i].abThrustMult.ToString();
            }
            UpdateThisPage();
        }

        public void ToggleInfiniteAmmo()
        {
            if (cannon != null)
            {
                infAmmo = !infAmmo;
                if (infAmmo)
                {
                    previousAmmo = cannon.currentAmmo;
                    previousMaxAmmo = cannon.maxAmmo;

                    cannon.maxAmmo = 999999999;
                    cannon.currentAmmo = 999999999;
                    funPageInfoTexts[DebugMFDInfoTexts.Middle3] = "YES";
                }
                else
                {
                    cannon.maxAmmo = previousMaxAmmo;
                    cannon.currentAmmo = previousAmmo;
                    funPageInfoTexts[DebugMFDInfoTexts.Middle3] = "NO";
                }
                UpdateThisPage();
            }
        }

        public void ToggleTimeSlow()
        {
            if(Time.timeScale != 0.25f)
            {
                Time.timeScale = 0.25f;
                funPageInfoTexts[DebugMFDInfoTexts.Left3] = "TIME SLOWED TO 0.25";
                UpdateThisPage();
                StartCoroutine(SlowAudioSourcesInScene(false));
            }
            else
            {
                Time.timeScale = previousTimeScale;
                funPageInfoTexts[DebugMFDInfoTexts.Left3] = "TIME REGULAR";
                UpdateThisPage();
                StartCoroutine(SlowAudioSourcesInScene(true));
            }
        }

        private IEnumerator SlowAudioSourcesInScene(bool restore)
        {
            AudioSource[] allSources = FindObjectsOfType<AudioSource>();

            for(int i = 0; i < allSources.Length; i++)
            {
                if (!restore)
                {
                    allSources[i].pitch *= 0.25f;
                }
                else
                {
                    allSources[i].pitch *= 4f;
                }
                //yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        private void UpdateThisPage()
        {
            FunPage.Update(funPageButtons, funPageInfoTexts);
            mfd.UpdatePage();
        }

        //Switch to this page!
        public void SwitchTo()
        {
            if(mfd != null && !this.FunPage.Equals(null))
            {
                mfd.SetPage(FunPage);
            }
        }
    }
}
