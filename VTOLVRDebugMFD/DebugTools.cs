using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace GentlesDebugTools
{
    public class DebugSet : VTOLMOD
    {
        public static bool ReadyForMFD = false;
        public static bool ModTestingMode = false;
        private GameObject DebugMFD;
        public static DebugSet instance;

        public delegate void dmfdFinished();
        public event dmfdFinished _finished;
        public bool DebugMFD_Setup_Finished = false;

        public void Awake()
        {
            if (this.gameObject != null)
            {
                DontDestroyOnLoad(this.gameObject);
            }
            instance = this;

            //Setup Events
            SceneManager.sceneLoaded += SceneLoaded;
            base.LogWarning("DebugTools: Awake()");
        }

        public override void ModLoaded()
        {
            base.ModLoaded();

            if(this.gameObject != null)
            {
                DontDestroyOnLoad(this.gameObject);
                base.Log("DebugTools: this.gameobject is not null.");
            }

            //If we are in testing mode, immediately load a freeflight using the last used vehicle from the pilot save.
            if (ModTestingMode)
            {
                gameObject.AddComponent<ModTestingFreeflight.ModTesting>();
            }
        }


        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            //If we are in a scene that contains a player-controlled plane (GameRig is just the SteamVR Rig and accompanying stuffs, not a plane)
            if(arg0.name == "CustomMapBase" || arg0.name == "Akutan")
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
                        DebugMFD.transform.localRotation = Quaternion.Euler(Vector3.zero);

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
            _finished?.Invoke();
            DebugMFD_Setup_Finished = true;
        }
    }

    //DEBUGGING TOOLS-----------------------------------------------------------
    public static class DebugTools
    {
        
    }
    //END DEBUGGING TOOLS-------------------------------------------------------
}
