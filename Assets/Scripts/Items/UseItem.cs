using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; //Text mesh pro

public class UseItem : MonoBehaviour
{
    public int UseItemQuantity = 0;
    public int thisPosition;
    [Header("Numero de ID del Use Item")]
    public int UseItemID; //que tipo de key item es este (0-4)

    private Image thisImage; //el image de este item

    GameObject InventoryMan; //referencia al GO del inventory manager
    InventoryManager InventoryManScript; //referencia al script inventory manager

    GameObject UseItemQuantityGO;
    TextMeshProUGUI UseItemQuantityText;

    void Awake()
    {
        thisImage = this.gameObject.GetComponent<Image>();
        InventoryMan = GameObject.FindObjectOfType<InventoryManager>().gameObject;
        InventoryManScript = InventoryMan.GetComponent<InventoryManager>();

        UseItemQuantityGO = this.transform.Find("Use"+(thisPosition+1)+"Quant").gameObject;
        UseItemQuantityText = UseItemQuantityGO.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if(UseItemID != 0){ //si hay un item
            thisImage.color = Color.white; //mostrarlo claro
            
        }else{ //si no hay item
            thisImage.color = Color.black; //mostrarlo oscuro
        }
        if(UseItemQuantity!=0){
            UseItemQuantityText.enabled=true;
            UseItemQuantityText.text = "x"+UseItemQuantity;
        }else{
            UseItemQuantityText.text = "x0";
            UseItemQuantityText.enabled=false;
        }
        thisImage.sprite = InventoryManScript.UseItemImages[UseItemID]; //le asigna su imagen segun el item ID
    }
}