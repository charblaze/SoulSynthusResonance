using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Equipm : MonoBehaviour {
    public GameObject target;
    // Use this for initialization
    void Start() {

	}
	
    public void AssignBones()
    {
        SkinnedMeshRenderer targetRenderer = target.GetComponent<SkinnedMeshRenderer>();
        Dictionary<string, Transform> bonemap = new Dictionary<string, Transform>();
        foreach (Transform bone in targetRenderer.bones)
        {
            bonemap[bone.gameObject.name] = bone;
        }
        SkinnedMeshRenderer myRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        Transform[] newBones = new Transform[myRenderer.bones.Length];
        for (int i = 0; i < myRenderer.bones.Length; ++i)
        {
            GameObject bone = myRenderer.bones[i].gameObject;
            if (!bonemap.TryGetValue(bone.name, out newBones[i]))
            {
                Debug.Log("Unable to map bone \"" + bone.name + "\" to target skeleton.");
            }
        }
        myRenderer.bones = newBones;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
