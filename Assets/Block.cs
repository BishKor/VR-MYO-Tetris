using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	public string state; // possible states are "player", "filled", "empty"
	private Color color;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void SetState(string argState){
		state = argState;
		switch (argState)
		{
		case "empty":
			this.gameObject.GetComponent<MeshRenderer> ().sharedMaterial = Resources.Load ("Materials/blockmat", typeof(Material)) as Material;
			this.gameObject.GetComponent<MeshRenderer> ().material.SetColor ("_Color", Color.clear);
			break;
		case "filled":
			this.gameObject.GetComponent<MeshRenderer> ().sharedMaterial = Resources.Load ("Materials/MKDemoMat2View1", typeof(Material)) as Material;
			this.gameObject.GetComponent<MeshRenderer> ().material.SetColor ("_MKGlowColor", this.color);
			this.gameObject.GetComponent<MeshRenderer> ().material.SetColor ("_MKGlowTexColor", this.color);
			this.gameObject.GetComponent<MeshRenderer>().material.SetFloat("_MKGlowPower", 0.2F);
			Debug.Log (this.color);
			break;
		case "active":
			this.gameObject.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Materials/MKDemoMat2View1", typeof(Material)) as Material;
			this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", this.color);
			this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", this.color);
			this.gameObject.GetComponent<MeshRenderer>().material.SetFloat("_MKGlowPower", 1.0F);
			Debug.Log (this.color);
			break;
		}
	}

	public void SetColor(Color color){
		this.color = color;
	}
	public Color GetColor(){
		return color;
	}
	
	public string GetState(){
		return state;
	}
}
