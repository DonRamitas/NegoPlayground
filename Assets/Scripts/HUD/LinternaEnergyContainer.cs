using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinternaEnergyContainer : MonoBehaviour
{
    private GameObject PlayerGO;
    private PlayerState PlayerStateScript;
    private CanvasGroup thisCanvasGroup;

    void Awake()
    {
        PlayerGO = GameObject.FindGameObjectsWithTag("Player")[0];
        PlayerStateScript = PlayerGO.GetComponent<PlayerState>();
        thisCanvasGroup = this.gameObject.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerStateScript.CurrentWeapon != 1){
            thisCanvasGroup.alpha = 0;
        }else{
            thisCanvasGroup.alpha = 1;
        }
    }
}
