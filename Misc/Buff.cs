using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// can be extended to make unique effects
public class Buff : MonoBehaviour {

    public string Title;
    public Sprite Icon;
    public string Description;
    public bool runsout = false;
    public float Duration = 0f;

    public int health;
    public float stambonus, healthregenbonus, stamregenbonus, equiploadbonus, maxequiploadbonus, runspeedbonus, poisresistbonus, ssresistbonus, tenacitybonus, swiftnessbonus, ter, lur, sor, chres, lires, inres, deres, spellpower;
    BuffDebuffHandler bdh;

    bool notstacking = false;

    public Text buffui;



    // Use this for initialization
    void Start()
    {
        bdh = GameObject.FindGameObjectWithTag("Player").GetComponent<BuffDebuffHandler>();
        if (bdh.CheckBuff(Title))
        {
            notstacking = true;
            Destroy(gameObject);
        }
        else
        {
            bdh.ApplyBuff(Title, Description, Icon, health, stambonus, healthregenbonus, stamregenbonus, equiploadbonus, maxequiploadbonus, runspeedbonus, poisresistbonus, ssresistbonus, tenacitybonus, swiftnessbonus, ter, lur, sor, chres, lires, inres, deres, spellpower, this);
        }
    }
	
    void OnDestroy()
    {
        if (!notstacking)
        {
            bdh.RemoveBuff(Title);
        }
        if(buffui != null)
        {
            Destroy(buffui.transform.parent.gameObject);
        }
    }

	// Update is called once per frame
	void Update () {
	    if(runsout)
        {
            if(Duration <= 0)
            {
                Destroy(gameObject);
            }
            Duration -= Time.deltaTime;
            if(buffui != null)
            {
                buffui.text = Duration.ToString("F1");
            }
        }
	}
}
