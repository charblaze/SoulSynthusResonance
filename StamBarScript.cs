using UnityEngine;
using System.Collections;

public class StamBarScript : MonoBehaviour {
    Player p;
    RectTransform bg, hb, ov;

    // Use this for initialization
    void Start()
    {
        p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        bg = transform.GetChild(0).GetComponent<RectTransform>();
        hb = transform.GetChild(1).GetComponent<RectTransform>();
        ov = transform.GetChild(2).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        bg.sizeDelta = new Vector2(p.MaximumStamina * 2f , 46f);
        ov.sizeDelta = new Vector2(p.MaximumStamina * 2f , 58f);
        hb.sizeDelta = new Vector2(p.CurrentStamina * 2f, 47.1f);
        //r.sizeDelta = new Vector2(p.CurrentHealth, 20);
    }
}
