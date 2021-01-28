using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {

	public GameObject mainSummonerPanel;
	public GameObject supportPanel_1;
	public GameObject supportPanel_2;

	public GameObject blackScreen;
	public GameObject changementDeJourGO;

	public Text goldText;
	public Text soulText;
	public Text dayText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init () {
		this.UpdateInfos();

		this.GenerateSummonerPanel (CampaignManager.playerTeam.mainSummoner);

		this.supportPanel_1.SetActive (false);
		this.supportPanel_2.SetActive (false);

		for (int i = 0; i < CampaignManager.playerTeam.supports.Length; i++) {
			if (i == 0) {
				this.supportPanel_1.SetActive (true);
			} else {
				this.supportPanel_2.SetActive (true);
			}

			this.GenerateSupportPanel (CampaignManager.playerTeam.supports[i], i);
		}
	}

	#region Generate Summoner / Support Panel
	public void GenerateSummonerPanel (SummonerData newSummoner) {
		this.mainSummonerPanel.GetComponent<CharacterMapInfosPanel> ().InitSummonerInfos (newSummoner);
	}

	public void GenerateSupportPanel (SupportData newSupport, int newId) {
		if (newId == 0) {
			this.supportPanel_1.GetComponent<CharacterMapInfosPanel> ().InitSupportInfos (newSupport);
		} else {
			this.supportPanel_2.GetComponent<CharacterMapInfosPanel> ().InitSupportInfos (newSupport);			
		}
	}
	#endregion

	public void UpdateInfos () {
		this.goldText.text = CampaignManager.gold.ToString ();
		this.soulText.text = CampaignManager.soul.ToString ();
		this.dayText.text = CampaignManager.day.ToString ();

		if (this.changementDeJourGO != null) {
			this.changementDeJourGO.transform.GetChild (0).GetComponent<Text> ().text = CampaignManager.day.ToString ();
		}
	}

	public IEnumerator AddGold () {
		int goldValue = CampaignManager.gold;
		int goldReward = CampaignManager.goldReward;
		CampaignManager.AddGoldReward ();
		for (int i = 0; i < goldReward; i++) {
			goldValue += 1;
			this.goldText.text = goldValue.ToString ();
			yield return new WaitForSeconds (0.02f);
		}
	}

	public IEnumerator AddSoul () {
		int soulValue = CampaignManager.soul;
		int soulReward = CampaignManager.soulReward;
		CampaignManager.AddSoulReward ();
		for (int i = 0; i < soulReward; i++) {
			soulValue += 1;
			this.soulText.text = soulValue.ToString ();
			yield return new WaitForSeconds (0.05f);
		}
	}

	public void CardBought (PopupElement newPopupElement) {
		if (newPopupElement.characterType == 1) {
			this.GenerateSummonerPanel (CampaignManager.playerTeam.mainSummoner);
		} else {
			this.GenerateSupportPanel (CampaignManager.playerTeam.supports [newPopupElement.characterId], newPopupElement.characterId);
		}
	}

	public void ItemBought (PopupElement newPopupElement) {
		if (newPopupElement.characterType == 1) {
			this.GenerateSummonerPanel (CampaignManager.playerTeam.mainSummoner);
		} else {
			this.GenerateSupportPanel (CampaignManager.playerTeam.supports [newPopupElement.characterId], newPopupElement.characterId);
		}
	}

	public void StartLooseGoldAnimItem () {
		StartCoroutine (this.LooseGold (CampaignManager.itemPrice));
	}
	public void StartLooseGoldAnim (int newInt) {
		StartCoroutine (this.LooseGold (newInt));
	}

	public void StartLooseSoulAnimItem () {
		StartCoroutine (this.LooseSoul (CampaignManager.itemPrice));
	}
	public void StartLooseSoulAnim (int newInt) {
		StartCoroutine (this.LooseSoul (newInt));
	}

	public IEnumerator LooseGold (int newGoldAmount) {
		int goldValue = CampaignManager.gold;
		int goldReward = newGoldAmount;
		CampaignManager.gold -= newGoldAmount;

		for (int i = 0; i < goldReward; i++) {
			goldValue -= 1;
			this.goldText.text = goldValue.ToString ();
			yield return new WaitForSeconds (0.05f);
		}
	}

	public IEnumerator LooseSoul (int newSoulAmount) {
		int soulValue = CampaignManager.soul;
		int soulReward = newSoulAmount;
		CampaignManager.soul -= newSoulAmount;

		for (int i = 0; i < soulReward; i++) {
			soulValue -= 1;
			this.soulText.text = soulValue.ToString ();
			yield return new WaitForSeconds (0.05f);
		}
	}

	public IEnumerator ShowChangementDeJour () {
		//print ("je show le changement de jour");
		this.changementDeJourGO.SetActive (true);
		this.changementDeJourGO.transform.GetChild(0).GetComponent<Text> ().canvasRenderer.SetAlpha( 0.0f );
		this.changementDeJourGO.transform.GetChild(0).GetComponent<Text> ().CrossFadeAlpha (1f, 0.2f, false);
		this.changementDeJourGO.transform.GetChild(1).GetComponent<Text> ().canvasRenderer.SetAlpha( 0.0f );
		this.changementDeJourGO.transform.GetChild(1).GetComponent<Text> ().CrossFadeAlpha (1f, 0.2f, false);
		yield return new WaitForSeconds (0.3f);
	}
	public IEnumerator HideChangementDeJour () {
		this.changementDeJourGO.transform.GetChild(0).GetComponent<Text> ().CrossFadeAlpha (0f, 0.5f, false);
		this.changementDeJourGO.transform.GetChild(1).GetComponent<Text>  ().CrossFadeAlpha (0f, 0.5f, false);
		yield return new WaitForSeconds (0.5f);
		this.changementDeJourGO.SetActive (false);
	}

	public IEnumerator ShowBlackScreen () {
		this.blackScreen.SetActive (true);
		this.blackScreen.GetComponent<Image> ().canvasRenderer.SetAlpha( 0.0f );
		this.blackScreen.GetComponent<Image> ().CrossFadeAlpha (1f, 0.5f, false);
		yield return new WaitForSeconds (0.5f);
	}
	public IEnumerator HideBlackScreen () {
		this.blackScreen.SetActive (true);
		this.blackScreen.GetComponent<Image> ().CrossFadeAlpha (0f, 0.5f, false);
		yield return new WaitForSeconds (0.5f);
		this.blackScreen.SetActive (false);
	}
}