using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour {

    Player p;
    public RectTransform bg, hb, ov;
    public Text t;

	// Use this for initialization
	void Start () {
        p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
	
	// Update is called once per frame
	void Update () {
        bg.sizeDelta = new Vector2(p.MaximumHealth / 2f, 46f);
        ov.sizeDelta = new Vector2(p.MaximumHealth / 2f, 58f);
        hb.sizeDelta = new Vector2(p.CurrentHealth / 2f, 47.1f);
        t.text = p.CurrentHealth + " mL";
        //r.sizeDelta = new Vector2(p.CurrentHealth, 20);
    }
}
