using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;

namespace GentlesDebugTools
{
    public class DebugMFD : MonoBehaviour
    {
        //Public vars
        public static DebugMFD instance;

        public Animator DebugMFDAnimator;
        public Material ScreenBackground;

        //DebugMFD Pages + Buttons
        public List<DebugMFDPage> DebugMFDPages;
        public Dictionary<DebugMFDPage, List<DebugMFDButton>> PageButtons;
        //private Dictionary<DebugMFDButtons, NoClickButton> noClickButtons;
        private Dictionary<DebugMFDButtons, VRInteractable> interactableButtons;

        public DebugMFDPage activePage;
        public List<DebugMFDButton> activeButtons;

        public DebugMFDHome home;

        //Private vars
        private DebugSet debugset;
        private GameObject debugMFDModel;
        private GameObject model;

        private bool textsSetup = false;
        private bool interactablesSetup = false;
        public Texture2D defaultPageBackground;
        public string debugtoolsModDirectory;

        //Texts
        private Dictionary<DebugMFDButtons, TextMeshPro> ButtonDescriptions;
        private Dictionary<DebugMFDInfoTexts, TextMeshPro> InfoTexts;
        private TextMeshPro PageName;
        private TextMeshPro ModName;

        //VRInteractable stuffs
        PoseBounds mfdPoseBounds;

        private void Awake()
        {
            if(instance != null) { Debug.LogError("DebugMFD: Instance was not null! Destroying this instance.."); Destroy(this); }
            instance = this;

            debugtoolsModDirectory = Directory.GetCurrentDirectory() + @"\VTOLVR_ModLoader\mods\Gentle's Debug Tools\";
            debugset = DebugSet.instance;

            defaultPageBackground = DebugMFDUtilities.FileLoader.LoadImageFromFile(debugtoolsModDirectory + "Backgrounds/default.png");

            InitCollections();
            InitDebugMFD();
            textsSetup = SetupTexts();
            SetupInteractableButtons();
            GoHome();
            MFDFinished();
            SetupBuiltInPages();
            GoHome();
        }

        private void InitCollections()
        {
            DebugMFDPages = new List<DebugMFDPage>();
            PageButtons = new Dictionary<DebugMFDPage, List<DebugMFDButton>>();
            interactableButtons = new Dictionary<DebugMFDButtons, VRInteractable>(11);
            //11 buttons on the custom MFD
            ButtonDescriptions = new Dictionary<DebugMFDButtons, TextMeshPro>(11);
            //10 default infotexts (FS1 is full-screen)
            InfoTexts = new Dictionary<DebugMFDInfoTexts, TextMeshPro>(10);
        }

        private void InitDebugMFD()
        {
            GameObject DefaultCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            DefaultCube.transform.position = Vector3.up * -500f;

            debugMFDModel = new GameObject("DebugMFD_Instance");
            debugMFDModel.transform.parent = this.transform;
            debugMFDModel.transform.localPosition = new Vector3(0, 0.5817f, 5.53f);

            Debug.Log("DebugMFD: Attempting to retrieve AssetBundle: 'debugmfdmodel' from: '" + debugtoolsModDirectory + "debugmfdmodel.mhtasset'");
            GameObject modelprefab = DebugMFDUtilities.FileLoader.GetAssetBundleAsGameObject(debugtoolsModDirectory + "debugmfdmodel.mhtasset", "DebugMFD.prefab");
            if (modelprefab == null)
            {
                Debug.Log("DebugMFD: 'modelprefab' is null. We got problems.");
            }
            model = Instantiate(modelprefab, debugMFDModel.transform);
            if (model != null)
            {
                //Set the transform
                model.transform.localPosition = Vector3.zero;
                model.transform.rotation = Quaternion.Euler(new Vector3(-120f, 180f, 0f));

                Debug.Log("DebugMFD: Object name: " + model.name);
                DebugMFDAnimator = model.GetComponent<Animator>();
                DebugMFDAnimator.SetBool("Extended", true);

                //Screen is material 2 (index 1) on the Skinned Mesh
                ScreenBackground = model.transform.Find("Mesh").GetComponent<SkinnedMeshRenderer>().sharedMaterials[1];
            }
            else
            {
                Debug.Log("DebugMFD: 'model' is null, no clue what's happening there. Better restart, again!");
            }
        }

