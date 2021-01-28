using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Support : MonoBehaviour {

	#region Id Variables
	public int id;
	public int idOriginal;
	public string supportName;
	public string idAvatar;
	public SpriteRenderer avatarImg;
	public int characterType;
	#endregion

	#region Upgrade Variables
	public int tier; 
	#endregion

	#region Dice Variables
	public int nbDiceMax;
	public int nbDice;
	public List<Dice> L_dices = new List<Dice>();
	public int[] L_dicesValue;
	#endregion

	#region Skills Variables
	public int nbSkillSlots;
	public int nbItemSlots;
	public List<int> L_skills = new List<int>();
	#endregion

	#region Etats Variables
	public List<int> L_talents = new List<int> ();
	public List<EtatsData> L_etats = new List<EtatsData> ();
	#endregion

	#region CharacterInfos Variables
	public GameObject charactersInfosPanel; 
	#endregion

	void OnEnable()
	{
	}


	void OnDisable()
	{
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {		
	}

	public void Init () {
		this.GetComponent<ActionManager> ().LoadTalents ();
		this.SetDices ();
	}

	#region Gestion des Dices
	private void SetDices () {
		for (int i = 0; i < this.nbDice; i++) {
			Dice dice = new Dice (this.gameObject, i, this.nbSkillSlots, 2);
			this.L_dices.Add (dice);
		}
		this.L_dicesValue = new int[this.L_dices.Count];
	}
	#endregion

	public void EndCombat () {
		Destroy (this.charactersInfosPanel);
	}

	public void CallUIAddEtat (EtatsData newEtat, int newInt) { // 1 je add / 2 jupdate / 3 je remove
		if (newInt == 1) {
			this.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().AddEtat (newEtat);
		} else if (newInt == 2) {
			this.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().UpdateEtat (newEtat);
		} else if (newInt == 3) {
			this.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().RemoveEtat (newEtat);
		}
	}
	public void CallUpdateUIShowDices () {
		this.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().ShowDices ();
	}
}

