using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamPanel : MonoBehaviour {

	public GameObject characterContainer;
	public GameObject charactrerPanelPrefab;
	public GameObject skillAttributionPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init (int newInitType) {
		
		foreach (Transform child in this.characterContainer.transform) {
			print ("je destroy " + child.name);
			GameObject.Destroy(child.gameObject);
		}
		this.characterContainer.transform.DetachChildren ();

		this.GenerateSummoner (newInitType);
		this.GenerateSupport (newInitType);
	}

	public void GenerateSummoner (int newInt) {
		SummonerData summonerScript = CampaignManager.playerTeam.mainSummoner;
		GameObject summonerGO = Instantiate (this.charactrerPanelPrefab, this.characterContainer.transform.position, this.transform.rotation, this.characterContainer.transform) as GameObject;
		summonerGO.name = summonerScript.summonerName;
		summonerGO.transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = summonerScript.summonerName;
		summonerGO.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("character_avatars/" + summonerScript.idAvatar);
		summonerGO.transform.GetChild (2).GetChild (0).GetChild (0).GetComponent<Text> ().text = summonerScript.pv.ToString() + " / " + summonerScript.pvMax.ToString ();
		summonerGO.transform.GetChild (2).GetChild (1).GetChild (0).GetComponent<Text> ().text = summonerScript.force.ToString ();
		summonerGO.transform.GetChild (2).GetChild (2).GetChild (0).GetComponent<Text> ().text = summonerScript.armure.ToString ();
		summonerGO.transform.GetChild (3).GetChild (0).GetChild (0).GetComponent<Text> ().text = summonerScript.nbSkillSlots.ToString ();
		summonerGO.transform.GetChild (3).GetChild (1).GetChild (0).GetComponent<Text> ().text = summonerScript.nbDice.ToString () + " / " + summonerScript.nbDiceMax.ToString ();

		summonerGO.GetComponent<PopupElement> ().summoner = summonerScript;
		summonerGO.GetComponent<PopupElement> ().characterType = 1;
		summonerGO.GetComponent<PopupElement> ().characterId = summonerScript.id;

		if (newInt == 1) {
			summonerGO.GetComponent<Button> ().enabled = false;
			summonerGO.GetComponent<Outline> ().enabled = false;
		} else {
			if (summonerScript.nbDice == summonerScript.nbDiceMax) {
				summonerGO.GetComponent<Button> ().enabled = false;
				summonerGO.GetComponent<Outline> ().enabled = false;
			}
		}

		for (int i = 0; i < summonerScript.nbSkillSlots; i++) {
			ActionsData actionTemp = DictionaryManager.GetAction (summonerScript.L_skills [i]);
			GameObject skillGO = Instantiate (this.skillAttributionPrefab, this.transform.position, this.transform.rotation, summonerGO.transform.GetChild (4)) as GameObject;

			skillGO.name = actionTemp.name;
			skillGO.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("skill_icons/" + actionTemp.id);
			skillGO.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("action_type_" + actionTemp.type);
			skillGO.transform.GetChild (3).GetComponent<Text> ().text = actionTemp.name;
			skillGO.GetComponent<Image> ().color = DictionaryManager.ActionColor (actionTemp.color);
			skillGO.GetComponent<PopupElement> ().action = actionTemp;
			skillGO.GetComponent<PopupElement> ().summoner = summonerScript;
			skillGO.GetComponent<PopupElement> ().characterType = 1;
			skillGO.GetComponent<PopupElement> ().characterId = summonerScript.id;
			skillGO.GetComponent<PopupElement> ().teamId = i; // c'est l'index du slot

			if (newInt == 2) {
				skillGO.GetComponent<Button> ().enabled = false;
				skillGO.GetComponent<Outline> ().enabled = false;
			}
		}
	}

	public void GenerateSupport (int newInt) {
		for (int i = 0; i < CampaignManager.playerTeam.supports.Length; i++) {
			SupportData supportScript = CampaignManager.playerTeam.supports[i];
			GameObject supportGO = Instantiate (this.charactrerPanelPrefab, this.characterContainer.transform.position, this.transform.rotation, this.characterContainer.transform) as GameObject;
			supportGO.name = supportScript.supportName;
			supportGO.transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = supportScript.supportName;
			supportGO.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("character_avatars/" + supportScript.idAvatar);
			supportGO.transform.GetChild (2).gameObject.SetActive (false);
			//supportGO.transform.GetChild (2).GetChild (0).GetChild (0).GetComponent<Text> ().text = supportScript.pv.ToString() + " / " + supportScript.pvMax.ToString ();
			//supportGO.transform.GetChild (2).GetChild (1).GetChild (0).GetComponent<Text> ().text = supportScript.force.ToString ();
			//supportGO.transform.GetChild (2).GetChild (2).GetChild (0).GetComponent<Text> ().text = supportScript.armure.ToString ();
			supportGO.transform.GetChild (3).GetChild (0).GetChild (0).GetComponent<Text> ().text = supportScript.nbSkillSlots.ToString ();
			supportGO.transform.GetChild (3).GetChild (1).GetChild (0).GetComponent<Text> ().text = supportScript.nbDice.ToString () + " / " + supportScript.nbDiceMax.ToString ();

			supportGO.GetComponent<PopupElement> ().support = supportScript;
			supportGO.GetComponent<PopupElement> ().characterType = 2;
			supportGO.GetComponent<PopupElement> ().characterId = i;

			if (newInt == 1) {
				supportGO.GetComponent<Button> ().enabled = false;
				supportGO.GetComponent<Outline> ().enabled = false;
			} else {
				if (supportScript.nbDice == supportScript.nbDiceMax) {
					supportGO.GetComponent<Button> ().enabled = false;
					supportGO.GetComponent<Outline> ().enabled = false;
				}
			}

			for (int j = 0; j < supportScript.nbSkillSlots; j++) {
				ActionsData actionTemp = DictionaryManager.GetAction (supportScript.L_skills [j]);
				GameObject skillGO = Instantiate (this.skillAttributionPrefab, this.transform.position, this.transform.rotation, supportGO.transform.GetChild (4)) as GameObject;

				skillGO.name = actionTemp.name;
				skillGO.transform.GetChild (1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("skill_icons/" + actionTemp.id);
				skillGO.transform.GetChild (2).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("action_type_" + actionTemp.type);
				skillGO.transform.GetChild (3).GetComponent<Text> ().text = actionTemp.name;
				skillGO.GetComponent<Image> ().color = DictionaryManager.ActionColor (actionTemp.color);
				skillGO.GetComponent<PopupElement> ().action = actionTemp;
				skillGO.GetComponent<PopupElement> ().support = supportScript;
				skillGO.GetComponent<PopupElement> ().characterType = 2;
				skillGO.GetComponent<PopupElement> ().characterId = i;
				skillGO.GetComponent<PopupElement> ().teamId = j;

				if (newInt == 2) {
					skillGO.GetComponent<Button> ().enabled = false;
					skillGO.GetComponent<Outline> ().enabled = false;
				}
			}
		}
	}
}
