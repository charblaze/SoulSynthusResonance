using UnityEngine;
using System.Collections;

public class HatScript : MonoBehaviour {
    public string name;
    public float weight;
    public int healthbonus;
    public float stam, healthregen, stamregen,  maxequip, runspeed, poisres, ssres, tenacity, swiftness, sp, ter, lur, sor, inr, chr, lir, der;
    public Vector3 pos;
    public Quaternion rot;

    public bool hideshair = false;

    GameObject player;
    Player p;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        p = player.GetComponent<Player>();

        p.EQUIPLOAD_hatbonus = weight;
        p.HEALTH_hatbonus = healthbonus;
        p.STAMINA_hatbonus = stam;
        p.HEALTHREGENhatbonus = healthregen;
        p.STAMINAREGEN_hatbonus = stamregen;
        p.MAXEQUIPLOAD_hatbonus = maxequip;
        p.RUNSPEED_hatbonus = runspeed;
        p.POISONRESIST_hatbonus = poisres;
        p.SSRESIST_hatbonus = ssres;
        p.TENACITY_hatbonus = tenacity;
        p.SWIFNTESS_hatbonus = swiftness;
        p.TRESIST_hatbonus = ter;
        p.SORESIST_hatbonus = sor;
        p.LURESIST_hatbonus = lur;
        p.INRESIST_hatbonus = inr;
        p.CHRESIST_hatbonus = chr;
        p.LIRESIST_hatbonus = lir;
        p.DERESIST_hatbonus = der;
        p.SPELLPOWER_hatbonus = sp;
        p.RecalculateStats();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
