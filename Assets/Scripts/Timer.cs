using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	private Text timerText;
	[HideInInspector] public bool isPlaying = false;
	[HideInInspector] public bool isCounting = false;
	[HideInInspector] public float timePassed;

    void Awake() {
		timerText = GetComponent<Text>();
		isPlaying = false;
    }
	
	void Update () {
		if (isCounting) {
			timePassed += Time.unscaledDeltaTime;
		}
		if (isPlaying) {
	        timerText.text = timePassed.ToString();
	    }
	}

	public void BestTime(bool showCurrentTime = true) {
		if (showCurrentTime) {
			timerText.text = "Time: \n" + timePassed + "\n" + "Best Time: \n" + PlayerPrefs.GetFloat("BestTime").ToString();
		} else {
			timerText.text = "Best Time: \n" + PlayerPrefs.GetFloat("BestTime").ToString();
		}
	}
}
