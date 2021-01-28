using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DamageAnimation(int newDamageType, int newDamage) {
		this.GetComponent<Text> ().text = newDamage.ToString ();
		this.GetComponent<Text> ().color = this.DamageColor (newDamageType);
		this.GetComponent<Outline> ().effectColor = this.DamageOutlineColor (newDamageType);
		StartCoroutine (this.StartAnimation());
	}

	public IEnumerator StartAnimation() {
		iTween.MoveTo (this.gameObject, iTween.Hash(
			"position", new Vector3 (this.transform.position.x, this.transform.position.y + 200f, this.transform.position.z), 
			"time", 2f, 
			"easetype", iTween.EaseType.easeInCirc));
		this.GetComponent<Text> ().CrossFadeAlpha (0, 2f, false);
		yield return new WaitForSeconds (2f);
		Destroy (this.gameObject);
	}

	private Color32 DamageColor (int newDamageType) {
		switch (newDamageType) {
		case 1: // c'est un degats
			return new Color32 (255, 30, 30, 255);
		case 2: // c'est un heal
			return new Color32 (41, 207, 48, 255);
		default:
			return new Color32 (142, 142, 142, 255);
		}
	}
	private Color32 DamageOutlineColor (int newDamageType) {
		switch (newDamageType) {
		case 1: // c'est un degats
			return new Color32 (192, 0, 0, 153);
		case 2: // c'est un heal
			return new Color32 (25, 121, 40, 153);
		default:
			return new Color32 (142, 142, 142, 153);
		}
	}
}
