using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContentObjectsManager : MonoBehaviour
{
    public static ContentObjectsManager Instance;

    public GameObject deleteContentObjectConfirmation;
    public GameObject selectedSymbolInfoPanel;
    public GameObject contentObjects;
    public List<Texture> symbolIcons;

    GameObject baseSymbolObject;
    GameObject distanceText;
    GameObject selectedSymbolObject;
    List<Symbol> symbols = new List<Symbol>();
    string selectedSymbolName;

    public bool isContentObjectCreated = false;

    public void Awake()
    {
        Instance = this;
    }

    private void Initiliaze()
    {
        baseSymbolObject = contentObjects.GetComponentInChildren<Collider>().gameObject;
        distanceText = contentObjects.GetComponentInChildren<TextMeshProUGUI>().gameObject;
    }

    private void Start()
    {
        Initiliaze();
        
    }

    public void ContentObjectCreator()
    {
        DestroyContentObjects();
        string data = WebServiceManager.Instance.getSymbolsData();
        symbols = JsonConvert.DeserializeObject<List<Symbol>>(data);

        foreach(Symbol symbol in symbols)
        {
            GameObject newContentObject = Instantiate(baseSymbolObject) as GameObject;
            //cubeNew.GetComponent<Renderer>().enabled = true;
            //cubeNew.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
            newContentObject.SetActive(true);
            newContentObject.transform.parent = contentObjects.transform;
            //newContentObject.tag = symbol.Category.ToString();
            newContentObject.name = symbol.SymbolName;
            newContentObject.transform.localEulerAngles = new Vector3(-90, 0, 0);
            AdjustGameObjectPlacement(symbol, newContentObject);
            AdjustTexture(symbol, newContentObject);
            AdjustScale(newContentObject);
            AdjustDistance(symbol, newContentObject);

            ///When the selectedSymbolObject refreshed, dont lose the focus on info panel and border
            if (newContentObject.name.Equals(selectedSymbolName))
            {
                selectedSymbolObject = newContentObject;
                selectedSymbolObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        isContentObjectCreated = true;
    }

    private void AdjustGameObjectPlacement(Symbol symbol, GameObject newContentObject)
    {
        // decrease the numbers to prevent floating precision limit error
        /*
        float latdif = ((float)symbol.Latitude - (float)UserManager.Instance.FindUser(UIManager.Instance.getUsername()).Latitude) * 100;
        float londif = ((float)symbol.Longitude - (float)UserManager.Instance.FindUser(UIManager.Instance.getUsername()).Longitude) * 100;
        float altdif = ((float)symbol.Altitude - (float)UserManager.Instance.FindUser(UIManager.Instance.getUsername()).Altitude) * 1;
        */
        float latdif = (float)symbol.Latitude * 100;
        float londif = (float)symbol.Longitude * 100;
        float altdif = (float)symbol.Altitude * 1;

        Vector diff = new Vector(londif, altdif, latdif);
        newContentObject.transform.position = diff.toVector3();
        //Debug.Log("Object Name: " + obj.name + "Pos: " + obj.transform.position);
    }

    private void AdjustTexture(Symbol symbol, GameObject newContentObject)
    {
        switch (symbol.Category)
        {
            case Category.Ambulance:
                SetImage(newContentObject, 0);
                break;
            case Category.Barrier:
                SetImage(newContentObject, 1);
                break;
            case Category.Blast:
                SetImage(newContentObject, 2);
                break;
            case Category.Bomb:
                SetImage(newContentObject, 3);
                break;
            case Category.Car:
                SetImage(newContentObject, 4);
                break;
            case Category.Construction:
                SetImage(newContentObject, 5);
                break;
            case Category.Ditch:
                SetImage(newContentObject, 6);
                break;
            case Category.Fire:
                SetImage(newContentObject, 7);
                break;
            case Category.Firstaid:
                SetImage(newContentObject, 8);
                break;
            case Category.Hunting:
                SetImage(newContentObject, 9);
                break;
            case Category.Male:
                SetImage(newContentObject, 10);
                break;
            case Category.Office_building:
                SetImage(newContentObject, 11);
                break;
            case Category.Police:
                SetImage(newContentObject, 12);
                break;
            case Category.Soldier:
                SetImage(newContentObject, 13);
                break;
            case Category.Traffic_light:
                SetImage(newContentObject, 14);
                break;
            default:
                break;
        }
    }
    
    private void SetImage(GameObject newContentObject, int textureNo)
    {
        newContentObject.GetComponent<Renderer>().material.mainTexture = symbolIcons[textureNo];
    }

    private void AdjustScale(GameObject newContentObject)
    {
        float symbolDistance = Vector3.Distance(Camera.main.transform.parent.position, newContentObject.transform.position);
        int symbolScaleDistanceRatio = 9;

        if (symbolDistance >= 0 && symbolDistance <= 100)
            symbolScaleDistanceRatio = 18;
        else if (symbolDistance > 100 && symbolDistance <= 150)
            symbolScaleDistanceRatio = 21;
        else if (symbolDistance > 150 && symbolDistance <= 200)
            symbolScaleDistanceRatio = 24;
        else if (symbolDistance > 200 && symbolDistance <= 300)
            symbolScaleDistanceRatio = 27;
        else if (symbolDistance > 300 && symbolDistance <= 400)
            symbolScaleDistanceRatio = 30;
        else if (symbolDistance > 400 && symbolDistance <= 500)
            symbolScaleDistanceRatio = 33;
        else if (symbolDistance > 500)
            symbolScaleDistanceRatio = 36;

        float properScaleValue = symbolDistance / symbolScaleDistanceRatio;

        newContentObject.transform.localScale = new Vector3(properScaleValue, properScaleValue, 1);
    }

    private void AdjustDistance(Symbol symbol, GameObject newContentObject)
    {
        float distanceToSymbol =
                CoordinateManager.Instance.GetDistanceFromLatLonInMeter((float)UserManager.Instance.FindUser(UIManager.Instance.getUsername()).Latitude, (float)UserManager.Instance.FindUser(UIManager.Instance.getUsername()).Longitude, (float)symbol.Latitude, (float)symbol.Longitude);
        Debug.Log("Distance:" + distanceToSymbol);

        if (newContentObject.transform.GetChild(0) != null)
        {
            if (distanceToSymbol < 1000.0f)
            {
                float inMeter = distanceToSymbol;
               newContentObject.transform.GetComponentInChildren<TextMeshProUGUI>().text = inMeter.ToString("F1") + "\n" + "metre";
            }
            else
            {
                float inKm = distanceToSymbol / 1000;
               newContentObject.transform.GetComponentInChildren<TextMeshProUGUI>().text = inKm.ToString("F2") + "\n" + "km";
            }
        }
    }
    

    public void DestroyContentObjects()
    {        
        symbols.Clear();
        foreach (Transform _symbols in contentObjects.transform)
        {
            if (_symbols.gameObject.name != "BaseSymbolObject")
            {
                Destroy(_symbols.gameObject);
            }
        }
        isContentObjectCreated = false;
    }


    /// Clicked event for the cross button on content object
    public void DeleteContentObject()
    {
        selectedSymbolName = string.Empty;
        GameObject gObj = EventSystem.current.currentSelectedGameObject;
        selectedSymbolName = gObj.GetComponentInParent<Collider>().gameObject.name;
        string username = UIManager.Instance.getUsername();

        deleteContentObjectConfirmation.SetActive(true);
    }

    public void DeleteContentObjectConfirm()
    {
        string username = UIManager.Instance.getUsername();
        WebServiceManager.Instance.DeleteSymbol(SymbolManager.Instance.FindSymbolByName(selectedSymbolName).getUUID, UserManager.Instance.FindUserUUIDbyUsername(username));
        CloseSymbolConfirmationWindow();

        
        Invoke("CallRefreshUserSymbols", 3f);
    }

    private void CallRefreshUserSymbols()
    {
        SymbolManager.Instance.RefreshUserSymbols();
    }


    public void DeleteContentObjectCancel()
    {
        CloseSymbolConfirmationWindow();
    }

    public void CloseSymbolConfirmationWindow()
    {
        deleteContentObjectConfirmation.SetActive(false);
    }
    /// Place and distance should change dynamically, there is no need to destroy and create every time for this
    /// However  POST and GET required, and that reason dont work dynamically
    /*
    public void DynamicallyAdjustPlaceAndDistance()
    {
        foreach (Transform _symbols in contentObjects.transform)
        {
            if (_symbols.gameObject.name != "BaseSymbolObject")
            {
                Symbol symbol = SymbolManager.Instance.FindSymbolByName(_symbols.gameObject.name);
                if(symbol != null)
                {
                    AdjustGameObjectPlacement(symbol, _symbols.gameObject);
                    AdjustScale(_symbols.gameObject);
                    AdjustDistance(symbol, _symbols.gameObject);
                }
            }
        }
    }
    
    
    private void LateUpdate()
    {
        if (isContentObjectCreated)
        {
            DynamicallyAdjustPlaceAndDistance();
        }
    }
    */
    private void Update()
    {

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetMouseButton(0))
#elif (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
#endif
        {
            ///When click anything except symbolinfoPanel dismiss the focus on symbolinfoPanel
            if (!UIManager.Instance.FocusOnSymbolInfoPanel())
            {
                if (selectedSymbolObject != null)
                {
                    selectedSymbolObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                    selectedSymbolInfoPanel.SetActive(false);
                    selectedSymbolObject = null;
                    selectedSymbolName = string.Empty;
                }
            }
            Ray ray;
            if(Application.isEditor)
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            else
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            RaycastHit hitPoint;

            if (Physics.Raycast(ray, out hitPoint))
            {
                selectedSymbolObject = hitPoint.transform.gameObject;
                selectedSymbolName = selectedSymbolObject.name;
                Debug.Log(selectedSymbolObject.name);
                Symbol symbol = null;
                if (selectedSymbolObject != null)
                {                    
                    symbol = SymbolManager.Instance.FindSymbolByName(selectedSymbolObject.name);
                }
                if(symbol != null)
                {
                    selectedSymbolObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                    selectedSymbolInfoPanel.SetActive(true);
                    UIManager.Instance.AutoLoadSymbolInfoPanel(symbol);
                }
            }

        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonUp(0))
#elif (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if (Input.touches[0].phase == TouchPhase.Ended)
#endif
        {
            UIManager.Instance.ClearUIResults();
        }
    }    
}
