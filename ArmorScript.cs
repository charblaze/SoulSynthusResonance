using UnityEngine;
using System.Collections;

public class ArmorScript : MonoBehaviour
{
    public string name;
    public float weight;
    public int healthbonus;
    public float stam, healthregen, stamregen, maxequip, runspeed, poisres, ssres, tenacity, swiftness, sp, ter, lur, sor, inr, chr, lir, der;

    GameObject player;
    Player p;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        p = player.GetComponent<Player>();

        p.EQUIPLOAD_armorbonus = weight;
        p.HEALTH_armorbonus = healthbonus;
        p.STAMINA_armorbonus = stam;
        p.HEALTHREGENarmorbonus = healthregen;
        p.STAMINAREGEN_armorbonus = stamregen;
        p.MAXEQUIPLOAD_armorbonus = maxequip;
        p.RUNSPEED_armorbonus = runspeed;
        p.POISONRESIST_armorbonus = poisres;
        p.SSRESIST_armorbonus = ssres;
        p.TENACITY_armorbonus = tenacity;
        p.SWIFNTESS_armorbonus = swiftness;
        p.TRESIST_armorbonus = ter;
        p.SORESIST_armorbonus = sor;
        p.LURESIST_armorbonus = lur;
        p.INRESIST_armorbonus = inr;
        p.CHRESIST_armorbonus = chr;
        p.LIRESIST_armorbonus = lir;
        p.DERESIST_armorbonus = der;
        p.SPELLPOWER_armorbonus = sp;
        p.RecalculateStats();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
