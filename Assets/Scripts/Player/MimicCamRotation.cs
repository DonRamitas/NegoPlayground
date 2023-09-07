using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicCamRotation : MonoBehaviour
{

    GameObject CamHolderGO;

    // Start is called before the first frame update
    void Awake()
    {
        CamHolderGO = GameObject.FindObjectOfType<FirstPersonLook>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = CamHolderGO.transform.rotation;
        this.transform.position = CamHolderGO.transform.position;
    }
}
