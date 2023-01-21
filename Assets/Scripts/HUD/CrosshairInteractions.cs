using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; //Text mesh pro

public class CrosshairInteractions : MonoBehaviour
{
    public bool isTargetInteractable = true;

    GameObject TextHolderGO;
    DialogManager TextHolderScript;

    GameObject CrosshairGO;
    Animator CrosshairAnimator;
    GameObject CrosshairTextGO;
    TextMeshProUGUI CrosshairTextMesh;

    GameObject InventoryGO;
    InventoryManager InventoryScript;

    int NPCLayer, KeyItemLayer, UseItemLayer;

    public PlayerState PlayerStateScript; //para que al recoger un arma se deje constancia en el script PlayerState que se obtuvo

    void Awake()
    {
        InventoryGO = GameObject.FindObjectOfType<InventoryManager>().gameObject;
        InventoryScript = InventoryGO.GetComponent<InventoryManager>();

        TextHolderGO = GameObject.FindObjectOfType<DialogManager>().gameObject;
        TextHolderScript = TextHolderGO.GetComponent<DialogManager>();

        CrosshairGO = GameObject.Find("Crosshair").transform.GetChild(0).GetChild(0).gameObject;
        CrosshairTextGO = CrosshairGO.transform.parent.parent.GetChild(1).gameObject;
        CrosshairAnimator = CrosshairGO.GetComponent<Animator>();
        CrosshairTextMesh = CrosshairTextGO.GetComponent<TextMeshProUGUI>();

        NPCLayer = LayerMask.NameToLayer("NPC");
        KeyItemLayer = LayerMask.NameToLayer("KeyItem");
        UseItemLayer = LayerMask.NameToLayer("UseItem");
    }

    void Update()
    {
        //!!!!!!!!!!!!!!!!!!!!!HACER ESTO CENTRALIZADO
        //si hay un dialogo activo se sale el crosshair y se bloquea la capacidad de interaccion
        if(PlayerStateScript!=null){
            if((PlayerStateScript.PlayerStatus[0] || PlayerStateScript.PlayerStatus[2])){
                SetCrosshairActiveState(true);
            }else{ //si no hay, se activa el crosshair y las interacciones
                SetCrosshairActiveState(false);
            }
        }
        if(PlayerStateScript!=null){
            if(PlayerStateScript.PlayerStatus[0]){
                RaycastHit hit;
                if (Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward), out hit, 4) && isTargetInteractable){ //si el raycast choca con algo
                    if(hit.transform.gameObject.layer == NPCLayer){ //si choca con NPC
                        CrosshairTextGO.SetActive(true);
                        CrosshairTextMesh.text = "Hablar";
                        CrosshairAnimator.SetBool("isInteractable",true);
                        if(Input.GetKeyDown(KeyCode.E) && isTargetInteractable){
                            HandleTalk(hit.transform.gameObject); //si aprieta E y puede interactuar iniciar dialogo
                        }
                    }else if(hit.transform.gameObject.layer == KeyItemLayer){ //si choca con un key item
                        CrosshairTextGO.SetActive(true);
                        CrosshairTextMesh.text = "Recoger";
                        CrosshairAnimator.SetBool("isInteractable",true);
                        if(Input.GetKeyDown(KeyCode.E) && isTargetInteractable){
                        HandleKeyPickup(hit.transform.gameObject); //si aprieta E y puede interactuar recoger item
                        }
                    }else if(hit.transform.gameObject.layer == UseItemLayer){
                        CrosshairTextGO.SetActive(true);
                        CrosshairTextMesh.text = "Recoger";
                        CrosshairAnimator.SetBool("isInteractable",true);
                        if(Input.GetKeyDown(KeyCode.E) && isTargetInteractable){
                        HandleUsablePickup(hit.transform.gameObject); //si aprieta E y puede interactuar recoger item
                        }
                    }else{
                        TargetNotInteractable(); //si choca con cualquier otra cosa no es interactuable
                    }
                }else{
                    TargetNotInteractable(); //si no choca con nada no es interactuable
                }
            }else{
                TargetNotInteractable(); //si no se está en estado playable no se puede interactuar
            }
        }
        
    }

    public void SetCrosshairActiveState(bool state){ //habilitar o deshabilitar crosshair e interacciones
        isTargetInteractable=state;
        CrosshairGO.SetActive(state);
        CrosshairTextGO.SetActive(state);
    }

    void TargetNotInteractable(){ //pone crosshair modo no interactuable
        CrosshairTextGO.SetActive(false);
        CrosshairAnimator.SetBool("isInteractable",false);
    }

    void HandleTalk(GameObject other){ //inicia dialogo segun DialogList del NPC 
        DialogList dialogo = other.GetComponent<DialogList>();
        //other.transform.LookAt(this.gameObject.transform); //placeholder
        //this.transform.LookAt(other.gameObject.transform);
        TextHolderScript.TryStartDialog(dialogo.dialogcodes[0]);
    }

    void HandleKeyPickup(GameObject other){ //agrega el key item al inventario
        string ItemName = other.gameObject.GetComponent<ItemIdentifier>().thisItemIs;
        int ItemID = 0;
        switch (ItemName){
            case "LinternaItem":
                ItemID = 1;
                break;
            case "CrucifixItem":
                ItemID = 2;
                break;
            case "GrappleShotItem":
                ItemID = 3;
                break;
            case "PistolItem":
                ItemID = 4;
                break;
            default:
                Debug.Log("El ID de este key item no es válido");
                break;
        }
        InventoryScript.KeyItemObtained(ItemID,ItemID+1); //posicion(0-4), new key item id(0-5) [0 = nada y 1 = mano]
        PlayerStateScript.isObtained[ItemID]=true;
        PlayerStateScript.ActiveWeapon(ItemID); //armas 0: mano, 1: linterna, 2: crucifijo, 3: gancho, 4: pistola
        Destroy(other);
    }

    void HandleUsablePickup(GameObject other){
        string ItemName = other.gameObject.GetComponent<ItemIdentifier>().thisItemIs;
        int ItemID = 0;
        switch(ItemName){
            case "BatteryItem":
                ItemID = 1;
                break;
            case "MedKitItem":
                ItemID = 2;
                break;
            default:
                Debug.Log("El ID de este consumible no es válido");
                break;
        }
        if(InventoryScript.CheckItemAvailability(ItemID)){
            InventoryScript.UseItemObtained(ItemID);
            Destroy(other);
        }else{
            Debug.Log("No puedes llevar más cantidad de este objeto (debug)");
        }
    }
}
