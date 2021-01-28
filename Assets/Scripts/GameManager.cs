using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameManager : MonoBehaviour {

	//public static List<GameObject> L_Summoners = new List<GameObject> ();
	//public static List<GameObject> L_Enemies = new List<GameObject> ();

	// Use this for initialization
	void Start () {
		CampaignManager.Init ();
		this.GetComponent<UiManager> ().Init ();

		if (CampaignManager.combatEnded == false) {
			StartCoroutine (this.GetComponent<UiManager> ().HideBlackScreen ());
		} else {
			StartCoroutine (this.ChangeDay (1));
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SaveCampaign () {
		CampaignManager.SaveCampaign ();
		print ("J'ai SAVE la campagne");
	}

	public void Abandonner () {
		CampaignManager.isCampaignGenerated = false;
		CampaignManager.SaveCampaign ();
		SceneManager.LoadScene ("mainMenu", LoadSceneMode.Single);
	}

	public void SaveEtMain () {
		CampaignManager.SaveCampaign ();
		SceneManager.LoadScene ("mainMenu", LoadSceneMode.Single);
	}

	public void EndTurnButton () {
		CampaignManager.SaveCampaign ();
		CampaignManager.daysToRemove = 1;
		this.LaunchChangeDay (2);
	}

	public void LaunchChangeDay (int newInt) {
		StartCoroutine (this.ChangeDay(newInt));
	}

	public IEnumerator ChangeDay (int newDayState) {
		//print ("je change le day : " + CampaignManager.daysToRemove.ToString());

		this.GetComponent<UiManager> ().blackScreen.SetActive (true);

		if (newDayState == 2) {
			StartCoroutine (this.GetComponent<UiManager> ().ShowBlackScreen ());
			yield return new WaitForSeconds (0.5f);		
		}

		StartCoroutine (this.GetComponent<UiManager> ().ShowChangementDeJour ());
		yield return new WaitForSeconds (0.5f);

		int dayToRemove = CampaignManager.daysToRemove;

		while (dayToRemove > 0) {
			CampaignManager.day -= 1;
			this.GetComponent<UiManager> ().UpdateInfos ();
			dayToRemove -= 1;
			yield return new WaitForSeconds (0.3f);
		}

		yield return new WaitForSeconds (0.5f);

		StartCoroutine (this.GetComponent<UiManager> ().HideBlackScreen ());
		StartCoroutine (this.GetComponent<UiManager> ().HideChangementDeJour ());

		yield return new WaitForSeconds (0.5f);
		CampaignManager.AddDay ();
		StartCoroutine (this.GetComponent<UiManager> ().AddGold ());
		StartCoroutine (this.GetComponent<UiManager> ().AddSoul ());
	}
}
