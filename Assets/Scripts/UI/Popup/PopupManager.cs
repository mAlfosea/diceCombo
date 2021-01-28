using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour {

	public GameObject popupMask;
	public GameObject popup_Actions;
	public GameObject popup_Talents;
	public GameObject popup_Characters;

	public GameObject popup_Chasse;
	public GameObject panel_Fight;

	public GameObject popup_Boss;

	public GameObject popup_Upgrade;
	public GameObject panel_Upgrade;

	public GameObject popup_Shop;
	public GameObject panel_Shop;
	public GameObject panel_ItemAttribution;

	public GameObject popup_Inn;

	private bool popupActionsGenerated;
	private bool popupTalentsGenerated;
	private bool popupCharactersGenerated;

	public GameObject popup_Card;
	public GameObject popup_Etat;

	public GameObject keywordsContainer;

	public int teamId;
	public int characterType;
	public int characterId;
	public int slotIndex;

	public delegate void ChangeSkill(int newTeamID, int newCharacterType, int newCharacterId, int newSkillIndex, int newActionId);
	public static event ChangeSkill E_ChangeSkill;

	public delegate void ChangePerk(int newTeamID, int newCharacterType, int newCharacterId, int newSkillIndex, int newEtatId);
	public static event ChangePerk E_ChangeTalent;

	public delegate void AddCharacter(int newTeamID, int newCharacterType, int newCharacterId, int newSkillIndex, int newActionId);
	public static event AddCharacter E_ChangeCharacter;

	void OnEnable()
	{
		PopupElement.E_ShowCard += this.ShowCard;
		PopupElement.E_HideCard += this.HideCard;
		PopupElement.E_ShowEtat += this.ShowEtat;
		PopupElement.E_HideEtat += this.HideEtat;
		PopupElement.E_ShowActionsPopup += this.PrepareActionsPopup;
		PopupElement.E_ShowTalentsPopup += this.PrepareTalentsPopup;
		PopupElement.E_ShowCharactersPopup += this.PrepareCharactersPopup;
		PopupElement.E_ChangeSkill += this.ChangeAction;
		PopupElement.E_ChangeTalent += this.ChangeTalent;
		PopupElement.E_ChangeCharacter += this.ChangeCharacter;
		CardPanel.E_CardHovered += this.GenerateKeywords;
		CardPanel.E_CardOut += this.DestroyKeywords;
	}


	void OnDisable()
	{
		PopupElement.E_ShowCard -= this.ShowCard;
		PopupElement.E_HideCard -= this.HideCard;
		PopupElement.E_ShowEtat -= this.ShowEtat;
		PopupElement.E_HideEtat -= this.HideEtat;
		PopupElement.E_ShowActionsPopup -= this.PrepareActionsPopup;
		PopupElement.E_ShowTalentsPopup -= this.PrepareTalentsPopup;
		PopupElement.E_ShowCharactersPopup -= this.PrepareCharactersPopup;
		PopupElement.E_ChangeSkill -= this.ChangeAction;
		PopupElement.E_ChangeTalent -= this.ChangeTalent;
		PopupElement.E_ChangeCharacter -= this.ChangeCharacter;
		CardPanel.E_CardHovered -= this.GenerateKeywords;
		CardPanel.E_CardOut -= this.DestroyKeywords;
	}

	// Use this for initialization
	void Start () {	
		this.HidePopup (1);	
		this.HidePopup (2);	
		this.HidePopup (3);	
		this.HidePopup (4);	
		this.HidePopup (5);	
		this.HidePopup (6);	
		this.HidePopup (7);	
		this.HidePopup (8);	
		this.HidePopup (9);	
	}
	
	// Update is called once per frame
	void Update () {		
	}

	#region Pop up Action / Talent / Characters pour l'editor
	public void PrepareActionsPopup (int newTeamId, int newCharacterType, int newCharacterId, int newSkillIndex) {
		this.teamId = newTeamId;
		this.characterType = newCharacterType;
		this.characterId = newCharacterId;
		this.slotIndex = newSkillIndex;

		if (this.popupActionsGenerated == false) {
			this.popup_Actions.GetComponent<PopupGenerator> ().GenerateActionsPopup ();
			this.popupActionsGenerated = true;
		}

		this.ShowPopup (1);
	}

	public void PrepareTalentsPopup (int newTeamId, int newCharacterType, int newCharacterId, int newSkillIndex) {
		this.teamId = newTeamId;
		this.characterType = newCharacterType;
		this.characterId = newCharacterId;
		this.slotIndex = newSkillIndex;

		if (this.popupTalentsGenerated == false) {
			this.popup_Talents.GetComponent<PopupGenerator> ().GenerateTalentsPopup ();
			this.popupTalentsGenerated = true;
		}

		this.ShowPopup (3);
	}

	public void PrepareCharactersPopup (int newTeamId, int newCharacterType, int newCharacterId, int newSkillIndex) {
		this.teamId = newTeamId;
		this.characterType = newCharacterType;
		this.characterId = newCharacterId;
		this.slotIndex = newSkillIndex;

		if (this.popupCharactersGenerated == false) {
			this.popup_Characters.GetComponent<PopupGenerator> ().GenerateCharactersPopup ();
			this.popupCharactersGenerated = true;
		}
		this.popup_Characters.GetComponent<PopupGenerator> ().FilterCharacters (newCharacterType);

		this.ShowPopup (2);
	}
	#endregion
		
	#region Prepare Batiment Panel
	public void PrepareFightPanel () {
		this.panel_Fight.GetComponent<FightPanel> ().Init ();
		this.HidePopup (4);
		this.panel_Fight.SetActive (true);
	}

	public void PrepareUpgradePanel () {
		this.panel_Upgrade.GetComponent<UpgradePanel> ().Init ();
		this.HidePopup (5);
		this.panel_Upgrade.SetActive (true);
	}

	public void PrepareShopPanel () {
		this.panel_Shop.GetComponent<ShopPanel> ().Init ();
		this.HidePopup (6);
		this.panel_Shop.SetActive (true);
	}

	public void PrepareAttributionPanel (int newType) {
		this.panel_ItemAttribution.GetComponent<TeamPanel> ().Init (newType);
		this.ShowPopup (7);
	}

	public void PrepareInn () {
		//print (CampaignManager.playerTeam.mainSummoner.pv.ToString ());
		CampaignManager.Rest ();
		//print (CampaignManager.playerTeam.mainSummoner.pv.ToString ());
		this.HidePopup (8);
	}

	public void PrepareBoss () {

		CampaignManager.isBossFight = true;
		CampaignManager.combatEnded = false;
		CampaignManager.SaveCampaign ();

		EnemyTeam monsterTeamToFight = new EnemyTeam ();
		monsterTeamToFight.enemies = new SummonerData[CampaignManager.finalBossTeam.group.Length];

		for (int i = 0; i < CampaignManager.finalBossTeam.group.Length; i++) {
			SummonerData enemyTemp = new SummonerData();

			for (int j = 0; j < CampaignManager.L_Enemies.Count; j++) {
				if (CampaignManager.L_Enemies [j].id == CampaignManager.finalBossTeam.group [i]) {
					enemyTemp = CampaignManager.L_Enemies [j];
				}
			}

			monsterTeamToFight.enemies [i] = enemyTemp;
		}

		JsonSave.SaveEnemyTeam (monsterTeamToFight);

		SceneManager.LoadScene ("combat", LoadSceneMode.Single);

	}
	#endregion

	#region Change Action / Talent / Character
	public void ChangeAction (int newActionId) {
		if (E_ChangeSkill != null) {
			E_ChangeSkill (this.teamId, this.characterType, this.characterId, this.slotIndex, newActionId);
			print (this.teamId + " / " + this.characterType + " / " +  this.characterId + " / " +  this.transform.GetSiblingIndex() + " / " +  newActionId);
		}
	} 

	public void ChangeTalent (int newEtatId) {
		if (E_ChangeTalent != null) {
			E_ChangeTalent (this.teamId, this.characterType, this.characterId, this.slotIndex, newEtatId);
			print (this.teamId + " / " + this.characterType + " / " +  this.characterId + " / " +  this.transform.GetSiblingIndex() + " / " +  newEtatId);
		}
	} 

	public void ChangeCharacter (int newCharacterId) {
		if (E_ChangeCharacter != null) {
			E_ChangeCharacter (this.teamId, this.characterType, this.characterId, this.slotIndex, newCharacterId);
			print (this.teamId + " / " + this.characterType + " / " +  this.characterId + " / " +  this.transform.GetSiblingIndex() + " / " +  newCharacterId);
		}
	}
	#endregion

	#region Buy Action / Item from batiment
	public void ActionBought (PopupElement newPopupElement) {
		CampaignManager.ChangePlayerAction (newPopupElement.characterType, newPopupElement.characterId, newPopupElement.teamId);
		this.panel_Shop.GetComponent<ShopPanel> ().Init ();
		this.panel_Shop.GetComponent<ShopPanel> ().CardBought (newPopupElement);
		this.HidePopup (7);
	}

	public void ItemBought (PopupElement newPopupElement) {
		CampaignManager.BuyItem (newPopupElement.characterType, newPopupElement.characterId, newPopupElement.teamId);
		this.panel_Shop.GetComponent<ShopPanel> ().Init ();
		this.panel_Shop.GetComponent<ShopPanel> ().ItemBought (newPopupElement);
		this.HidePopup (7);
	}
	#endregion

	#region Fonction de Show / Hide popup
	public void ShowPopup (int newPopupType) {
		this.popupMask.SetActive (true);
		if (newPopupType == 1) {
			this.popup_Actions.SetActive (true);
		} else if (newPopupType == 2) {
			this.popup_Characters.SetActive (true);
		} else if (newPopupType == 3) {
			this.popup_Talents.SetActive (true);
		} else if (newPopupType == 4) {
			this.popup_Chasse.SetActive (true);
		} else if (newPopupType == 5) {
			this.popup_Upgrade.SetActive (true);
		} else if (newPopupType == 6) {
			this.popup_Shop.SetActive (true);
		} else if (newPopupType == 7) {
			this.panel_ItemAttribution.SetActive (true);
		} else if (newPopupType == 8) {
			this.popup_Inn.SetActive (true);
		} else if (newPopupType == 9) {
			this.popup_Boss.SetActive (true);
		}
	}

	public void HidePopup (int newPopupType) {
		this.popupMask.SetActive (false);
		if (newPopupType == 1 && this.popup_Actions != null) {
			this.popup_Actions.SetActive (false);
		} else if (newPopupType == 2 && this.popup_Characters != null) {
			this.popup_Characters.SetActive (false);
		} else if (newPopupType == 3 && this.popup_Talents != null) {
			this.popup_Talents.SetActive (false);
		} else if (newPopupType == 4 && this.popup_Chasse != null) {
			this.popup_Chasse.SetActive (false);
		} else if (newPopupType == 5 && this.popup_Upgrade != null) {
			this.popup_Upgrade.SetActive (false);
		} else if (newPopupType == 6 && this.popup_Shop != null) {
			this.popup_Shop.SetActive (false);
		} else if (newPopupType == 7 && this.panel_ItemAttribution != null) {
			this.panel_ItemAttribution.SetActive (false);
		} else if (newPopupType == 8 && this.popup_Inn != null) {
			this.popup_Inn.SetActive (false);
		} else if (newPopupType == 9 && this.popup_Boss != null) {
			this.popup_Boss.SetActive (false);
		}
	}

	public void HideBatiment (int newBatimentType) {
		if (newBatimentType == 1 && this.panel_Fight != null) {
			this.panel_Fight.SetActive (false);
		} else if (newBatimentType == 2 && this.panel_Upgrade != null) {
			this.panel_Upgrade.SetActive (false);
		} else if (newBatimentType == 3 && this.panel_Shop != null) {
			this.panel_Shop.SetActive (false);
		}
	}

	public void HideAllBatiment () {
		if (this.panel_Fight != null) {
			this.panel_Fight.SetActive (false);
		}
		if (this.panel_Upgrade != null) {
			this.panel_Upgrade.SetActive (false);
		}
		if (this.panel_Shop != null) {
			this.panel_Shop.SetActive (false);
		}
	}
	#endregion

	#region Show Card / Etat
	public void ShowCard (GameObject newObjectPos, ActionsData newAction) {
		this.popup_Card.transform.position = new Vector3 (newObjectPos.transform.position.x + 300f, newObjectPos.transform.position.y, newObjectPos.transform.position.z);
		this.popup_Card.GetComponent<PopupGenerator> ().GenerateCard (newAction);
		this.GenerateKeywords (this.popup_Card);
		this.popup_Card.SetActive (true);
	}

	public void HideCard () {
		this.popup_Card.SetActive (false);	
		this.DestroyKeywords ();
	}

	public void GenerateKeywords (GameObject newGameObject) {
		this.DestroyKeywords ();

		this.keywordsContainer.GetComponent<KeywordsPanel> ().Init (newGameObject);
		this.keywordsContainer.SetActive (true);

		print ("je genere le keyword");
	}

	public void DestroyKeywords () {
		this.keywordsContainer.GetComponent<KeywordsPanel> ().DestroyKeywords ();
		this.keywordsContainer.SetActive (false);
	}

	public void ShowEtat (GameObject newObjectPos, EtatsData newEtat) {
		this.popup_Etat.transform.position = new Vector3 (newObjectPos.transform.position.x, newObjectPos.transform.position.y + 100f, newObjectPos.transform.position.z);
		this.popup_Etat.GetComponent<PopupGenerator> ().GenerateEtat (newEtat);
		this.popup_Etat.SetActive (true);

	}

	public void HideEtat () {
		this.popup_Etat.SetActive (false);		
	}
	#endregion
}
