using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenameRobot : MonoBehaviour {

    public Text nameText;

    public void Rename(string s){
        nameText.text = s;
    }
}
