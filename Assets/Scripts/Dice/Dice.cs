using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice {

	public Summoner parentSummonerScript;
	public Support parentSupportScript;
	public int diceId;
	public int nbDiceFace;
	public int characterType; // sert a savoir si l'owner est un summoner ou un support (1 summoner, 2 support)

	private int previousDiceFace = 99;
	public int diceFace;
	public bool isBlocked = false;

	public Coroutine diceCoroutine;
	public bool isDicing = false;

	public Dice (GameObject newCharacter, int newDiceId, int newNbDiceFace, int newCharacterType) {
		this.characterType = newCharacterType;
		if (this.characterType == 1) {
			this.parentSummonerScript = newCharacter.GetComponent<Summoner> ();
		} else if (this.characterType == 2) {
			this.parentSupportScript = newCharacter.GetComponent<Support> ();
		}
		this.diceId = newDiceId;
		this.nbDiceFace = newNbDiceFace;
	}

	private void GetDice () {
		diceFace = Random.Range (0, this.nbDiceFace);
		while (previousDiceFace == diceFace) {
			diceFace = Random.Range (0, this.nbDiceFace);				
		}
		previousDiceFace = diceFace;

		if (this.characterType == 1) {
			this.parentSummonerScript.L_dicesValue [this.diceId] = this.diceFace;
			this.parentSummonerScript.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().ChangeDiceFace (this.diceId, this.diceFace);
		} else if (this.characterType == 2) {
			this.parentSupportScript.L_dicesValue [this.diceId] = this.diceFace;
			this.parentSupportScript.charactersInfosPanel.GetComponent<CharacterInfosPanel> ().ChangeDiceFace (this.diceId, this.diceFace);
		}
	}

	public IEnumerator DiceAnimation () {
		while (isDicing) {
			this.GetDice ();
			yield return new WaitForSeconds (0.1f);
		}
	}
}
