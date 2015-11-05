using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {
    /*
        This component contains all info on the player's current inventory.
    */

    public List<Weapon> Weapons = new List<Weapon>();
    public List<Hat> Hats = new List<Hat>();
    public List<Amulet> Amulets = new List<Amulet>();
    public List<Tunic> Tunics = new List<Tunic>();
    public List<Ring> Rings = new List<Ring>();
    public List<Armor> Armors = new List<Armor>();
    public List<Cape> Capes = new List<Cape>();
    PlayerMovement pm;
    Player p;

    public struct Armor
    {
        public string ID;
        public string Description;
    }

    public void AddArmor(string id)
    {
        Armor x = new Armor();
        x.ID = id;
        x.Description = "An Armor Set";
        Armors.Add(x);
    }

    public struct Cape
    {
        public string ID;
        public string Description;
    }

    public void AddCape(string id)
    {
        Cape x = new Cape();
        x.ID = id;
        x.Description = "A cape";
        Capes.Add(x);
    }

    public struct Amulet
    {
        public string ID;
        public string Description;
    }

    public void AddAmulet(string id)
    {
        Amulet x = new Amulet();
        x.ID = id;
        x.Description = "An ammy";
        Amulets.Add(x);
    }

    public struct Hat
    {
        public string ID;
        public string Description;
    }

    public void AddHat(string id)
    {
        Hat x = new Hat();
        x.ID = id;
        x.Description = "A HAT";
        Hats.Add(x);
    }

    public struct Tunic
    {
        public string ID;
        public string Description;
    }

    public void AddTunic(string id)
    {
        Tunic x = new Tunic();
        x.ID = id;
        x.Description = "A tunic";
        Tunics.Add(x);
    }

    public struct Ring
    {
        public string ID;
        public string Description;
    }

    public void AddRing(string id)
    {
        Ring x = new Ring();
        x.ID = id;
        x.Description = "A tunic";
        Rings.Add(x);
    }


    // 
    public struct Weapon
    {
        public string WeaponID;
        public int UpgradeLevel;
        public string Description;
    }
    
    public string GetWeaponDescription(string id)
    {
        switch (id)
        {
            case "Zweihander": return "A big ol sword";
            case "Edged Rapier": return "a rapier";
            case "Great Scythe": return "a big ol scythe";
        }
        return "A great weapon.";
    }

    public void AddWeapon(string id, int upgradelvl = 0)
    {
        Weapon x = new Weapon();
        x.WeaponID = id;
        x.UpgradeLevel = upgradelvl;
        x.Description = GetWeaponDescription(id);
        Weapons.Add(x);
    }

    public void DropWeapon(int loc)
    {
        Weapons.RemoveAt(loc);
    }

    // deprecated
    public void EquipWeaponToMain(int loc)
    {
        p.EquippedWeapon = Weapons[loc].WeaponID;
        pm.WeaponChanged();
    }


	// Use this for initialization
	void Start () {
        pm = GetComponent<PlayerMovement>();
        p = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
