using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [Header("Variables")]
        public bool DialogActive; //indica si hay un dialogo activo

        float MainTypeSpeed = 20; //velocidad de escritura base
        [SerializeField] float TotalTypeSpeed; //velocidad de escritura total
        float AccelValue; //para acelerar el texto
        float AccelWaitMultiplier; //para acelerar las pausas

        bool accelerable; //para controlar si se puede acelerar el texto
        bool skipped; //para hacer un corte abrupto en el typeo del texto

        string CurrentDialogCode; //almacena el DialogCode actual

        [SerializeField] bool InstaText; //indica si el texto se muestra instantaneamente

        char CurrentColorOverride = 'n';

    [Header("Asset Resources")]
        public AudioClip[] AllVoices; //almacena todos los clip de voz

    [Header("GO References")]
        GameObject TextHolderGO;
        TMP_Text TextHolderText; //el comp texto de este GO
        AudioSource TextHolderAudioSource; //apunta al audiosource de este GO
        RectTransform TextHolderRectTransform; //apunta al Rect Transform de este GO

        GameObject CanNextGO; //la flechita que indica si se puede avanzar
        Image CanNextImage;

        GameObject BoxGO; //apunta al GO de la caja
        Image BoxImage; //apunta al image de la caja
        RectTransform BoxRectTransform;
        Animator BoxAnimator;

        GameObject NameGO;
        TMP_Text NameText;

        GameObject ChoiceBoxGO; //GO del ChoiceBox
        Animator ChoiceBoxAnimator; //Animator del ChoiceBox
        Image ChoiceBoxImage;
        AudioSource ChoiceBoxAudioSource;
        RectTransform ChoiceBoxRect;

        GameObject[] ChoicesGO = new GameObject[4];
        Text[] ChoicesText = new Text[4];

        GameObject SelectArrowGO; //GO de la flecha de seleccion
        Image SelectArrowImage; //Image de la flecha
        GameObject SelectArrowContainerGO;
        RectTransform SelectArrowContainerRect;

        GameObject PlayerCamGO;
        PlayerState PlayerStateScript;

        GameObject CanvasGO;
        RectTransform CanvasRectTransform;

        GameObject SeparationLineGO;
        RectTransform SeparationLineRectTransform;
        Image SeparationLineImage;
    
    public void Awake(){
        InicioComponentes();
    }

    //inicia los componentes necesarios
    public void InicioComponentes(){
        TextHolderGO = this.transform.GetChild(0).Find("TextHolder").gameObject;
        TextHolderAudioSource = TextHolderGO.GetComponent<AudioSource>();
        TextHolderText = TextHolderGO.GetComponent<TMP_Text>();
        TextHolderRectTransform = TextHolderGO.GetComponent<RectTransform>();

        CanNextGO = this.transform.GetChild(0).Find("CanNext").gameObject;
        CanNextImage = CanNextGO.GetComponent<Image>();

        BoxGO = this.transform.Find("Box").gameObject;
        BoxImage = BoxGO.GetComponent<Image>();
        BoxRectTransform = BoxGO.GetComponent<RectTransform>();
        BoxAnimator = BoxGO.GetComponent<Animator>();

        PlayerCamGO = GameObject.FindObjectOfType<PlayerState>().gameObject;
        PlayerStateScript = PlayerCamGO.GetComponent<PlayerState>();

        NameGO = BoxGO.transform.Find("Name").gameObject;
        NameText = NameGO.GetComponent<TMP_Text>();

        CanvasGO = GameObject.FindObjectOfType<Canvas>().gameObject;
        CanvasRectTransform = CanvasGO.GetComponent<RectTransform>();

        SeparationLineGO = this.transform.GetChild(0).Find("SeparationLine").gameObject;
        SeparationLineRectTransform = SeparationLineGO.GetComponent<RectTransform>();
        SeparationLineImage = SeparationLineGO.GetComponent<Image>();
    }

    //verifica si hay un dialogo activo antes de iniciar otro
    public bool CanStartDialog(){
        if (!DialogActive && PlayerStateScript.PlayerStatus[0])
        {
            return true;
        }
        return false;
    }

    //intenta iniciar un dialogo
    public void TryStartDialog(string code){
        if(CanStartDialog()){
            StartDialog(code);
            return;
        }
        Debug.Log("No se pudo iniciar el dialogo");
    }

    //limpia el cuadro de dialogo y parametros
    void WipeDialog(){
        TextHolderText.text = "";
        CanNextGO.SetActive(false); //desactiva la flechita
        AccelValue = 1;
    }

    //establece las dimensiones del dialogo
    void SetDialogSizePosition(int DialogType){
        bool valido = false;
        switch (DialogType){
            //dialogo normal bottom
            case 0:    
                BoxRectTransform.sizeDelta = new Vector2(CanvasRectTransform.rect.width-40,150); //ajusta la escala a completa
                BoxRectTransform.anchoredPosition = new Vector2(20,5); //ajusta la posicion a completa 
                valido = true;
                break;
            //dialogo normal top
            case 1:
                BoxRectTransform.sizeDelta = new Vector2(CanvasRectTransform.rect.width-40,150); //ajusta la escala a completa
                BoxRectTransform.anchoredPosition = new Vector2(20,CanvasRectTransform.rect.height-BoxRectTransform.sizeDelta.y-5); //ajusta la posicion a completa 
                valido = true;
                break;
            default:
                Debug.Log("No existe ese tipo de dialogo");
                break;
        }
        if(valido){
            TextHolderRectTransform.sizeDelta = new Vector2(BoxRectTransform.sizeDelta.x-35,BoxRectTransform.sizeDelta.y-50);
            TextHolderRectTransform.anchoredPosition = new Vector2(30,-40); //ajusta la posicion a completa 
        }
    }

    //prepara el dialogo para uno nuevo
    void PrepareDialog(){
        WipeDialog(); //resetea el dialogo
        BoxGO.SetActive(true);
        DialogActive = true; //indica que hay un dialogo activo
        SetDialogSizePosition(0);
        BoxAnimator.Play("PopOut");
    }

    //inicia el dialogo segun un id entregado
    void StartDialog(string DialogCode){
        PrepareDialog();
        //lee dialogos.csv
        StreamReader reader = new StreamReader("Assets/AArchivos/dialogos.csv");  
        bool EndOfFile = false; 
        while (!EndOfFile)
        {
            string FullCsv = reader.ReadLine();
            if(FullCsv == null)
            {
                EndOfFile = true;
                break;
            }
            string[] filas = FullCsv.Split('}');
            for(int i = 0; i < filas.Length; i++)
            {
                string[] celdas = filas[i].Split(';'); //divide cada fila entre la cantidad de celdas
                if (celdas[0] == DialogCode)
                {
                    CurrentDialogCode = DialogCode;
                    SetTyper(celdas[1]);
                    StartCoroutine(Escribir(celdas));
                    break;
                }
            }
        }
    }

    //setea un typer segun numero ingresado
    void SetTyper(string TyperCode){
        StreamReader reader = new StreamReader("Assets/AArchivos/typerlist.csv");  
        bool EndOfFile = false; 
        while (!EndOfFile)
        {
            string FullCsv = reader.ReadLine();
            if(FullCsv == null)
            {
                EndOfFile = true;
                break;
            }
            string[] filas = FullCsv.Split('}'); //divide las filas del csv en un arreglo
            for(int i = 0; i < filas.Length; i++)
            {
                string[] celdas = filas[i].Split(';'); //divide cada fila entre la cantidad de celdas
                if (celdas[0] == TyperCode)
                {
                    DecideName(celdas[2]); //setea el nombre, si tiene
                    DecideVoiceSettings(celdas[3]); //setea la voz
                    DecideTypeSpeed(int.Parse(celdas[4])); //setea el typespeed
                    DecideTextColor(celdas[5]); //setea el color del texto
                    DecidePitch(float.Parse(celdas[6])); //setea el pitch de la voz
                    DecideAccelerable(celdas[7]); //setea si se puede acelerar el texto
                    DecideBoxColor(celdas[8]); //setea el color de la caja
                    DecideNameColor(celdas[9]); //setea el color del nombre
                    DecideLineColor(celdas[10]); //setea el color de la linea de separacion
                    DecideCanNextColor(celdas[11]); //setea el color de la flecha Can Next
                }
            }
        }
    }

    void DecideName(string name){
        if(name!="NONAME"){
            NameText.text = name;
            SeparationLineGO.SetActive(true);
            SeparationLineRectTransform.sizeDelta = new Vector2(name.Length*7,1);
        }else{
            SeparationLineGO.SetActive(false);
            NameText.text = "";
        }
        
    }

    void DecideVoiceSettings(string chartalking){
        for(int i = 0; i < AllVoices.Length; i++)
        {
            if(AllVoices[i].name == chartalking)
            {
                TextHolderAudioSource.clip = AllVoices[i];
                break;
            }
        }
    }

    void DecideTypeSpeed(int typespeed){
        MainTypeSpeed = typespeed;
    }

    void DecideTextColor(string colorname){
        Color newcolor = Color.clear;
        ColorUtility.TryParseHtmlString(colorname, out newcolor);
        TextHolderText.color = newcolor;
    }

    void DecideNameColor(string colorname){
        Color newcolor = Color.clear;
        ColorUtility.TryParseHtmlString(colorname, out newcolor);
        NameText.color = newcolor;
    }

    void DecidePitch(float porcentaje){
        float newpitch = porcentaje / 100;
        TextHolderAudioSource.pitch = newpitch;
    }

    void DecideAccelerable(string decision){
        if (decision == "true")
        {
            accelerable = true;
        }
        else
        {
            accelerable = false;
        }
    }

    void DecideBoxColor(string colorname){
        Color newcolor = Color.clear;
        ColorUtility.TryParseHtmlString(colorname, out newcolor);
        Color32 newcolor32 = newcolor;
        BoxImage.color = new Color32(newcolor32.r,newcolor32.g,newcolor32.b, 255);
    }

    void DecideLineColor(string colorname){
        Color newcolor = Color.clear;
        ColorUtility.TryParseHtmlString(colorname, out newcolor);
        Color32 newcolor32 = newcolor;
        SeparationLineImage.color = new Color32(newcolor32.r,newcolor32.g,newcolor32.b, 255);
    }

    void DecideCanNextColor(string colorname){
        Color newcolor = Color.clear;
        ColorUtility.TryParseHtmlString(colorname, out newcolor);
        Color32 newcolor32 = newcolor;
        CanNextImage.color = new Color32(newcolor32.r,newcolor32.g,newcolor32.b, 255);
    }

    //le quita los primeros N caracteres a un string y lo retorna
    string LeerResto(int offset, string texto){
        string restotexto = "";
        for (int i = offset; i < texto.Length; i++)
        {
            restotexto += texto[i];
        }
        return restotexto;
    }

    //modifica uno o varios parametros del dialogo
    void UpdateSettings(string line){
        string[] updatedsettings = line.Split('/');
        for (int i = 0; i < updatedsettings.Length; i++)
        {
            //[Name / Voice / VelTypeo / TextColor / Pitch / Accelerable / BoxColor / Typer / NameColor / SizePosition / LineColor]
            switch (updatedsettings[i][1])
            {
                case 'n': //cambio nombre
                    DecideName(LeerResto(2, updatedsettings[i]));
                    break;
                case 'v': //cambio voz
                    DecideVoiceSettings(LeerResto(2, updatedsettings[i]));
                    break;
                case 't': //cambio typespeed
                    DecideTypeSpeed(int.Parse(LeerResto(2, updatedsettings[i])));
                    break;
                case 'c': //cambio text color
                    DecideTextColor(LeerResto(2, updatedsettings[i]));
                    break;
                case 'p': //cambio pitch 0-100
                    DecidePitch(int.Parse(LeerResto(2, updatedsettings[i])));
                    break;
                case 'a': //cambio de accelerable
                    DecideAccelerable(LeerResto(2, updatedsettings[i]));
                    break;
                case 'b': //cambio box color
                    DecideBoxColor(LeerResto(2, updatedsettings[i]));
                    break;
                case 'T': //cambio typer
                    SetTyper(LeerResto(2,updatedsettings[i]));
                    break;
                case 'C':
                    DecideNameColor(LeerResto(2,updatedsettings[i]));
                    break;
                case 'D':
                    SetDialogSizePosition(int.Parse(LeerResto(2,updatedsettings[i])));
                    break;
                case 'l':
                    DecideLineColor(LeerResto(2,updatedsettings[i]));
                    break;
                case 'x':
                    DecideCanNextColor(LeerResto(2,updatedsettings[i]));
                    break;
                default:
                    Debug.Log("No existe la función");
                    break;
            }
        }
    }

    IEnumerator Escribir(string[] FullDialog){
        //recorre el dialogo exceptuando las dos primeras columnas
        for (int i = 2; i < FullDialog.Length; i++)
        {
            //si no hay nada en la linea leida se sale del for
            if (FullDialog[i] == "")
            {
                DialogActive=false;
                break;
            }

            //si la linea empieza con ^ 
            if (FullDialog[i][0] == '^')
            {
                UpdateSettings(FullDialog[i]);
                continue;
            }

            //analiza cada letra del dialogo actual
            for (int j = 0; j < FullDialog[i].Length; j++)
            {
                //si la letra es un { entonces es una funcion especial
                if (FullDialog[i][j] == '{')
                {
                    switch (FullDialog[i][j + 1])
                    {
                        //{e hace un salto de linea
                        case 'e':
                            TextHolderText.text += "\n";
                            j++;
                            break;
                        //{w hace una espera en el texto
                        case 'w':
                            float waitlevel = 10;
                            if (FullDialog[i][j + 2] != 'f')
                            {
                                waitlevel = (float)System.Char.GetNumericValue(FullDialog[i][j + 2]); //de 1 a 9
                            }
                            if (waitlevel >= 1 && waitlevel <= 10)
                            {
                                if(!InstaText){
                                    yield return new WaitForSeconds((waitlevel / 10) * AccelWaitMultiplier);
                                }
                                j += 2;
                            }else{
                                Debug.Log("invalid waitlevel");
                            }
                            break;
                        //si es una s se pasa al siguiente dialogo
                        case 's':
                            skipped = true;
                            j++;
                            break;
                        case 'c':
                            CurrentColorOverride = FullDialog[i][j + 2];
                            j+=2;
                            break;
                        //si es cualquier otra letra despues del { entonces se omite
                        default:
                            j++;
                            break;
                    }
                }
                //si no es un {
                else
                {
                    //se imprime la letra
                    TextHolderText.text += FullDialog[i][j];
                    if(CurrentColorOverride != 'n'){

                        TMP_TextInfo textInfo = TextHolderText.textInfo;

                        CanvasRenderer uiRenderer = TextHolderText.canvasRenderer;

                        Mesh newMesh = TextHolderText.mesh;

                        int vertexIndex = textInfo.characterInfo[textInfo.characterCount-1].vertexIndex;
                        Debug.Log(vertexIndex);
                        uiVertices[] newVertexColors = newMesh.vertices;
                        Color32 myColor = Color.red;

                        newVertexColors[vertexIndex + 0] = myColor;
                        newVertexColors[vertexIndex + 1] = myColor;
                        newVertexColors[vertexIndex + 2] = myColor;
                        newVertexColors[vertexIndex + 3] = myColor;

                        uiRenderer.SetMesh(newMesh);
                    }
                    //si la letra no es un espacio
                    if (FullDialog[i][j] != ' ')
                    {
                        //suena un sonido
                        TextHolderAudioSource.Stop();
                        TextHolderAudioSource.Play();
                    }
                }
                if(!InstaText){
                    yield return new WaitForSeconds(1 / TotalTypeSpeed);
                }
            }
            if (skipped) //si el texto no se auto skipea
            {
                skipped = false;
            }
            else
            {
                CanNextGO.SetActive(true); //al terminar de escribir se ve el CanNext
                BoxAnimator.Play("CanNextJiggle");
                yield return new WaitForSeconds(0.05f);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            }
            if(i != FullDialog.Length-1)
            {
                WipeDialog();
            }
        }
    }

    public void ForceStopDialog(){
        StopAllCoroutines();
        DialogActive = false;
        BoxGO.SetActive(false);
        PlayerStateScript.SetPlayerStatus(0,true);
    }

    void AccelerateText(bool doAccel){
        if(doAccel){
            AccelValue = 15; //acelera el texto
            AccelWaitMultiplier = 0.66f; //acelera las pausas
            TextHolderAudioSource.volume = 0.5f; //hace el ruido menos molesto

        }else{
            AccelValue = 0;
            AccelWaitMultiplier = 1f;
            TextHolderAudioSource.volume = 1f;
        }
        TotalTypeSpeed = MainTypeSpeed + AccelValue; //acelera el texto
    }

    public void Update(){
        if (Input.GetKeyDown(KeyCode.P)){
            ForceStopDialog();
        }

        if (accelerable) //va mas rapido el texto y las pausas
        {
            if(Input.GetKey(KeyCode.LeftShift)){
                AccelerateText(true);
            }else{
                AccelerateText(false);
            }

            if(Input.GetKey(KeyCode.Q)){
                InstaText = true;
            }else{
                InstaText = false;
            }
        }

        if (!DialogActive && BoxGO.activeSelf)
        {
            BoxGO.SetActive(false);
        }

    }
}
