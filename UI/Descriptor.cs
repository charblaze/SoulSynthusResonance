using UnityEngine;

using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Descriptor : MonoBehaviour
{
    public string ID;
    Text title, descrip;
    Image i;

    public struct s
    {
        public Sprite ICON;
        public string Description;
    }

    Dictionary<string, s> d = new Dictionary<string, s>();

    [System.Serializable]
    
    public struct ItemDescriptions
    {
        public Sprite ICON;
        public string ID;
        [TextArea(5, 10)]
        public string Description;
    }

    public ItemDescriptions[] Descriptions;

    // Use this for initialization
    void Start()
    {
        i = transform.GetChild(0).gameObject.GetComponent<Image>();
        title = transform.GetChild(1).gameObject.GetComponent<Text>();
        descrip = transform.GetChild(2).gameObject.GetComponent<Text>();

        for(int c = 0; c < Descriptions.Length; ++c)
        {
            s aaa = new s();
            aaa.ICON = Descriptions[c].ICON;
            aaa.Description = Descriptions[c].Description;
            d.Add(Descriptions[c].ID, aaa);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    

    public void DOIT()
    {
        title.text = ID;
        if (d.ContainsKey(ID))
        {
            i.sprite = d[ID].ICON;
            descrip.text = d[ID].Description;
        }
        else
        {
            descrip.text = "No Description.";
        }
    }

}