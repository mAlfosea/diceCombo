using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapElement : MonoBehaviour {

	public string batimentNameTxt;
	public string descriptionTxt; 

	public Text batimentName; 
	public Text description; 

	public GameObject descriptionGameobject;

	// Use this for initialization
	void Start () {
		this.batimentName.text = this.batimentNameTxt;
		this.description.text = this.descriptionTxt;
		this.descriptionGameobject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowDescription () {
		this.descriptionGameobject.SetActive (true);
	}
	public void HideDescription () {
		this.descriptionGameobject.SetActive (false);
	}
}
