using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBankHelperObject : MonoBehaviour {
    public void PlayButtonHover() {
        SoundBankController.instance.buttonHover.Play();
    }
}
