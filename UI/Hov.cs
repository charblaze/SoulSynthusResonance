using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Hov : MonoBehaviour, IPointerEnterHandler {
    public string ID;
    Descriptor d;
	// Use this for initialization
	void Start () {
        d = GameObject.FindGameObjectWithTag("Descriptor").GetComponent<Descriptor>();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        d.ID = ID;
        d.DOIT();
    }

	// Update is called once per frame
	void Update () {
	
	}
}
