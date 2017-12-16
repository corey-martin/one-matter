using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAudio : MonoBehaviour {

	public void ToggleSound() {
		AudioListener.pause = !AudioListener.pause;
	}
}
