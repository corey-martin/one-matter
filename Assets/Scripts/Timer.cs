using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	private Text timerText;
	[HideInInspector] public bool isPlaying = false;
	[HideInInspector] public float timePassed;
    [HideInInspector] public float timeToIgnore = 0;

    void Awake() {
		timerText = GetComponent<Text>();
		isPlaying = false;
    }
	
	void Update () {
		timePassed = Time.realtimeSinceStartup - timeToIgnore;
		if (isPlaying) {
	        timerText.text = timePassed.ToString();
	    }
	}

	public void BestTime() {
		timerText.text = "Best Time: \n" + PlayerPrefs.GetFloat("BestTime").ToString();
	}
}
