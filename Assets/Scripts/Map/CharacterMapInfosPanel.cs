using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMapInfosPanel : MonoBehaviour {

	public SummonerData summoner;
	public SupportData support;

	public Text pvText;
	public Image characterSprite;

	public GameObject skillPanel;
	public GameObject skillContainer;
	public GameObject skillPrefab;

	public GameObject diceBloc;

	public GameObject skillPanelInitialPos;
	public GameObject skillPanelTargetPos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void InitSummonerInfos (SummonerData newSummoner) {
		this.summoner = newSummoner;

		this.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("character_avatars/" + newSummoner.idAvatar);
			
		this.pvText.text = newSummoner.pv.ToString() + " / " + newSummoner.pvMax.ToString();
		this.diceBloc.transform.GetChild (1).GetComponent<Text> ().text = newSummoner.nbDice.ToString () + " / " + newSummoner.nbDiceMax.ToString();

		skillPanelTargetPos.transform.position = new Vector3(20f + (60f * this.summoner.nbSkillSlots), skillPanelTargetPos.transform.position.y, skillPanelTargetPos.transform.position.z);

		this.GenerateSkillSlots ();
	}

	public void InitSupportInfos (SupportData newSupport) {
		this.support = newSupport;

		this.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("character_avatars/" + newSupport.idAvatar);

		this.diceBloc.transform.GetChild (1).GetComponent<Text> ().text = newSupport.nbDice.ToString () + " / " + newSupport.nbDiceMax.ToString();

		skillPanelTargetPos.transform.position = new Vector3(20f + (60f * this.support.nbSkillSlots), skillPanelTargetPos.transform.position.y, skillPanelTargetPos.transform.position.z);

		this.GenerateSkillSlots ();
	}

	public void GenerateSkillSlots () {
		
		foreach (Transform child in this.skillContainer.transform) {
			//print ("je destroy " + child.name);
			GameObject.Destroy(child.gameObject);
		}
		this.skillContainer.transform.DetachChildren ();

		if (this.summoner.id != 0) {

			for (int i = 0; i < this.summoner.L_skills.Count; i++) {
				GameObject skillTemp = Instantiate (this.skillPrefab, this.skillContainer.transform.position, this.transform.rotation, this.skillContainer.transform) as GameObject;
				skillTemp.name = i.ToString ();
				skillTemp.transform.GetChild (3).GetComponentInChildren<Text> ().text = (i + 1).ToString ();
				skillTemp.transform.GetChild (1).gameObject.SetActive (false);

				//print ("j'instancie " + skillTemp.name);
				//skillTemp.GetComponent<PopupElement> ().teamId = this.teamId;
				skillTemp.GetComponent<PopupElement> ().characterType = this.summoner.characterType;
				skillTemp.GetComponent<PopupElement> ().characterId = this.summoner.id;
			}
		} else {

			for (int i = 0; i < this.support.L_skills.Count; i++) {
				GameObject skillTemp = Instantiate (this.skillPrefab, this.skillContainer.transform.position, this.transform.rotation, this.skillContainer.transform) as GameObject;
				skillTemp.name = i.ToString ();
				skillTemp.transform.GetChild (3).GetComponentInChildren<Text> ().text = (i + 1).ToString ();
				skillTemp.transform.GetChild (1).gameObject.SetActive (false);
				//skillTemp.GetComponent<PopupElement> ().teamId = this.teamId;
				skillTemp.GetComponent<PopupElement> ().characterType = this.support.characterType;
				skillTemp.GetComponent<PopupElement> ().characterId = this.support.id;
			}		
		}
		this.UpdateActionsSlotImages ();
	}

	#region Gestion des skills
	public void UpdateActionsSlot (int newMod) {
		if (newMod == 1) {
			Destroy (this.skillContainer.transform.GetChild (0).GetChild (this.skillContainer.transform.GetChild (0).childCount - 1).gameObject);
		} else {
			if (this.summoner.id != 0) {
				GameObject skillTemp = Instantiate (this.skillPrefab, this.skillContainer.transform.GetChild (0).position, this.transform.rotation, this.skillContainer.transform.GetChild (0)) as GameObject;
				skillTemp.name = (this.skillContainer.transform.GetChild (0).childCount).ToString ();
				skillTemp.transform.GetChild (3).GetComponentInChildren<Text> ().text = (this.skillContainer.transform.GetChild (0).childCount).ToString ();
				skillTemp.transform.GetChild (1).gameObject.SetActive (false);
				//skillTemp.GetComponent<PopupElement> ().teamId = this.teamId;
				skillTemp.GetComponent<PopupElement> ().characterType = this.summoner.characterType;
				skillTemp.GetComponent<PopupElement> ().characterId = this.summoner.id;
			} else {
				GameObject skillTemp = Instantiate (this.skillPrefab, this.skillContainer.transform.GetChild (0).position, this.transform.rotation, this.skillContainer.transform.GetChild (0)) as GameObject;
				skillTemp.name = (this.skillContainer.transform.GetChild (0).childCount).ToString ();
				skillTemp.transform.GetChild (3).GetComponentInChildren<Text> ().text = (this.skillContainer.transform.GetChild (0).childCount).ToString ();
				skillTemp.transform.GetChild (1).gameObject.SetActive (false);
				//skillTemp.GetComponent<PopupElement> ().teamId = this.teamId;
				skillTemp.GetComponent<PopupElement> ().characterType = this.support.characterType;
				skillTemp.GetComponent<PopupElement> ().characterId = this.support.id;
			}
		}
	}

	public void UpdateActionsSlotImages () {
		if (this.summoner.id != 0) {
			for (int i = 0; i < this.summoner.L_skills.Count; i++) {
				ActionsData actionTemp = DictionaryManager.GetAction(this.summoner.L_skills [i]);

				this.skillContainer.transform.GetChild (i).GetComponent<PopupElement> ().action = actionTemp;
				this.skillContainer.transform.GetChild (i).GetComponent<Image> ().color = DictionaryManager.ActionColor (actionTemp.color);
				this.skillContainer.transform.GetChild (i).GetChild (0).GetChild(0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("skill_icons/" + actionTemp.id);
				this.skillContainer.transform.GetChild (i).GetChild (2).gameObject.SetActive (false);
			}
		} else {
			for (int i = 0; i < this.support.L_skills.Count; i++) {
				ActionsData actionTemp = DictionaryManager.GetAction (this.support.L_skills [i]);

				this.skillContainer.transform.GetChild (i).GetComponent<PopupElement> ().action = actionTemp;
				this.skillContainer.transform.GetChild (i).GetComponent<Image> ().color = DictionaryManager.ActionColor (actionTemp.color);
				this.skillContainer.transform.GetChild (i).GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("skill_icons/" + actionTemp.id);
				this.skillContainer.transform.GetChild (i).GetChild (2).gameObject.SetActive (false);
			}
		}
	}
	#endregion

	#region Expand / Hide SkillPanel
	public void ExpandSkillPanel () {
		iTween.MoveTo (this.skillPanel, iTween.Hash(
			"position", skillPanelTargetPos.transform.position, 
			"time", 0.5f, 
			"easetype", iTween.EaseType.easeOutBack));
	}

	public void HideSkillPanel () {
		iTween.MoveTo (this.skillPanel, iTween.Hash(
			"position", this.skillPanelInitialPos.transform.position, 
			"time", 0.5f, 
			"easetype", iTween.EaseType.easeOutBack));
	}
	#endregion
}
