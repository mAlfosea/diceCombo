using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanelEditor : MonoBehaviour {

	public Button ChangeCharacterButton;
	public Button RemoveCharacterButton;
	public Text summonerName;
	public Image summonerAvatar;
	private int characterType;
	public int teamId; // sert a savoir pour le "empty panel" s'il créer pour les players ou les monstres

	public Text pvText;
	public Text armureText;
	public Text forceText;

	public Text passiveDescText;

	public GameObject blocNbSkill;

	public GameObject blocNbDice;
	public int nbDice;

	public GameObject blocNbActions;
	public GameObject blocNbEtats;

	public SummonerData summonerScript;
	public SupportData supportScript;

	public GameObject skillPrefab;
	public GameObject talentPrefab;
	public GameObject dicePrefab;

	public delegate void CharacterUpdatedSummoner(SummonerData newSupportScript);
	public static event CharacterUpdatedSummoner E_SummonerUpdated;
	public delegate void CharacterUpdatedSupport(SupportData newSupportScript);
	public static event CharacterUpdatedSupport E_SupportUpdated;
	public delegate void CharacterRemoved(int newTeamId, int newCharacterType, int newId);
	public static event CharacterRemoved E_RemoveCharacter;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region Generation du panel
	public void GenerateSummoner () {

		this.summonerName.text = this.summonerScript.summonerName;
		this.summonerAvatar.sprite = Resources.Load<Sprite> ("character_avatars/" + summonerScript.idAvatar);
		this.characterType = summonerScript.characterType;

		if (this.transform.GetSiblingIndex () == 0) {
			this.RemoveCharacterButton.gameObject.SetActive (false);
		}

		this.pvText.text = this.summonerScript.pvMax.ToString ();
		this.armureText.text = "0";
		this.forceText.text = "0";

		if (this.summonerScript.L_talents[0] != 0) {
			EtatsData etat = DictionaryManager.GetEtat (this.summonerScript.L_talents [0]);
			this.passiveDescText.text = DictionaryManager.GetEtatDescription (etat);
		} else {
			this.passiveDescText.text = "Aucune capacité passive";
		}

		this.blocNbSkill.transform.GetChild (1).GetComponentInChildren<Text> ().text = this.summonerScript.nbSkillSlots.ToString ();

		this.nbDice = this.summonerScript.nbDice;
		this.GenerateDice ();

		this.GenerateActionsSlot ();
		this.GenerateTalentSlot ();

		this.ChangeCharacterButton.GetComponent<PopupElement> ().teamId = this.teamId;
		this.ChangeCharacterButton.GetComponent<PopupElement> ().characterId = this.summonerScript.id;
		this.ChangeCharacterButton.GetComponent<PopupElement> ().characterType = this.summonerScript.characterType;
	}

	public void GenerateSupport () {

		this.summonerName.text = this.supportScript.supportName;
		this.summonerAvatar.sprite = Resources.Load<Sprite> ("character_avatars/" + supportScript.idAvatar);
		this.characterType = supportScript.characterType;

		this.pvText.transform.parent.gameObject.SetActive (false);
		this.armureText.transform.parent.gameObject.SetActive (false);
		this.forceText.transform.parent.gameObject.SetActive (false);

		if (this.supportScript.L_talents.Count != 0) {
			EtatsData etat = DictionaryManager.GetEtat (this.supportScript.L_talents [0]);
			this.passiveDescText.text = DictionaryManager.GetEtatDescription (etat);
		} else {
			this.passiveDescText.text = "Aucune capacité passive";
		}

		this.blocNbSkill.transform.GetChild (1).GetComponentInChildren<Text> ().text = this.supportScript.nbSkillSlots.ToString ();

		this.nbDice = this.supportScript.nbDice;
		this.GenerateDice ();

		this.GenerateActionsSlot ();

		this.ChangeCharacterButton.GetComponent<PopupElement> ().teamId = this.teamId;
		this.ChangeCharacterButton.GetComponent<PopupElement> ().characterId = this.supportScript.id;
		this.ChangeCharacterButton.GetComponent<PopupElement> ().characterType = this.supportScript.characterType;
	}
	#endregion

	#region Gestion des dices
	private void GenerateDice () { // a optimiser en envoyant en parametre direct le nombre de dice du script en question
		if (this.characterType == 1 || this.characterType == 3) {
			for (int i = 0; i < this.summonerScript.nbDiceMax; i++) {
				GameObject diceTemp = Instantiate (this.dicePrefab, this.blocNbDice.transform.position, this.transform.rotation, this.blocNbDice.transform) as GameObject;
				diceTemp.name = i.ToString ();
			}
		} else {
			for (int i = 0; i < this.supportScript.nbDiceMax; i++) {
				GameObject diceTemp = Instantiate (this.dicePrefab, this.blocNbDice.transform.position, this.transform.rotation, this.blocNbDice.transform) as GameObject;
				diceTemp.name = i.ToString ();
			}		
		}
		this.ToggleDices ();
	}

	private void ToggleDices () {
		if (this.characterType == 1 || this.characterType == 3) {
			for (int i = 0; i < this.summonerScript.nbDiceMax; i++) {
				if ((i + 1) <= this.summonerScript.nbDice) {
					this.blocNbDice.transform.GetChild (i).GetComponent<Image> ().CrossFadeAlpha (1f, 0.1f, false);
				} else {
					this.blocNbDice.transform.GetChild (i).GetComponent<Image> ().CrossFadeAlpha (0.3f, 0.1f, false);			
				}
			}
		} else {
			for (int i = 0; i < this.supportScript.nbDiceMax; i++) {
				if ((i + 1) <= this.supportScript.nbDice) {
					this.blocNbDice.transform.GetChild (i).GetComponent<Image> ().CrossFadeAlpha (1f, 0.1f, false);
				} else {
					this.blocNbDice.transform.GetChild (i).GetComponent<Image> ().CrossFadeAlpha (0.3f, 0.1f, false);			
				}
			}
		}
	}

	public void ChangeDicesNumber (int newMod) {
		if (newMod == 1) {
			if (this.nbDice > 1) {
				this.nbDice -= 1;
			}
		} else {
			if (this.nbDice < this.blocNbDice.transform.childCount) {
				this.nbDice += 1;
			}			
		}

		if (this.characterType == 1 || this.characterType == 3) {
			this.summonerScript.nbDice = this.nbDice;
			if (E_SummonerUpdated != null) {
				E_SummonerUpdated (this.summonerScript);
			}
		} else {
			this.supportScript.nbDice = this.nbDice;
			if (E_SupportUpdated != null) {
				E_SupportUpdated (this.supportScript);
			}
		}
		this.ToggleDices ();
	}
	#endregion

	#region Gestion des skills
	public void ChangeNbSkills (int newMod) {
		if (this.characterType == 1 || this.characterType == 3) {
			if (newMod == 1) {
				if (this.summonerScript.nbSkillSlots > 2) {
					this.summonerScript.nbSkillSlots -= 1;
					this.summonerScript.L_skills.RemoveAt (this.summonerScript.nbSkillSlots);

					this.blocNbSkill.transform.GetChild (1).GetComponentInChildren<Text> ().text = this.summonerScript.nbSkillSlots.ToString ();

					if (E_SummonerUpdated != null) {
						E_SummonerUpdated (this.summonerScript);
					}
					this.UpdateActionsSlot (newMod);
				}
			} else {
				if (this.summonerScript.nbSkillSlots < 10) {
					this.summonerScript.nbSkillSlots += 1;
					this.summonerScript.L_skills.Add (0);

					this.blocNbSkill.transform.GetChild (1).GetComponentInChildren<Text> ().text = this.summonerScript.nbSkillSlots.ToString ();

					if (E_SummonerUpdated != null) {
						E_SummonerUpdated (this.summonerScript);
					}
					this.UpdateActionsSlot (newMod);
				}			
			}
		} else {
			if (newMod == 1) {
				if (this.supportScript.nbSkillSlots > 2) {
					this.supportScript.nbSkillSlots -= 1;
					this.supportScript.L_skills.RemoveAt (this.supportScript.nbSkillSlots);

					this.blocNbSkill.transform.GetChild (1).GetComponentInChildren<Text> ().text = this.supportScript.nbSkillSlots.ToString ();
					if (E_SupportUpdated != null) {
						E_SupportUpdated (this.supportScript);
					}
					this.UpdateActionsSlot (newMod);
				}
			} else {
				if (this.supportScript.nbSkillSlots < 6) {
					this.supportScript.nbSkillSlots += 1;
					this.supportScript.L_skills.Add (0);

					this.blocNbSkill.transform.GetChild (1).GetComponentInChildren<Text> ().text = this.supportScript.nbSkillSlots.ToString ();
					if (E_SupportUpdated != null) {
						E_SupportUpdated (this.supportScript);
					}
					this.UpdateActionsSlot (newMod);
				}			
			}
		}
	}

	private void GenerateActionsSlot () { // a optimiser en envoyant directement la liste des skill en parametre
		if (this.characterType == 1 || this.characterType == 3) {
			for (int i = 0; i < this.summonerScript.nbSkillSlots; i++) {
				GameObject skillTemp = Instantiate (this.skillPrefab, this.blocNbActions.transform.GetChild (0).position, this.transform.rotation, this.blocNbActions.transform.GetChild (0)) as GameObject;
				skillTemp.name = i.ToString ();
				skillTemp.transform.GetChild (3).GetComponentInChildren<Text> ().text = (i + 1).ToString ();
				skillTemp.GetComponent<PopupElement> ().teamId = this.teamId;
				skillTemp.GetComponent<PopupElement> ().characterType = this.summonerScript.characterType;
				skillTemp.GetComponent<PopupElement> ().characterId = this.summonerScript.id;
				skillTemp.GetComponent<PopupElement> ().isOnEditor = true;
			}
		} else {
			for (int i = 0; i < this.supportScript.nbSkillSlots; i++) {
				GameObject skillTemp = Instantiate (this.skillPrefab, this.blocNbActions.transform.GetChild (0).position, this.transform.rotation, this.blocNbActions.transform.GetChild (0)) as GameObject;
				skillTemp.name = i.ToString ();
				skillTemp.transform.GetChild (3).GetComponentInChildren<Text> ().text = (i + 1).ToString ();
				skillTemp.GetComponent<PopupElement> ().teamId = this.teamId;
				skillTemp.GetComponent<PopupElement> ().characterType = this.supportScript.characterType;
				skillTemp.GetComponent<PopupElement> ().characterId = this.supportScript.id;
				skillTemp.GetComponent<PopupElement> ().isOnEditor = true;
			}
		}
		this.UpdateActionsSlotImages ();
	}
	public void UpdateActionsSlot (int newMod) {
		if (newMod == 1) {
			Destroy (this.blocNbActions.transform.GetChild (0).GetChild (this.blocNbActions.transform.GetChild (0).childCount - 1).gameObject);
		} else {
			if (this.characterType == 1 || this.characterType == 3) {
				GameObject skillTemp = Instantiate (this.skillPrefab, this.blocNbActions.transform.GetChild (0).position, this.transform.rotation, this.blocNbActions.transform.GetChild (0)) as GameObject;
				skillTemp.name = (this.blocNbActions.transform.GetChild (0).childCount).ToString ();
				skillTemp.transform.GetChild (3).GetComponentInChildren<Text> ().text = (this.blocNbActions.transform.GetChild (0).childCount).ToString ();
				skillTemp.GetComponent<PopupElement> ().teamId = this.teamId;
				skillTemp.GetComponent<PopupElement> ().characterType = this.summonerScript.characterType;
				skillTemp.GetComponent<PopupElement> ().characterId = this.summonerScript.id;
				skillTemp.GetComponent<PopupElement> ().isOnEditor = true;
			} else {
				GameObject skillTemp = Instantiate (this.skillPrefab, this.blocNbActions.transform.GetChild (0).position, this.transform.rotation, this.blocNbActions.transform.GetChild (0)) as GameObject;
				skillTemp.name = (this.blocNbActions.transform.GetChild (0).childCount).ToString ();
				skillTemp.transform.GetChild (3).GetComponentInChildren<Text> ().text = (this.blocNbActions.transform.GetChild (0).childCount).ToString ();
				skillTemp.GetComponent<PopupElement> ().teamId = this.teamId;
				skillTemp.GetComponent<PopupElement> ().characterType = this.supportScript.characterType;
				skillTemp.GetComponent<PopupElement> ().characterId = this.supportScript.id;
				skillTemp.GetComponent<PopupElement> ().isOnEditor = true;
			}
		}
	}

	public void UpdateActionsSlotImages () {
		if (this.characterType == 1 || this.characterType == 3) {
			for (int i = 0; i < this.summonerScript.L_skills.Count; i++) {
				if (this.summonerScript.L_skills [i] != 0) {
					ActionsData actionTemp;
					DictionaryManager.actionsDictionary.TryGetValue (this.summonerScript.L_skills [i], out actionTemp);
					this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().action = actionTemp;
					this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<Image> ().color = DictionaryManager.ActionColor (actionTemp.color);
					//this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().teamId = this.teamId;
					//this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().characterType = this.characterType;
					//this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().characterId = this.summonerScript.id;
					this.blocNbActions.transform.GetChild (0).GetChild (i).GetChild (0).GetChild(0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("skill_icons/" + actionTemp.id);
					this.blocNbActions.transform.GetChild (0).GetChild (i).GetChild (2).gameObject.SetActive (false);
				} else {
					this.blocNbActions.transform.GetChild (0).GetChild (i).GetChild (2).gameObject.SetActive (true);
				}
			}
		} else {
			for (int i = 0; i < this.supportScript.L_skills.Count; i++) {
				if (this.supportScript.L_skills [i] != 0) {
					ActionsData actionTemp;
					DictionaryManager.actionsDictionary.TryGetValue (this.supportScript.L_skills [i], out actionTemp);
					this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().action = actionTemp;
					this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<Image> ().color = DictionaryManager.ActionColor (actionTemp.color);
					//this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().teamId = this.teamId;
					//this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().characterType = this.characterType;
					//this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().characterId = this.supportScript.id;
					this.blocNbActions.transform.GetChild (0).GetChild (i).GetChild (0).GetChild(0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("skill_icons/" + actionTemp.id);
					this.blocNbActions.transform.GetChild (0).GetChild (i).GetChild (2).gameObject.SetActive (false);
				} else {
					this.blocNbActions.transform.GetChild (0).GetChild (i).GetChild (2).gameObject.SetActive (true);
				}
			}
		}
	}
	#endregion

	#region Gestion des talents
	private void GenerateTalentSlot () { // a optimiser en envoyant directement la liste des skill en parametre
		if (this.characterType == 1 || this.characterType == 3) {
			//for (int i = 0; i < this.summonerScript.nbSkillSlots; i++) {
				for (int i = 0; i < 6; i++) {
				GameObject skillTemp = Instantiate (this.talentPrefab, this.blocNbEtats.transform.GetChild (0).position, this.transform.rotation, this.blocNbEtats.transform.GetChild (0)) as GameObject;
				skillTemp.name = i.ToString ();
				skillTemp.transform.GetChild (3).GetComponentInChildren<Text> ().text = (i + 1).ToString ();
				skillTemp.GetComponent<PopupElement> ().teamId = this.teamId;
				skillTemp.GetComponent<PopupElement> ().characterType = this.summonerScript.characterType;
				skillTemp.GetComponent<PopupElement> ().characterId = this.summonerScript.id;
			}
		} else {
			for (int i = 0; i < this.supportScript.nbSkillSlots; i++) {
				GameObject skillTemp = Instantiate (this.talentPrefab, this.blocNbEtats.transform.GetChild (0).position, this.transform.rotation, this.blocNbEtats.transform.GetChild (0)) as GameObject;
				skillTemp.name = i.ToString ();
				skillTemp.transform.GetChild (3).GetComponentInChildren<Text> ().text = (i + 1).ToString ();
				skillTemp.GetComponent<PopupElement> ().teamId = this.teamId;
				skillTemp.GetComponent<PopupElement> ().characterType = this.supportScript.characterType;
				skillTemp.GetComponent<PopupElement> ().characterId = this.supportScript.id;
			}
		}
		this.UpdateTalentsSlotImages ();
	}
	public void UpdateTalentSlot (int newMod) {
		if (newMod == 1) {
			Destroy (this.blocNbEtats.transform.GetChild (0).GetChild (this.blocNbEtats.transform.GetChild (0).childCount - 1).gameObject);
		} else {
			if (this.characterType == 1 || this.characterType == 3) {
				GameObject skillTemp = Instantiate (this.talentPrefab, this.blocNbEtats.transform.GetChild (0).position, this.transform.rotation, this.blocNbEtats.transform.GetChild (0)) as GameObject;
				skillTemp.name = (this.blocNbEtats.transform.GetChild (0).childCount).ToString ();
				skillTemp.transform.GetChild (3).GetComponentInChildren<Text> ().text = (this.blocNbEtats.transform.GetChild (0).childCount).ToString ();
				skillTemp.GetComponent<PopupElement> ().teamId = this.teamId;
				skillTemp.GetComponent<PopupElement> ().characterType = this.summonerScript.characterType;
				skillTemp.GetComponent<PopupElement> ().characterId = this.summonerScript.id;
			} else {
				GameObject skillTemp = Instantiate (this.talentPrefab, this.blocNbEtats.transform.GetChild (0).position, this.transform.rotation, this.blocNbEtats.transform.GetChild (0)) as GameObject;
				skillTemp.name = (this.blocNbEtats.transform.GetChild (0).childCount).ToString ();
				skillTemp.transform.GetChild (3).GetComponentInChildren<Text> ().text = (this.blocNbEtats.transform.GetChild (0).childCount).ToString ();
				skillTemp.GetComponent<PopupElement> ().teamId = this.teamId;
				skillTemp.GetComponent<PopupElement> ().characterType = this.supportScript.characterType;
				skillTemp.GetComponent<PopupElement> ().characterId = this.supportScript.id;
			}
		}
	}

	public void UpdateTalentsSlotImages () {
		if (this.characterType == 1 || this.characterType == 3) {
			for (int i = 0; i < this.summonerScript.L_talents.Count; i++) {
				if (this.summonerScript.L_talents [i] != 0) {
					EtatsData etatTemp;
					DictionaryManager.etatsDictionary.TryGetValue (this.summonerScript.L_talents [i], out etatTemp);
					this.blocNbEtats.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().etat = etatTemp;
					this.blocNbEtats.transform.GetChild (0).GetChild (i).GetComponent<Image> ().color = DictionaryManager.ActionColor (5);
					//this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().teamId = this.teamId;
					//this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().characterType = this.characterType;
					//this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().characterId = this.summonerScript.id;
					this.blocNbEtats.transform.GetChild (0).GetChild (i).GetChild (0).GetChild(0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("etat_icons/" + etatTemp.id);
					this.blocNbEtats.transform.GetChild (0).GetChild (i).GetChild (2).gameObject.SetActive (false);
				} else {
					this.blocNbEtats.transform.GetChild (0).GetChild (i).GetChild (2).gameObject.SetActive (true);
				}
			}
		} else {
			for (int i = 0; i < this.supportScript.L_talents.Count; i++) {
				if (this.supportScript.L_talents [i] != 0) {
					EtatsData etatTemp;
					DictionaryManager.etatsDictionary.TryGetValue (this.supportScript.L_talents [i], out etatTemp);
					this.blocNbEtats.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().etat = etatTemp;
					this.blocNbEtats.transform.GetChild (0).GetChild (i).GetComponent<Image> ().color = DictionaryManager.ActionColor (5);
					//this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().teamId = this.teamId;
					//this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().characterType = this.characterType;
					//this.blocNbActions.transform.GetChild (0).GetChild (i).GetComponent<PopupElement> ().characterId = this.supportScript.id;
					this.blocNbEtats.transform.GetChild (0).GetChild (i).GetChild (0).GetChild(0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("etat_icons/" + etatTemp.id);
					this.blocNbEtats.transform.GetChild (0).GetChild (i).GetChild (2).gameObject.SetActive (false);
				} else {
					this.blocNbEtats.transform.GetChild (0).GetChild (i).GetChild (2).gameObject.SetActive (true);
				}
			}
		}
	}
	#endregion

	public void RemoveCharacter () {
		if (this.characterType == 1 || this.characterType == 3) {
			if (E_RemoveCharacter != null) { 
				E_RemoveCharacter (this.teamId, this.characterType, this.summonerScript.id);
				Destroy (this.gameObject);
			}
		} else {
			if (E_RemoveCharacter != null) { 
				E_RemoveCharacter (this.teamId, this.characterType, this.supportScript.id);
				Destroy (this.gameObject);
			}			
		}
	}
}
