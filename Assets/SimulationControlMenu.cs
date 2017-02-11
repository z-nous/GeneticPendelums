using UnityEngine;
using System.Collections;

public class SimulationControlMenu : MonoBehaviour {
    public Transform ActiveLocation;
    public Transform DeactiveLocation;

    public bool Isactive = false;

    public void Change()
    {
        Isactive = !Isactive;
        if (Isactive == true) gameObject.transform.localPosition = ActiveLocation.localPosition;
        else gameObject.transform.localPosition = DeactiveLocation.localPosition;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
