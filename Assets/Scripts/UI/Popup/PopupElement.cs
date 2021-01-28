using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupElement : MonoBehaviour {

	public int teamId;
	public int characterType;
	public int characterId;
	public ActionsData action;
	public EtatsData etat;
	public SummonerData summoner;
	public SupportData support;

	public bool isOnEditor;

	#region Show / Hide Card
	public delegate void ShowCard(GameObject newPopUpType, ActionsData newAction);
	public static event ShowCard E_ShowCard;

	public delegate void HideCard();
	public static event HideCard E_HideCard;
	#endregion

	#region Show / Hide Etat
	public delegate void ShowEtat(GameObject newEtatPos, EtatsData newEtat);
	public static event ShowEtat E_ShowEtat;

	public delegate void HideEtat();
	public static event HideEtat E_HideEtat;
	#endregion

	#region Show Popup
	public delegate void ShowActionsPopup(int newTeamId, int newCharacterType, int newCharacterId, int newSlotIndex);
	public static event ShowActionsPopup E_ShowActionsPopup;

	public delegate void ShowTalentsPopup(int newTeamId, int newCharacterType, int newCharacterId, int newSlotIndex);
	public static event ShowTalentsPopup E_ShowTalentsPopup;

	public delegate void ShowCharactersPopup(int newTeamId, int newCharacterType, int newCharacterId, int newSlotIndex);
	public static event ShowCharactersPopup E_ShowCharactersPopup;
	#endregion

	#region Change Skill / Remove
	public delegate void ChangeSkill(int newActionId);
	public static event ChangeSkill E_ChangeSkill;

	public delegate void RemoveSkill(int newTeamId, int newCharacterType, int newCharacterId, int newSlotIndex);
	public static event RemoveSkill E_RemoveSkill;
	#endregion

	#region Change Talent / Remove
	public delegate void ChangePerk(int newTalentId);
	public static event ChangePerk E_ChangeTalent;

	public delegate void RemovePerk(int newTeamId, int newCharacterType, int newCharacterId, int newSlotIndex);
	public static event RemovePerk E_RemoveTalent;
	#endregion

	#region Add Character
	public delegate void AddCharacter(int newCharacterId);
	public static event AddCharacter E_ChangeCharacter;
	#endregion

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region Show Card
	public void ShowCardInfos() {
		if (E_ShowCard != null) {
			E_ShowCard (this.gameObject, this.action);
		}
	}

	public void HideCardInfos() {
		if (E_HideCard != null) {
			E_HideCard ();
		}
	}
	#endregion

	#region Show Etat
	public void ShowEtatInfos() {
		if (E_ShowEtat != null) {
			E_ShowEtat (this.gameObject, this.etat);
		}
	}

	public void HideEtatInfos() {
		if (E_HideEtat != null) {
			E_HideEtat ();
		}
	}
	#endregion

	public void ShowActions () {
		if (E_ShowActionsPopup != null && isOnEditor) {
			E_ShowActionsPopup (this.teamId, this.characterType, this.characterId, this.transform.GetSiblingIndex());
		}
	}

	public void ShowTalents () {
		if (E_ShowTalentsPopup != null) {
			E_ShowTalentsPopup (this.teamId, this.characterType, this.characterId, this.transform.GetSiblingIndex());
		}
	}

	public void ShowCharacters () {
		if (E_ShowCharactersPopup != null) {
			E_ShowCharactersPopup (this.teamId, this.characterType, this.characterId, this.transform.GetSiblingIndex());
		}
	}

	public void ChangeAction () {
		this.HideCardInfos ();
		if (E_ChangeSkill != null) {
			E_ChangeSkill (this.action.id);
		}
	}

	public void ChangeTalent () {
		this.HideEtatInfos ();
		if (E_ChangeTalent != null) {
			E_ChangeTalent (this.etat.id);
		}
	}

	public void ChangeCharacter () {
		if (this.characterType == 1) {
			if (E_ChangeCharacter != null) {
				E_ChangeCharacter (this.summoner.id);
			}
		} else if (this.characterType == 2) {
			if (E_ChangeCharacter != null) {
				E_ChangeCharacter (this.support.id);
			}
		} else {
			if (E_ChangeCharacter != null) {
				E_ChangeCharacter (this.summoner.id);
			}
		}
	}

	public void RemoveAction () {
		if (E_RemoveSkill != null) {
			E_RemoveSkill (this.teamId, this.characterType, this.characterId, this.transform.GetSiblingIndex());
		}
	}

	public void RemoveTalent () {
		if (E_RemoveTalent != null) {
			E_RemoveTalent (this.teamId, this.characterType, this.characterId, this.transform.GetSiblingIndex());
		}
	}
}