        private bool SetupTexts()
        {
            if(model != null)
            {
                PageName = model.transform.Find("Armature/Root/Extension/Rect/PageName").transform.GetComponent<TextMeshPro>();
                ModName = model.transform.Find("Armature/Root/Extension/Rect/ModName").transform.GetComponent<TextMeshPro>();

                //ButtonDescriptions setup

                foreach (DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons)))
                {
                    RectTransform temp = (RectTransform)model.transform.Find("Armature/Root/Extension/Rect/" + item.ToString());
                    ButtonDescriptions[item] = temp.GetComponent<TextMeshPro>();
                }

                foreach (DebugMFDInfoTexts item in Enum.GetValues(typeof(DebugMFDInfoTexts)))
                {
                    RectTransform temp = (RectTransform)model.transform.Find("Armature/Root/Extension/Rect/info_" + item.ToString());
                    InfoTexts[item] = temp.GetComponent<TextMeshPro>();
                }

                Debug.Log("DebugMFD: Successfully setup display texts.");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetupInteractableButtons()
        {
            if (!interactablesSetup)
            {
                foreach (DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons)))
                {
                    interactableButtons[item] = model.transform.Find("Buttons/" + item.ToString()).gameObject.AddComponent<VRInteractable>();

                    mfdPoseBounds = model.transform.Find("PoseBoundsArea").gameObject.AddComponent<PoseBounds>();
                    mfdPoseBounds.pose = GloveAnimation.Poses.Point;
                    mfdPoseBounds.size = new Vector3(2.75f, 2.75f, 0.75f);

                    interactableButtons[item].interactableName = ButtonDescriptions[item].text;
                    interactableButtons[item].OnInteract = new UnityEngine.Events.UnityEvent();
                    interactableButtons[item].button = VRInteractable.Buttons.Trigger;
                    interactableButtons[item].radius = 0.01f;
                    interactableButtons[item].sqrRadius = 0.01f;
                    interactableButtons[item].poseBounds = mfdPoseBounds;
                }
            }
            else
            {
                if (!activePage.Equals(null))
                {
                    Debug.Log("DebugMFD: activePage was not null, setting up VRInteractable Buttons for:" + activePage.PageName);
                    foreach (DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons)))
                    {
                        interactableButtons[item].OnInteract.RemoveAllListeners();
                        if (activePage.PageButtons[item].ButtonPressedEvent != null)
                        {
                            interactableButtons[item].OnInteract.AddListener(activePage.PageButtons[item].ButtonPressedEvent);
                        }
                        Debug.Log(interactableButtons[item].gameObject.name + " - " + activePage.PageButtons[item].ButtonDescription);

                        interactableButtons[item].interactableName = ButtonDescriptions[item].text;
                    }
                }
            }
            interactablesSetup = true;
        }

        public void GoHome()
        {
            if(home == null)
            {
                home = debugMFDModel.AddComponent<DebugMFDHome>();
            }
            home.GoHome();
        }

        public void MFDFinished()
        {
            debugset.DebugMFDFinished();
        }

        public void SetupBuiltInPages()
        {
            debugMFDModel.AddComponent<VehicleFunPage>();
            //debugMFDModel.AddComponent<InGameLog>();
            debugMFDModel.AddComponent<YoutubeDLPlayer>();
        }

        private void InitPage()
        {
            if(activePage.Equals(null)) { return; }
            foreach(DebugMFDInfoTexts item in Enum.GetValues(typeof(DebugMFDInfoTexts)))
            {
                this.InfoTexts[item].text = activePage.InfoTexts[item];
            }
            foreach(DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons)))
            {
                this.ButtonDescriptions[item].text = activePage.PageButtons[item].ButtonDescription;
                this.interactableButtons[item].interactableName = activePage.PageButtons[item].ButtonDescription;
            }
            PageName.text = activePage.PageName;
            ModName.text = activePage.ModName;
            if(activePage.PageBackground != null)
            {
                ScreenBackground.SetTexture("_MainTex", activePage.PageBackground);
            }
            else
            {
                ScreenBackground.SetTexture("_MainTex", defaultPageBackground);
            }
        }

        public void SetPage(DebugMFDPage page)
        {
            activePage.LostFocusSend();
            activePage = page;
            if (!DebugMFDPages.Contains(page))
            {
                DebugMFDPages.Add(page);
            }
            Debug.Log("DebugMFD: Set '" + page.PageName + "' as activePage.");
            SetupInteractableButtons();
            InitPage();
        }

        public void UpdatePage()
        {
            SetupInteractableButtons();
            InitPage();
        }
    }
}
