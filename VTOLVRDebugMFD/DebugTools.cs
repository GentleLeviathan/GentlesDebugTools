using UnityEngine;
using System.Collections;
using System.IO;
using GentlesDebugTools.MFD;

namespace GentlesDebugTools
{
    public class DebugSet : VTOLMOD
    {
        public static bool ReadyForMFD = false;
        public static bool ModTestingMode = false;
        public static DebugSet _instance;
        public static string vIndex = "v0.4a";

        public delegate void dmfdFinished();
        public event dmfdFinished finished;
        public bool DebugMFD_Setup_Finished = false;

        private GameObject DebugMFD;
        private AssetBundle bundle;

        public void Awake()
        {
            if (this.gameObject != null)
            {
                DontDestroyOnLoad(this.gameObject);
            }
            _instance = this;

            //Setup Events
            VTOLAPI.SceneLoaded += SceneLoaded;
            VTOLAPI.MissionReloaded += MissionReloaded;
            base.LogWarning("DebugTools: Awake()");

            bundle = DebugMFDUtilities.FileLoader.GetAssetBundleFromPath(Directory.GetCurrentDirectory() + @"\VTOLVR_ModLoader\mods\Gentle's Debug Tools\" + "debugmfdmodel.mhtasset");
        }

        public override void ModLoaded()
        {
            base.ModLoaded();

            if(this.gameObject != null)
            {
                DontDestroyOnLoad(this.gameObject);
            }

            //If we are in testing mode, immediately load a freeflight using the last used vehicle from the pilot save.
            if (ModTestingMode)
            {
                gameObject.AddComponent<ModTestingFreeflight.ModTesting>();
            }
        }


        private void SceneLoaded(VTOLScenes scene)
        {
            if(scene == VTOLScenes.Akutan || scene == VTOLScenes.CustomMapBase)
            {
                ReadyForMFD = true;
                StartCoroutine(MFDWaiter());
            }
            else
            {
                ReadyForMFD = false;
                DestroyMFDInstance();
            }
        }

        private void MissionReloaded()
        {
            ReadyForMFD = true;
            StartCoroutine(MFDWaiter());
        }

        private IEnumerator MFDWaiter()
        {
            yield return new WaitForSeconds(5f);
            InitMFD();
            yield break;
        }

        //Create the DebugMFD object
        private void InitMFD()
        {
            if (ReadyForMFD)
            {
                base.LogWarning("DebugTools: Instantiating DebugMFD.");

                GameObject pv;
                DebugMFD = new GameObject("DebugMFD", typeof(DebugMFD));
                DebugMFD.GetComponent<DebugMFD>().bundle = bundle;
                if(VTOLAPI.GetPlayersVehicleEnum() != VTOLVehicles.None)
                {
                    pv = VTOLAPI.GetPlayersVehicleGameObject();
                }
                else
                {
                    pv = null;
                }
                if (pv != null)
                {
                    if(DebugMFD != null)
                    {
                        DebugMFD.transform.parent = pv.transform;
                        DebugMFD.transform.localPosition = Vector3.zero;
                        DebugMFD.transform.localRotation = Quaternion.identity;

                    }
                    else
                    {
                        base.Log("DebugTools: DebugMFD is null. Instantiate didn't work either :*(");
                    }
                }
                else
                {
                    Debug.LogWarning("DebugTools: Player Vehicle is null?");
                }
            }
        }

        private void DestroyMFDInstance()
        {
            if(DebugMFD != null)
            {
                Destroy(DebugMFD);
            }
        }

        public void DebugMFDFinished()
        {
            finished?.Invoke();
            DebugMFD_Setup_Finished = true;
        }
    }

    //DEBUGGING TOOLS-----------------------------------------------------------
    public static class DebugTools
    {
        
    }
    //END DEBUGGING TOOLS-------------------------------------------------------
}
