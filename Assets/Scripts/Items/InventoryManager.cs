using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Arrays de items")]
    //key items: mano, linterna, crucifijo, gancho, pistola
    GameObject[] KeyItems = new GameObject[5]; //los items clave del inventario
    GameObject[] UseItems = new GameObject[5]; //los consumibles del inventario

    KeyItem[] KeyItemScripts = new KeyItem[5]; //los scripts de cada key item
    public Sprite[] KeyItemImages = new Sprite[6]; //imagenes ui inventario

    UseItem[] UseItemScripts = new UseItem[5];
    public Sprite[] UseItemImages = new Sprite[6];

    GameObject invHUDGO;
    GameObject[] HUDUseItemsGO = new GameObject[5];
    UseItem[] HUDUseItemScripts = new UseItem[5];

    GameObject KeyItemsArray; //padre que tiene de hijos a los key items
    GameObject UseItemsArray; //padre que tiene de hijos a los consumibles

    GameObject InventoryPanelGO;
    bool isInventoryOpen = false;
    
    GameObject PlayerCamGO;
    PlayerState PlayerStateScript;

    GameObject LinternaGO;
    Linterna LinternaScript;

    void Awake(){
        InventoryPanelGO = this.transform.GetChild(0).gameObject;
        invHUDGO = this.transform.parent.Find("InvHUD").gameObject;
        KeyItemsArray = InventoryPanelGO.transform.Find("KeyItems").gameObject;
        UseItemsArray = InventoryPanelGO.transform.Find("UsableItems").gameObject;
        for(int i=0;i<5;i++){
            KeyItems[i] = KeyItemsArray.transform.GetChild(i).gameObject;
            UseItems[i] = UseItemsArray.transform.GetChild(i).gameObject;
            HUDUseItemsGO[i] = invHUDGO.transform.GetChild(i).gameObject;
            KeyItemScripts[i] = KeyItems[i].GetComponent<KeyItem>();
            UseItemScripts[i] = UseItems[i].GetComponent<UseItem>();
            HUDUseItemScripts[i] = HUDUseItemsGO[i].GetComponent<UseItem>();
        }
        PlayerCamGO = GameObject.FindObjectOfType<Camera>().gameObject;
        PlayerStateScript = PlayerCamGO.GetComponent<PlayerState>();

        LinternaGO = PlayerCamGO.transform.Find("HandWithLinterna").gameObject;
        LinternaScript = LinternaGO.GetComponent<Linterna>();
    }

    void Start(){
        SetInventoryOpenState(false); //al empezar tener el inventario cerrado
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && (PlayerStateScript.PlayerStatus[0] || PlayerStateScript.PlayerStatus[1])){ //switch para abrir y cerrar el inventario
            if(isInventoryOpen){
                PlayerStateScript.SetPlayerStatus(0,true);
                SetInventoryOpenState(false);
            }else{
                PlayerStateScript.SetPlayerStatus(1,true);
                SetInventoryOpenState(true);
            }
        }

        if(isInventoryOpen){ //si esta abierto el inventario desbloquear cursor
            Cursor.lockState = CursorLockMode.None;
        }else{
            if(Input.GetKeyDown(KeyCode.Alpha1) && UseItemScripts[0].UseItemID!=0){
                ConsumeItem(UseItemScripts[0].UseItemID,0);
            }
            if(Input.GetKeyDown(KeyCode.Alpha2) && UseItemScripts[1].UseItemID!=0){
                ConsumeItem(UseItemScripts[1].UseItemID,1);
            }
            if(Input.GetKeyDown(KeyCode.Alpha3) && UseItemScripts[2].UseItemID!=0){
                ConsumeItem(UseItemScripts[2].UseItemID,2);
            }
            if(Input.GetKeyDown(KeyCode.Alpha4) && UseItemScripts[3].UseItemID!=0){
                ConsumeItem(UseItemScripts[3].UseItemID,3);
            }
            if(Input.GetKeyDown(KeyCode.Alpha5) && UseItemScripts[4].UseItemID!=0){
                ConsumeItem(UseItemScripts[4].UseItemID,4);
            }
        }
    }

    public void KeyItemObtained(int posicion, int NewKeyItemID){ //reemplazar key item al conseguirlo
        if(posicion!=0){
            KeyItemScripts[posicion].KeyItemID = NewKeyItemID;
        }
    }

    public void UseItemObtained(int NewUseItemID){
        for(int i = 0;i<UseItemScripts.Length;i++){
            if((UseItemScripts[i].UseItemID == 0) || (UseItemScripts[i].UseItemID == NewUseItemID)){ //si el espacio está desocupado o ya existe el objeto
                UseItemScripts[i].UseItemID = NewUseItemID;
                UseItemScripts[i].UseItemQuantity++;
                HUDUseItemScripts[i].UseItemID = NewUseItemID;
                HUDUseItemScripts[i].UseItemQuantity++;
                return;
            }
        }
        Debug.Log("No queda espacio en el inventario");
    }

    public bool CheckItemAvailability(int ItemID){
        for(int i = 0;i<UseItemScripts.Length;i++){
            if((UseItemScripts[i].UseItemID == ItemID) && (UseItemScripts[i].UseItemQuantity>=10)){
                return false;
            }
        }
        return true;
    }

    public void SetInventoryOpenState(bool state){ //habilitar o deshabilitar cursor
        InventoryPanelGO.SetActive(state);
        isInventoryOpen = state;
    }

    public void ConsumeItem(int ItemID,int position){
        bool wasConsumed=false;
        switch(ItemID){
            case 1: //batería de linterna
                if(PlayerStateScript.CurrentWeapon!=1){
                    Debug.Log("Debes tener la linterna en la mano");
                    break;
                }
                if(PlayerStateScript.isObtained[1]){
                    if(LinternaScript.LinternaBattery<100){
                        LinternaScript.LinternaBattery = 100;
                        wasConsumed=true;
                    }else{
                        Debug.Log("La bateria de tu linterna esta al maximo");
                    }
                }else{
                    Debug.Log("No tienes linterna");
                }
                break;
            case 2: //medkit
                Debug.Log("Te has curado (debug)");
                wasConsumed=true;
                break;
            default:
                Debug.Log("El ID de este consumible no es valido");
                break;
        }
        if(wasConsumed){
            if(UseItemScripts[position].UseItemQuantity<=1){
                UseItemScripts[position].UseItemID=0;
                HUDUseItemScripts[position].UseItemID=0;
            }
            UseItemScripts[position].UseItemQuantity--;
            HUDUseItemScripts[position].UseItemQuantity--;
        }
        return;
    }
}
