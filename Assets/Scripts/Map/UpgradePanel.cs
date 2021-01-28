using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour {

	public GameObject mainSummonerPanel;
	public GameObject supportPanel_1;
	public GameObject supportPanel_2;

	public GameObject forcePanel;
	public GameObject pvPanel;
	public GameObject armurePanel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init () {
		this.UpdateStatsUpgrades ();
		this.UpdateCharactersTier ();
	}

	public void UpdateStatsUpgrades () {
		this.pvPanel.transform.GetChild (2).GetComponent<Text>().text = CampaignManager.playerTeam.mainSummoner.pvMax + "   >   " + (CampaignManager.playerTeam.mainSummoner.pvMax + 5);
		this.pvPanel.transform.GetChild (3).GetChild (1).GetComponent<Text> ().text = "150";

		this.forcePanel.transform.GetChild (2).GetComponent<Text>().text = CampaignManager.playerTeam.mainSummoner.force + "   >   " + (CampaignManager.playerTeam.mainSummoner.force + 1);
		this.forcePanel.transform.GetChild (3).GetChild (1).GetComponent<Text> ().text = "150";

		this.armurePanel.transform.GetChild (2).GetComponent<Text>().text = CampaignManager.playerTeam.mainSummoner.armure + "   >   " + (CampaignManager.playerTeam.mainSummoner.armure + 1);
		this.armurePanel.transform.GetChild (3).GetChild (1).GetComponent<Text> ().text = "150";

		if (CampaignManager.gold < 150) {
			this.pvPanel.GetComponent<Button> ().enabled = false;
			this.pvPanel.transform.GetChild (3).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor(1);

			this.forcePanel.GetComponent<Button> ().enabled = false;
			this.forcePanel.transform.GetChild (3).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor(1);

			this.armurePanel.GetComponent<Button> ().enabled = false;
			this.armurePanel.transform.GetChild (3).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor(1);

		} else {
			this.pvPanel.GetComponent<Button> ().enabled = true;
			this.pvPanel.transform.GetChild (3).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor(2);

			this.forcePanel.GetComponent<Button> ().enabled = true;
			this.forcePanel.transform.GetChild (3).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor(2);

			this.armurePanel.GetComponent<Button> ().enabled = true;
			this.armurePanel.transform.GetChild (3).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor(2);

		}
	}

	public void UpdateCharactersTier () {

		this.supportPanel_1.SetActive (false);
		this.supportPanel_2.SetActive (false);

		this.mainSummonerPanel.transform.GetChild (0).GetComponent<Text> ().text = "Tier. " + CampaignManager.playerTeam.mainSummoner.tier.ToString ();
		this.mainSummonerPanel.transform.GetChild (1).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("character_avatars/" + CampaignManager.playerTeam.mainSummoner.idAvatar);
		if ((CampaignManager.playerTeam.mainSummoner.tier + 1) % 3 == 0) {
			//this.mainSummonerPanel.transform.GetChild (3).GetComponent<Text> ().text = CampaignManager.playerTeam.mainSummoner.nbDiceMax.ToString () + "   >   " + (CampaignManager.playerTeam.mainSummoner.nbDiceMax + 1);
			this.mainSummonerPanel.transform.GetChild (3).GetComponent<Text> ().text = CampaignManager.playerTeam.mainSummoner.nbDiceMax.ToString () + "   >   " + (CampaignManager.playerTeam.mainSummoner.nbDiceMax + 1);
			this.mainSummonerPanel.transform.GetChild (5).GetComponent<Text> ().text = CampaignManager.playerTeam.mainSummoner.nbSkillSlots.ToString () + "   >   " + (CampaignManager.playerTeam.mainSummoner.nbSkillSlots + 1);
		} else {
			this.mainSummonerPanel.transform.GetChild (3).GetComponent<Text> ().text = CampaignManager.playerTeam.mainSummoner.nbDiceMax.ToString ();
			this.mainSummonerPanel.transform.GetChild (5).GetComponent<Text> ().text = CampaignManager.playerTeam.mainSummoner.nbSkillSlots.ToString () + "   >   " + (CampaignManager.playerTeam.mainSummoner.nbSkillSlots + 1);
		}

		this.mainSummonerPanel.transform.GetChild (6).GetChild (1).GetComponent<Text> ().text = (3 * CampaignManager.playerTeam.mainSummoner.tier).ToString ();

		if (CampaignManager.soul < (3 * CampaignManager.playerTeam.mainSummoner.tier)) {
			this.mainSummonerPanel.GetComponent<Button> ().enabled = false;
			this.mainSummonerPanel.transform.GetChild (6).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor(1);
		} else {
			this.mainSummonerPanel.GetComponent<Button> ().enabled = true;
			this.mainSummonerPanel.transform.GetChild (6).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor(2);
		}

		for (int i = 0; i < CampaignManager.playerTeam.supports.Length; i++) {
			SupportData supportScript;
			if (i == 0) {
				supportScript = CampaignManager.playerTeam.supports [i];

				this.supportPanel_1.transform.GetChild (0).GetComponent<Text> ().text = "Tier. " + supportScript.tier.ToString ();
				this.supportPanel_1.transform.GetChild (1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("character_avatars/" + supportScript.idAvatar);

				if ((supportScript.tier + 1) % 3 == 0) {			
					this.supportPanel_1.transform.GetChild (3).GetComponent<Text> ().text = supportScript.nbDiceMax.ToString () + "   >   " + (supportScript.nbDiceMax + 1);
					this.supportPanel_1.transform.GetChild (5).GetComponent<Text> ().text = supportScript.nbSkillSlots.ToString () + "   >   " + (supportScript.nbSkillSlots + 1);
				} else {
					this.supportPanel_1.transform.GetChild (3).GetComponent<Text> ().text = supportScript.nbDiceMax.ToString ();
					this.supportPanel_1.transform.GetChild (5).GetComponent<Text> ().text = supportScript.nbSkillSlots.ToString () + "   >   " + (supportScript.nbSkillSlots + 1);
				}

				this.supportPanel_1.transform.GetChild (6).GetChild (1).GetComponent<Text> ().text = (3 * supportScript.tier).ToString ();

				if (CampaignManager.soul < (3 * supportScript.tier)) {
					this.supportPanel_1.GetComponent<Button> ().enabled = false;
					this.supportPanel_1.transform.GetChild (6).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor(1);
				} else {
					this.supportPanel_1.GetComponent<Button> ().enabled = true;
					this.supportPanel_1.transform.GetChild (6).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor(2);
				}

				this.supportPanel_1.SetActive (true);
			} else {
				supportScript = CampaignManager.playerTeam.supports [i];

				this.supportPanel_2.transform.GetChild (0).GetComponent<Text> ().text = "Tier. " + supportScript.tier.ToString ();
				this.supportPanel_2.transform.GetChild (1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("character_avatars/" + supportScript.idAvatar);

				if ((supportScript.tier + 1) % 3 == 0) {			
					this.supportPanel_2.transform.GetChild (3).GetComponent<Text> ().text = supportScript.nbDiceMax.ToString () + "   >   " + (supportScript.nbDiceMax + 1);
					this.supportPanel_2.transform.GetChild (5).GetComponent<Text> ().text = supportScript.nbSkillSlots.ToString () + "   >   " + (supportScript.nbSkillSlots + 1);
				} else {
					this.supportPanel_2.transform.GetChild (3).GetComponent<Text> ().text = supportScript.nbDiceMax.ToString ();
					this.supportPanel_2.transform.GetChild (5).GetComponent<Text> ().text = supportScript.nbSkillSlots.ToString () + "   >   " + (supportScript.nbSkillSlots + 1);
				}
					
				this.supportPanel_2.transform.GetChild (6).GetChild (1).GetComponent<Text> ().text = (3 * supportScript.tier).ToString ();

				if (CampaignManager.soul < (3 * supportScript.tier)) {
					this.supportPanel_2.GetComponent<Button> ().enabled = false;
					this.supportPanel_2.transform.GetChild (6).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor(1);
				} else {
					this.supportPanel_2.GetComponent<Button> ().enabled = true;
					this.supportPanel_2.transform.GetChild (6).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor(2);
				}

				this.supportPanel_2.SetActive (true);
			}
		}
	}

	public void UpdateCharacterStats (int newStatsId) {

		if (newStatsId == 1) {
			CampaignManager.playerTeam.mainSummoner.pvMax += 5;
		} else if (newStatsId == 2) {
			CampaignManager.playerTeam.mainSummoner.force += 1;		
		} else if (newStatsId == 3) {
			CampaignManager.playerTeam.mainSummoner.armure += 1;		
		}

		CampaignManager.SaveCampaign ();
		CampaignManager.daysToRemove = 1;
		print ("j'ai modif le character");
	}

	public void UpdateCharacterTier (int newCharacterId) {

		if (newCharacterId == 1) {
			CampaignManager.itemPrice = 3 * CampaignManager.playerTeam.mainSummoner.tier;

			if ((CampaignManager.playerTeam.mainSummoner.tier + 1) % 3 == 0) {
				CampaignManager.playerTeam.mainSummoner.nbDiceMax += 1;
				CampaignManager.playerTeam.mainSummoner.nbSkillSlots += 1;
				CampaignManager.playerTeam.mainSummoner.L_skills.Add (1);
				CampaignManager.playerTeam.mainSummoner.tier += 1;
			} else {
				CampaignManager.playerTeam.mainSummoner.nbSkillSlots += 1;
				CampaignManager.playerTeam.mainSummoner.L_skills.Add (1);
				CampaignManager.playerTeam.mainSummoner.tier += 1;				
			}
		} else if (newCharacterId == 2) {
			CampaignManager.itemPrice = 3 * CampaignManager.playerTeam.supports[0].tier;

			if ((CampaignManager.playerTeam.supports[0].tier + 1) % 3 == 0) {
				CampaignManager.playerTeam.supports[0].nbDiceMax += 1;
				CampaignManager.playerTeam.supports[0].nbSkillSlots += 1;
				CampaignManager.playerTeam.supports[0].L_skills.Add (1);
				CampaignManager.playerTeam.supports[0].tier += 1;
			} else {
				CampaignManager.playerTeam.supports[0].nbSkillSlots += 1;
				CampaignManager.playerTeam.supports[0].L_skills.Add (1);
				CampaignManager.playerTeam.supports[0].tier += 1;				
			}
		} else if (newCharacterId == 3) {
			CampaignManager.itemPrice = 3 * CampaignManager.playerTeam.supports[1].tier;

			if ((CampaignManager.playerTeam.supports[1].tier + 1) % 3 == 0) {
				CampaignManager.playerTeam.supports[1].nbDiceMax += 1;
				CampaignManager.playerTeam.supports[1].nbSkillSlots += 1;
				CampaignManager.playerTeam.supports[1].L_skills.Add (1);
				CampaignManager.playerTeam.supports[1].tier += 1;
			} else {
				CampaignManager.playerTeam.supports[1].nbSkillSlots += 1;
				CampaignManager.playerTeam.supports[1].L_skills.Add (1);
				CampaignManager.playerTeam.supports[1].tier += 1;				
			}
		}

		CampaignManager.SaveCampaign ();
		CampaignManager.daysToRemove = 1;
		//print ("j'ai modif le character");
	}
}
