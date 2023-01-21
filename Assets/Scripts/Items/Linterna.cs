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

    public bool isLightOn = false;

    GameObject PlayerCamGO;
    public PlayerState PlayerStateScript;

    GameObject WeaponSoundGO;
    AudioSource WeaponSoundSource;

    public float LinternaBattery = 100;
    public float LinternaBatteryDrainMultiplier = 1;
    public bool LinternaBatteryDead = false;

    GameObject LinternaContainerGO;
    GameObject LinternaBarGO;
    
    Animator thisAnimator;

    GameObject FPFXGO;
    Image FPFXImage;
    Animator FPFXAnimator;

    void Awake()
    {
        LinternaStatesGO = this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        LinternaStatesSR = LinternaStatesGO.GetComponent<SpriteRenderer>();

        LightSourceGO = LinternaStatesGO.transform.GetChild(0).gameObject;
        LightSourceLight = LightSourceGO.GetComponent<Light>();

        PlayerCamGO = this.gameObject.transform.parent.gameObject;
        PlayerStateScript = PlayerCamGO.GetComponent<PlayerState>();

        WeaponSoundGO = PlayerCamGO.transform.Find("WeaponSoundSource").gameObject;
        WeaponSoundSource = WeaponSoundGO.GetComponent<AudioSource>();

        //DEBUGGGGGGGGGGGGGGGGGGGGGG BARRA DE ENERGIA LINTERNA
        LinternaContainerGO = GameObject.FindObjectOfType<Canvas>().gameObject.transform.Find("LinternaEnergyContainer").gameObject;
        LinternaBarGO = LinternaContainerGO.transform.GetChild(0).gameObject;

        thisAnimator = this.gameObject.GetComponent<Animator>();
    }

    void Start(){
        LinternaStatesSR.sprite = LinternaStatesImages[0];
        LightSourceLight.enabled = false;
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

            if(PlayerStateScript.PlayerStatus[0] && UnityEngine.Input.GetMouseButtonDown(1) && isLightOn){
                if(LinternaBattery>=50 && !thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("LinternaNormalFlash")){
                    thisAnimator.SetTrigger("gotFlash");
                    FPFXAnimator.SetTrigger("LinternaNormalFlash");
                    WeaponSoundSource.PlayOneShot(FlashlightSounds[3]);
                    LinternaBattery-=50;
                }else{
                    Debug.Log("No tienes suficiente batería");
                }
            }

            if(isLightOn){
                LinternaBattery-=Time.deltaTime * LinternaBatteryDrainMultiplier;
            }

            if(LinternaBattery<=0 && !LinternaBatteryDead){
                TurnOffLight(false);
            }

            if(LinternaBattery>0){
                LinternaBatteryDead=false;
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
