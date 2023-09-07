using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class NotificationManager : MonoBehaviour
{

    public GameObject notificationGO;
    TMP_Text mensaje_text;
    Image NotiImage;
    Animator NotiAnimator;
    public AudioClip[] NotiSound;
    AudioSource NotiAudioSource;

    bool isCoroutineRunning = false;

    void Awake(){
        InitComponentes();
    }

    void InitComponentes(){
        notificationGO = this.transform.GetChild(0).gameObject;
        mensaje_text = notificationGO.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        NotiImage = notificationGO.GetComponent<Image>();
        NotiAnimator = notificationGO.GetComponent<Animator>();
        notificationGO.SetActive(false);
        NotiAudioSource = this.gameObject.GetComponent<AudioSource>();
    }

    public void ShowNotification(string mensaje, string colorname,string sound){
        notificationGO.SetActive(true);
        mensaje_text.text = mensaje;
        Color newcolor = Color.clear;
        ColorUtility.TryParseHtmlString(colorname, out newcolor);
        Color32 newcolor32 = newcolor;
        NotiAnimator.Play("NotiPop");
        NotiImage.color = new Color32(newcolor32.r,newcolor32.g,newcolor32.b, 100);
        switch(sound){
            case "bad":
                NotiAudioSource.PlayOneShot(NotiSound[0]);
                break;
            case "heal":
                NotiAudioSource.PlayOneShot(NotiSound[1]);
                break;
            default:
                break;
        }
    }
}
