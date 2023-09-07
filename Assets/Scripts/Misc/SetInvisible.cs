using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInvisible : MonoBehaviour
{
    GameObject Hand;

    void Awake(){
        Hand = this.gameObject;
    }

    void Start(){
        SetTargetInvisible(Hand);
    }

    void SetTargetInvisible(GameObject Target)
    {
        foreach (Renderer r in Target.GetComponentsInChildren(typeof(Renderer)))
        {
            r.enabled = false;
        }
    }

    void SetTargetVisible(GameObject Target){
        foreach (Renderer r in Target.GetComponentsInChildren(typeof(Renderer)))
        {
            r.enabled = true;
        }
    }
}
