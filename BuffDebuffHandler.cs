using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BuffDebuffHandler : MonoBehaviour {

    public RectTransform buffholder;

    public struct Buffa
    {
        public int healthbonus;
        public float stambonus, healthregenbonus, stamregenbonus, equiploadbonus, maxequiploadbonus, runspeedbonus, poisresistbonus, ssresistbonus, tenacitybonus, swiftnessbonus, ter, lur, sor, chres, lires, inres, deres, spellpower;
    }
    Dictionary<string, Buffa> Buffs = new Dictionary<string, Buffa>();

    public bool CheckBuff(string Title)
    {
        return Buffs.ContainsKey(Title);
    }
    

    // gets called when buffs run out or are put on player
    public void ApplyBuff(string Title, string Descrip, Sprite spr, int health, float stam, float healthregen, float stamregen, float equipload, float maxequip, float runspeed, float poisres, float ssresist, float tenacity, float swiftness, float ter, float lur, float sor, float chres, float lires, float inres, float deres, float sp, Buff bee)
    {
        // buffs of same title do not stack
        Buffa b = new Buffa();
        b.healthbonus = health;
        b.stambonus = stam; b.healthregenbonus = healthregen; b.stamregenbonus = stamregen; b.equiploadbonus = equipload; b.maxequiploadbonus = maxequip; b.runspeedbonus = runspeed; b.poisresistbonus = poisres; b.ssresistbonus = ssresist; b.tenacitybonus = tenacity; b.swiftnessbonus = swiftness; b.ter = ter; b.lur = lur; b.sor = sor; b.inres = inres; b.chres = chres; b.lires = lires; b.deres = deres; b.spellpower = sp;
        Buffs.Add(Title, b);

        p.HEALTH_buffbonus += health;
        p.STAMINA_buffbonus += stam;
        p.HEALTHREGENbuffbonus += healthregen;
        p.STAMINAREGEN_buffbonus += stamregen;
        p.EQUIPLOAD_buffbonus += equipload;
        p.MAXEQUIPLOAD_buffbonus += maxequip;
        p.RUNSPEED_buffbonus += runspeed;
        p.POISONRESIST_buffbonus += poisres;
        p.SSRESIST_buffbonus += ssresist;
        p.TENACITY_buffbonus += tenacity;
        p.SWIFNTESS_buffbonus += swiftness;
        p.TRESIST_buffbonus += ter;
        p.SORESIST_buffbonus += sor;
        p.LURESIST_buffbonus += lur;
        p.INRESIST_buffbonus += inres;
        p.CHRESIST_buffbonus += chres;
        p.LIRESIST_buffbonus += lires;
        p.DERESIST_buffbonus += deres;
        p.SPELLPOWER_buffbonus += sp;
        p.RecalculateStats();

        // UI
        GameObject bu = Instantiate(Resources.Load("UI/Buff")) as GameObject;
        bu.GetComponent<Image>().sprite = spr;
        bu.transform.GetChild(0).GetComponent<Text>().text = Descrip;
        bee.buffui = bu.transform.GetChild(1).gameObject.GetComponent<Text>();
        bu.transform.SetParent(buffholder);
    }

    public void RemoveBuff(string Title)
    {
        p.HEALTH_buffbonus -= Buffs[Title].healthbonus;
        p.STAMINA_buffbonus -= Buffs[Title].stambonus;
        p.HEALTHREGENbuffbonus -= Buffs[Title].healthregenbonus;
        p.STAMINAREGEN_buffbonus -= Buffs[Title].stamregenbonus;
        p.EQUIPLOAD_buffbonus -= Buffs[Title].equiploadbonus;
        p.MAXEQUIPLOAD_buffbonus -= Buffs[Title].maxequiploadbonus;
        p.RUNSPEED_buffbonus -= Buffs[Title].runspeedbonus;
        p.POISONRESIST_buffbonus -= Buffs[Title].poisresistbonus;
        p.SSRESIST_buffbonus -= Buffs[Title].ssresistbonus;
        p.TENACITY_buffbonus -= Buffs[Title].tenacitybonus;
        p.SWIFNTESS_buffbonus -= Buffs[Title].swiftnessbonus;
        p.TRESIST_buffbonus -= Buffs[Title].ter;
        p.SORESIST_buffbonus -= Buffs[Title].sor;
        p.LURESIST_buffbonus -= Buffs[Title].lur;
        p.INRESIST_buffbonus -= Buffs[Title].inres;
        p.CHRESIST_buffbonus -= Buffs[Title].chres;
        p.LIRESIST_buffbonus -= Buffs[Title].lires;
        p.DERESIST_buffbonus -= Buffs[Title].deres;
        p.SPELLPOWER_buffbonus -= Buffs[Title].spellpower;
        Buffs.Remove(Title);
        p.RecalculateStats();
    }


    Player p;
	// Use this for initialization
	void Start () {
        p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
