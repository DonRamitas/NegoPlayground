using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Linterna : MonoBehaviour
{
    GameObject LinternaStatesGO;
    SpriteRenderer LinternaStatesSR;

    public Sprite[] LinternaStatesImages;
    public AudioClip[] FlashlightSounds;

    GameObject LightSourceGO;
    Light LightSourceLight;

    GameObject UVSourceGO;
    Light UVSourceLight;

    public bool isLightOn = false;
    public bool isUVOn = false;

    GameObject PlayerGO;
    public PlayerState PlayerStateScript;

    GameObject WeaponSoundGO;
    AudioSource WeaponSoundSource;

    AudioSource UVSoundSource;

    public float UVLevel = 0;

    public float LinternaBattery = 100;
    public float LinternaBatteryDrainMultiplier = 1;
    public bool LinternaBatteryDead = false;

    GameObject LinternaContainerGO;
    CanvasGroup LinternaContainerCanvasGroup;

    GameObject LinternaBarGO;
    
    Animator thisAnimator;

    private FirstPersonMovement controller;

    bool changedSpeed = false;

    void Awake()
    {
        LinternaStatesGO = this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        LinternaStatesSR = LinternaStatesGO.GetComponent<SpriteRenderer>();

        LightSourceGO = LinternaStatesGO.transform.GetChild(0).gameObject;
        LightSourceLight = LightSourceGO.GetComponent<Light>();

        UVSourceGO = LinternaStatesGO.transform.GetChild(1).gameObject;
        UVSourceLight = UVSourceGO.GetComponent<Light>();
        UVSoundSource = UVSourceGO.GetComponent<AudioSource>();

        PlayerGO = this.gameObject.transform.parent.parent.gameObject;
        PlayerStateScript = PlayerGO.GetComponent<PlayerState>();

        WeaponSoundGO = PlayerGO.transform.Find("Weapons").Find("WeaponSoundSource").gameObject;
        WeaponSoundSource = WeaponSoundGO.GetComponent<AudioSource>();

        LinternaContainerGO = GameObject.FindObjectOfType<Canvas>().gameObject.transform.Find("LinternaEnergyContainer").gameObject;
        LinternaContainerCanvasGroup = LinternaContainerGO.GetComponent<CanvasGroup>();

        LinternaBarGO = LinternaContainerGO.transform.GetChild(0).gameObject;

        thisAnimator = this.gameObject.GetComponent<Animator>();

        controller = GameObject.FindWithTag("Player").gameObject.GetComponent<FirstPersonMovement>();
    }

    void Start(){
        LinternaStatesSR.sprite = LinternaStatesImages[0];
        LightSourceLight.enabled = false;
        UVSourceLight.enabled = false;
    }

    void Update()
    {
        if(PlayerStateScript.CurrentWeapon == 1){
            LinternaBarGO.transform.localScale = new Vector3(LinternaBattery/100,1,1);
            if(PlayerStateScript.PlayerStatus[0] && UnityEngine.Input.GetMouseButtonDown(0)){
                if(!isLightOn && LinternaBattery>0){ //si está apagada y tiene batería se prende
                    TurnOnLight();
                }else{ //si le falta alguna de esas se apaga la luz
                    TurnOffLight(true);
                }
            }

            if(PlayerStateScript.PlayerStatus[0]){
                if(UnityEngine.Input.GetMouseButtonDown(1)){
                    if(LinternaBattery>0 && !isUVOn){
                        TurnOnUVLight();
                    }
                }

                if(UnityEngine.Input.GetMouseButtonUp(1)){
                    TurnOffUVLight(true);
                }
            }

            if(isLightOn){
                LinternaBattery-=Time.deltaTime * LinternaBatteryDrainMultiplier;
            }

            if(isUVOn){
                LinternaBattery-=Time.deltaTime * LinternaBatteryDrainMultiplier;
            }

            if(LinternaBattery<=0 && !LinternaBatteryDead){
                TurnOffLight(false);
                TurnOffUVLight(false);
            }

            if(LinternaBattery>0){
                LinternaBatteryDead=false;
            }
        }
    }

    void FixedUpdate(){
        if(isUVOn){
            if(!changedSpeed){
                controller.canRun=false;
                controller.speed=3;
                Debug.Log("Lentito");
                changedSpeed = true;
            }
        }else{
            if(changedSpeed){
                controller.canRun=true;
                controller.speed=5;
                Debug.Log("Rápido de nuevo");
                changedSpeed=false;
            }
        }
    }

    public void TurnOnLight(){
        WeaponSoundSource.PlayOneShot(FlashlightSounds[1]);
        LinternaStatesSR.sprite = LinternaStatesImages[1];
        LightSourceLight.enabled = true;
        LinternaBatteryDead=false;
        isLightOn=true;
    }

    public void TurnOnUVLight(){
        UVSourceLight.enabled = true;
        UVSoundSource.Play();
        UVSoundSource.volume = 0.25f;
        LinternaBatteryDead=false;
        isUVOn=true;
    }

    public void TurnOffUVLight(bool wasManual){
        if(!wasManual){
            WeaponSoundSource.PlayOneShot(FlashlightSounds[2]);
            LinternaBattery=0;
            LinternaBatteryDead=true;
        }
        UVSourceLight.enabled = false;
        UVSoundSource.Stop();
        UVSoundSource.volume = 0f;
        isUVOn=false;
    }

    public void TurnOffLight(bool wasManual){
        if(wasManual){
            WeaponSoundSource.PlayOneShot(FlashlightSounds[0]);
        }else{
            WeaponSoundSource.PlayOneShot(FlashlightSounds[2]);
            LinternaBattery=0;
            LinternaBatteryDead=true;
        }
        LinternaStatesSR.sprite = LinternaStatesImages[0];
        LightSourceLight.enabled = false;
        isLightOn=false;
    }
}
