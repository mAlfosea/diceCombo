using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public Button continueButton;
	public Button newCampaignButton;
	public Button editorButton;
	public Button exitButton;

	// Use this for initialization
	void Start () {
		CampaignManager.Init ();

		if (CampaignManager.isCampaignGenerated == false) {
			this.continueButton.gameObject.SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ContinueCampaign () {
		SceneManager.LoadScene ("campaign", LoadSceneMode.Single);
	}

	public void NewCampaign () {
		CampaignManager.GenerateCampaign ();
		CampaignManager.SaveCampaign ();
		SceneManager.LoadScene ("campaign", LoadSceneMode.Single);
	}

	public void Editor () {
		SceneManager.LoadScene ("editor", LoadSceneMode.Single);		
	}
	public void Exit () {
		Application.Quit ();
	}
}
