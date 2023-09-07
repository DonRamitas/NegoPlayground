using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    //player statuses - 0: NormalPlayable - 1: Inventory - 2: Radial - 3: InNormalDialog
    public bool[] PlayerStatus = {true,false,false,false};

    GameObject[] Weapons = new GameObject[5];
    Animator[] WeaponsAnim = new Animator[5]; //animadores de las armas
    public AudioClip[] WeaponDrawSounds;

    GameObject PlayerGO;
    GameObject WeaponSoundGO;
    public AudioSource WeaponSoundSource;

    [Header("Arma actual y obtenidas")]
    public int CurrentWeapon; //numero que indica que arma es la usada actualmente (0-4, mano-pistola)

    public bool[] isObtained = {true,false,false,false,false,false}; //arreglo que dice cuales armas estan desbloqueadas

    void Awake()
    {
        for(int i=0;i<5;i++){
            Weapons[i] = this.transform.Find("Weapons").GetChild(i).gameObject;
            WeaponsAnim[i] = Weapons[i].GetComponent<Animator>();
        }

        PlayerGO = GameObject.FindObjectOfType<PlayerState>().gameObject;
        WeaponSoundGO = PlayerGO.transform.Find("Weapons").Find("WeaponSoundSource").gameObject;
        WeaponSoundSource = WeaponSoundGO.GetComponent<AudioSource>();
    }

    void Start(){
        ActiveWeapon(0);
    }

    public void SetPlayerStatus(int statusnum, bool value){
        for(int i = 0;i<PlayerStatus.Length;i++){
            if(i==statusnum){
                PlayerStatus[i]=true;
            }else{
                PlayerStatus[i]=false;
            }
        }
    }
    
    public void ActiveWeapon(int WeaponID){ //cambiar de arma activa
        for(int i = 0; i < 5 ; i++){
            if(i == WeaponID){
                CurrentWeapon = i;
                Weapons[i].SetActive(true);
                if(WeaponsAnim[i].GetBool("gotDrown")){
                    WeaponsAnim[i].SetTrigger("gotDrown");
                }
                WeaponSoundSource.PlayOneShot(WeaponDrawSounds[i]);
            }else{
                Weapons[i].SetActive(false);
            }
        }
    }
}
