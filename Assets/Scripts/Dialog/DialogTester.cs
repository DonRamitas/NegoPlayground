using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTester : MonoBehaviour
{
    GameObject TextHolderGO;
    DialogManager TextHolderScript;

    void Start()
    {
        TextHolderGO = GameObject.FindObjectOfType<DialogManager>().gameObject;
        TextHolderScript = TextHolderGO.GetComponent<DialogManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            TextHolderScript.TryStartDialog("test_1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TextHolderScript.TryStartDialog("test_2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TextHolderScript.TryStartDialog("test_3");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TextHolderScript.TryStartDialog("test_4");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TextHolderScript.TryStartDialog("test_5");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            TextHolderScript.TryStartDialog("test_6");
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            TextHolderScript.TryStartDialog("test_7");
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            TextHolderScript.TryStartDialog("test_8");
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            TextHolderScript.TryStartDialog("test_9");
        }
    }
}
