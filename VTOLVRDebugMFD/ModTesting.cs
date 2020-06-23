using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModTestingFreeflight
{
    //No longer necessary, and didn't work anyway!
    //Thank you Marsh for including this in the ModLoader!
    public class ModTesting : MonoBehaviour
    {
        private VTOLVehicles vehicleEnum = VTOLVehicles.FA26B;
        private string pilotName = "Ray";

        private void Awake()
        {
            //vehicleEnum = VTOLAPI.GetPlayersVehicleEnum();
            DontDestroyOnLoad(this.gameObject);
            LoadIntoLevelAfterModLoad();
        }

        public void LoadIntoLevelAfterModLoad()
        {
            StartCoroutine(LoadLevel());
        }

        private IEnumerator LoadLevel()
        {
            VTMapManager.nextLaunchMode = VTMapManager.MapLaunchModes.Scenario;
            LoadingSceneController.LoadScene(7);

            yield return new WaitForSeconds(5);
            //After here we should be in the loader scene

            Debug.Log("ModTesting: Setting Pilot");
            PilotSaveManager.current = PilotSaveManager.pilots[pilotName];
            Debug.Log("ModTesting: Going though All built in campaigns");
            if (VTResources.GetBuiltInCampaigns() != null)
            {
                foreach (VTCampaignInfo info in VTResources.GetBuiltInCampaigns())
                {

                    if (vehicleEnum == VTOLVehicles.AV42C && info.campaignID == "av42cQuickFlight")
                    {
                        Debug.Log("ModTesting: Setting Campaign");
                        PilotSaveManager.currentCampaign = info.ToIngameCampaign();
                        Debug.Log("ModTesting: Setting Vehicle");
                        PilotSaveManager.currentVehicle = VTResources.GetPlayerVehicle(info.vehicle);
                        break;
                    }

                    if (vehicleEnum == VTOLVehicles.FA26B && info.campaignID == "fa26bFreeFlight")
                    {
                        Debug.Log("ModTesting: Setting Campaign");
                        PilotSaveManager.currentCampaign = info.ToIngameCampaign();
                        Debug.Log("ModTesting: Setting Vehicle");
                        PilotSaveManager.currentVehicle = VTResources.GetPlayerVehicle(info.vehicle);
                        break;
                    }
                }
            }
            else
                Debug.Log("ModTesting: Campaigns are null");

            PilotSaveManager.currentCampaign.missions = VTResources.GetBuiltInCampaign("fa26bFreeFlight").ToIngameCampaign().missions;
            Debug.Log("ModTesting: Going though All missions in that campaign");
            foreach (CampaignScenario cs in PilotSaveManager.currentCampaign.missions)
            {
                Debug.Log("ModTesting: CampaignScenario == " + cs.scenarioID);
                if (cs.scenarioID == "freeFlight" || cs.scenarioID == "Free Flight")
                {
                    Debug.Log("ModTesting: Setting Scenario");
                    PilotSaveManager.currentScenario = cs;
                    break;
                }
            }

            VTScenario.currentScenarioInfo = VTResources.GetScenario(PilotSaveManager.currentScenario.scenarioID, PilotSaveManager.currentCampaign);

            Debug.Log(string.Format("ModTesting: Loading into game, Pilot:{3}, Campaign:{0}, Scenario:{1}, Vehicle:{2}",
                PilotSaveManager.currentCampaign.campaignName, PilotSaveManager.currentScenario.scenarioName,
                PilotSaveManager.currentVehicle.vehicleName, pilotName));

            LoadingSceneController.instance.PlayerReady(); //<< Auto Ready

            while (SceneManager.GetActiveScene().buildIndex != 7)
            {
                //Pausing this method till the loader scene is unloaded
                yield return null;
            }
        }
    }
}
