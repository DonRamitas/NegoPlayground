using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyItem : MonoBehaviour
{
    [Header("Numero de ID del Key Item")]
    public int KeyItemID; //que tipo de key item es este (0-4, mano a pistola)

    private Image thisImage; //el image de este key item

    GameObject InventoryMan; //referencia al GO del inventory manager
    InventoryManager InventoryManScript; //referencia al script inventory manager

    void Awake()
    {
        thisImage = this.gameObject.GetComponent<Image>();
        InventoryMan = GameObject.FindObjectOfType<InventoryManager>().gameObject;
        InventoryManScript = InventoryMan.GetComponent<InventoryManager>();
    }

    void Update()
    {
        if(KeyItemID!=0){ //si hay un item
            thisImage.color = Color.white; //mostrarlo claro
            
        }else{ //si no hay item
            thisImage.color = Color.yellow; //mostrarlo oscuro
        }
        thisImage.sprite = InventoryManScript.KeyItemImages[KeyItemID]; //le asigna su imagen segun el key item ID
    }
}
