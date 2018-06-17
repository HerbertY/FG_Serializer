using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Image image = GetComponent<Image>();
        image.RecalculateClipping();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
