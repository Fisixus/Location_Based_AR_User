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
        if (data.Equals(""))
        {
            Debug.Log("Waiting connection!");
            return;
        }
        symbols = JsonConvert.DeserializeObject<List<Symbol>>(data);

        foreach(Symbol symbol in symbols)
        {
            GameObject newContentObject = Instantiate(baseSymbolObject) as GameObject;
            newContentObject.transform.parent = contentObjects.transform;
            //newContentObject.tag = symbol.Category.ToString();
            newContentObject.name = symbol.SymbolName;
            
            //newContentObject.transform.localEulerAngles = new Vector3(-90, 0, 0);
            GiveContentObjectName(newContentObject);
            AdjustGameObjectLocation(symbol, newContentObject);
            AdjustGameObjectRotation(newContentObject);
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

    private void GiveContentObjectName(GameObject newContentObject)
    {
        foreach (TextMeshProUGUI symbolNameText in newContentObject.transform.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (symbolNameText.gameObject.name.Equals("SymbolNameText"))
            {
                symbolNameText.text = newContentObject.name;
            }
        }
    }

    private void AdjustGameObjectLocation(Symbol symbol, GameObject newContentObject)
    {
        LatLonH latlon = new LatLonH((float)symbol.Longitude, (float)symbol.Latitude, (float)symbol.Altitude);
        Vector diff = CoordinateManager.Instance.ToWorldCoord(latlon);
        newContentObject.transform.position = diff.toVector3();

        ///Dont use because of the unity floating point precision
        /*
        float latdif = (float)symbol.Latitude * 100;
        float londif = (float)symbol.Longitude * 100;
        float altdif = (float)symbol.Altitude * 1;

        Vector diff = new Vector(londif, altdif, latdif);
        newContentObject.transform.position = diff.toVector3();
        */
    }

    private void AdjustGameObjectRotation(GameObject newContentObject)
    {
        Vector3 relativePos = Camera.main.transform.position - newContentObject.transform.position;
        relativePos = relativePos * -1;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        rotation.x = 0;
        rotation.z = 0;
        newContentObject.transform.rotation = rotation;
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
        ///Dont use FindUser because want to see that update of distance instantly
        //User user = UserManager.Instance.FindUser(UIManager.Instance.getUsername());
        User user = UserManager.Instance.getOnlineUser();
        if(user != null)
        {
            float distanceToSymbol =
                    CoordinateManager.Instance.GetDistanceFromLatLonInMeter((float)user.Latitude, (float)user.Longitude, (float)symbol.Latitude, (float)symbol.Longitude);
            //Debug.Log("Distance:" + distanceToSymbol);

            if (newContentObject.transform.GetChild(0) != null)
            {
                if (distanceToSymbol < 1000.0f)
                {
                    float inMeter = distanceToSymbol;
                    foreach(TextMeshProUGUI distanceText in newContentObject.transform.GetComponentsInChildren<TextMeshProUGUI>())
                    {
                        if (distanceText.gameObject.name.Equals("DistanceText"))
                        {
                            distanceText.text = inMeter.ToString("F1") + "\n" + "metre";
                        }
                    }
                }
                else
                {
                    float inKm = distanceToSymbol / 1000;
                    foreach (TextMeshProUGUI distanceText in newContentObject.transform.GetComponentsInChildren<TextMeshProUGUI>())
                    {
                        if (distanceText.gameObject.name.Equals("DistanceText"))
                        {
                            distanceText.text = inKm.ToString("F2") + "\n" + "km";
                        }
                    }
                }
            }

        }
        else
        {
            Debug.Log("Connected user cannot find!");
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
        CloseSymbolConfirmationWindow();

        Symbol deletingSymbol = SymbolManager.Instance.FindSymbolByName(selectedSymbolName);
        if(deletingSymbol != null)
        {
            WebServiceManager.Instance.DeleteSymbol(deletingSymbol.getUUID, UserManager.Instance.FindUserUUIDbyUsername(username));
        }
        else
        {
            Debug.Log("Delete ERROR!");
        }
        
        //Invoke("CallRefreshUserSymbols", 3f);
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
    public void DynamicallyAdjustRotationAndDistance()
    {
        foreach (Transform _symbols in contentObjects.transform)
        {
            if (_symbols.gameObject.name != "BaseSymbolObject")
            {
                Symbol symbol = SymbolManager.Instance.FindSymbolByName(_symbols.gameObject.name);
                if(symbol != null)
                {
                    //AdjustGameObjectLocation(symbol, _symbols.gameObject);
                    AdjustGameObjectRotation(_symbols.gameObject);
                    //AdjustScale(_symbols.gameObject);
                    AdjustDistance(symbol, _symbols.gameObject);
                }
            }
        }
    }
    
    private void LateUpdate()
    {
        if (isContentObjectCreated)
        {
            DynamicallyAdjustRotationAndDistance();
        }
    }
    
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ///When click anything except symbolinfoPanel dismiss the focus on symbolinfoPanel
            if (!UIManager.Instance.FocusOnSymbolInfoPanel())
            {
                if (selectedSymbolObject != null)
                {
                    selectedSymbolObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                    selectedSymbolInfoPanel.SetActive(false);
                    selectedSymbolObject = null;
                    //selectedSymbolName = string.Empty;
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitPoint;

            if (Physics.Raycast(ray, out hitPoint))
            {
                selectedSymbolObject = hitPoint.transform.gameObject;
                selectedSymbolName = selectedSymbolObject.name;
                Debug.Log("Selected Symbol:" + selectedSymbolObject.name);
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

        if (Input.GetMouseButtonUp(0))
        {
            UIManager.Instance.ClearUIResults();
        }
    }    
}
