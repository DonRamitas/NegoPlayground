using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadialItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Image thisImage;

    public int ThisOptionID; //el numero de arma de este key item

    //formas de seleccionar el arma
    public bool isMouseOver = false;
    public bool isRayOver = false;
    
    GameObject PlayerGO;
    PlayerState PlayerStateScript; //para ver si el jugador tiene el arma en cuestion y renderizar su icono en el menu radial

    GameObject RadialMenuGO;
    RadialMenu RadialMenuScript; //para indicarle si se clickeo en algun item para cerrar el menu

    GameObject RadialIconGO;
    Image RadialIconImage; //para poner no visible en caso de que no este desbloqueada el arma

    void Awake()
    {
        thisImage = this.gameObject.GetComponent<Image>();

        RadialMenuGO = this.transform.parent.gameObject.transform.parent.gameObject;
        RadialMenuScript = RadialMenuGO.GetComponent<RadialMenu>();

        RadialIconGO = this.transform.GetChild(0).gameObject;
        RadialIconImage = RadialIconGO.GetComponent<Image>();

        PlayerGO = GameObject.FindObjectOfType<PlayerState>().gameObject;
        PlayerStateScript = PlayerGO.GetComponent<PlayerState>();
    }

    // Update is called once per frame
    void Start()
    {
        CheckObtained();
        thisImage.alphaHitTestMinimumThreshold = 0.5f; //para que no tome en cuenta el alfa que no se ve
    }

    void Update(){
        CheckObtained();
        thisImage.color = new Color32(0,0,0,127);
        if(isMouseOver || isRayOver){ //si el mouse pasa por encima se hace highlight
            thisImage.color = new Color32(255,255,255,127);
            if (UnityEngine.Input.GetMouseButtonDown(0)){ 
                RadialMenuScript.haveClicken=true;
            }
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData){ //hover enter
        isMouseOver = true;
    }

    public void OnPointerDown(PointerEventData eventData){ //click
        RadialMenuScript.haveClicken=true;
    }

    public void OnPointerExit(PointerEventData eventData){ //hover exit
        isMouseOver = false;
    }

    void CheckObtained(){ //ve si está obtenida este key item
        if(!PlayerStateScript.isObtained[ThisOptionID-1]){
            RadialIconImage.enabled = false;
        }else{
            RadialIconImage.enabled = true;
        }
    }
}
