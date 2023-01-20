using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadialMenu : MonoBehaviour
{
    GameObject RadialPanel; //el panel intermediario
    public GameObject[] RadialOption = new GameObject[5];
    public RadialItem[] RadialOptionScripts = new RadialItem[5]; //los scripts de cada opcion

    bool isOpen = false;

    GameObject PlayerCam;
    PlayerState PlayerStateScript; //para ver si al cambiar el jugador tiene el arma en cuestion

    public bool haveClicken = false; //si se clickeo un key item del menu radial

    Camera PlayerCamCam;

    GameObject CrosshairGO;
    Animator CrosshairAnimator;

    GameObject CrosshairPivotGO;

    void Awake(){
        RadialPanel = this.transform.GetChild(0).gameObject;
        for(int i=0;i<5;i++){
            RadialOption[i] = RadialPanel.transform.GetChild(i).gameObject;
            RadialOptionScripts[i] = RadialOption[i].GetComponent<RadialItem>();
        }

        PlayerCam = GameObject.FindObjectOfType<Camera>().gameObject;
        PlayerStateScript = PlayerCam.GetComponent<PlayerState>();
        PlayerCamCam = PlayerCam.GetComponent<Camera>();

        CrosshairGO = this.transform.parent.Find("Crosshair").GetChild(0).GetChild(0).gameObject;
        CrosshairAnimator = CrosshairGO.GetComponent<Animator>();

        CrosshairPivotGO = CrosshairGO.transform.parent.gameObject;
    }

    void Update(){
        if(Input.GetKey(KeyCode.Tab)){ //si se apreta tab
            if(!isOpen && !haveClicken && PlayerStateScript.PlayerStatus[0]){ //si no esta abierto el menu radial y no se ha clickeado aun una opcion
                Cursor.lockState = CursorLockMode.None; //desbloquear cursor
                CrosshairAnimator.SetBool("isRadialMenu",true); //indica al animador del crosshair que se está en un radial menu
                isOpen = true; //indicar que esta abierto
                RadialPanel.SetActive(true); //activar panel
                PlayerStateScript.SetPlayerStatus(2,true);
            }
            if(haveClicken){ //si se clickeó, cerrar inmediatamente el menu radial y escoger el item clickeado, dejando haveclicken en true para que no se vuelva a abrir
                CloseRadial();
            }
        }else{ //si no está apretado tab, el menu radial permanece cerrado y se resetea el haveclicken
            CloseRadial();
            haveClicken=false;
        }
    }

    void OnGUI(){
        if(isOpen){
            RayoDesdeCentroRadial();
        }
    }

    void RayoDesdeCentroRadial(){
        Vector2 ScreenCenter = new Vector2((PlayerCamCam.pixelWidth-1)/2,(PlayerCamCam.pixelHeight-1)/2); //recibe constantemente el centro de la pantalla
        Vector2 MouseDirection = (ScreenCenter-(Vector2)Input.mousePosition).normalized*-Vector2.one; //el vector, desde el centro, hacia el mouse
        float DistanciaCentroMouse = Vector2.Distance(ScreenCenter,Input.mousePosition);
        if(DistanciaCentroMouse>6){
            CrosshairAnimator.SetBool("isMouseFarEnough",true);
            Vector3 difference = ScreenCenter-(Vector2)Input.mousePosition;
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            CrosshairPivotGO.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        }else{
            CrosshairAnimator.SetBool("isMouseFarEnough",false);
            CrosshairPivotGO.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        RaycastHit2D hit = Physics2D.Raycast(ScreenCenter,MouseDirection,DistanciaCentroMouse); //rayo desde el centro al mouse
        if(hit.collider!=null){ //si esta tocando un collider
            for(int i=0;i<RadialOption.Length;i++){ //para cada opcion radial
                if(RadialOption[i].name==hit.collider.transform.parent.gameObject.name){ //si el nombre de la opcion coincide con el objeto hiteado
                    RadialOptionScripts[i].isRayOver=true; //indica que tiene el rayo encima
                }else{
                    RadialOptionScripts[i].isRayOver=false; //indica que no tiene el rayo encima
                }
            }
        }else{ //si no toca nada
            for(int i=0;i<RadialOption.Length;i++){
                RadialOptionScripts[i].isRayOver=false; //nadie tiene el rayo encima
            }
        }
    }
    
    void ResetParameters(){ //resetea los mouse over (y por lo tanto, tambien highlightings) de todos los items del menu radial
        for(int i=0;i<5;i++){
            RadialOptionScripts[i].isMouseOver=false;
            RadialOptionScripts[i].isRayOver=false;
            CrosshairPivotGO.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
    }

    public void CloseRadial(){ //cuando se cierra un menu radial
        Cursor.lockState = CursorLockMode.Locked; //bloquear el cursor
        if(isOpen){
            for(int i=0;i<5;i++){ //verificar cual de los items es el seleccionado, si esta desbloqueado, y ponerlo activo
                if((RadialOptionScripts[i].isMouseOver || RadialOptionScripts[i].isRayOver) && PlayerStateScript.isObtained[i] && (PlayerStateScript.CurrentWeapon != i)){
                    PlayerStateScript.ActiveWeapon(i);
                }
            }
            ResetParameters();
            PlayerStateScript.SetPlayerStatus(0,true);
            isOpen=false;
        }
        CrosshairAnimator.SetBool("isRadialMenu",false);
        RadialPanel.SetActive(false); //desactivar el panel radial
    }
}
