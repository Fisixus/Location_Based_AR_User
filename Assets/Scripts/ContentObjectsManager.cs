using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContentObjectsManager : MonoBehaviour
{
    public static ContentObjectsManager Instance;

    public GameObject contentObject;
    public List<Texture> symbolIcons;
    public TextMeshProUGUI username;

    GameObject baseSymbolObject;
    GameObject distanceText;
    List<Symbol> symbols = new List<Symbol>();

    public void Awake()
    {
        Instance = this;
    }

    private void Initiliaze()
    {
        baseSymbolObject = contentObject.GetComponentInChildren<Collider>().gameObject;
        distanceText = contentObject.GetComponentInChildren<TextMeshProUGUI>().gameObject;
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
            newContentObject.transform.parent = contentObject.transform;
            //newContentObject.tag = symbol.Category.ToString();
            newContentObject.name = symbol.SymbolName;
            newContentObject.transform.localEulerAngles = new Vector3(-90, 0, 0);
            SymbolTextureSetter(symbol, newContentObject);
            SymbolDistanceTruncater(symbol, newContentObject);
            AdjustScale(newContentObject);
            AdjustDistance(symbol, newContentObject);
        }
    }

    private void SymbolTextureSetter(Symbol symbol, GameObject newContentObject)
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

    private void SymbolDistanceTruncater(Symbol symbol, GameObject newContentObject)
    {
        // decrease the numbers to prevent floating precision limit error

        float latdif = ((float)symbol.Latitude - (float)UserManager.Instance.FindUser(username.text).Latitude) * 100;
        float londif = ((float)symbol.Longitude - (float)UserManager.Instance.FindUser(username.text).Longitude) * 100;
        float altdif = ((float)symbol.Altitude - (float)UserManager.Instance.FindUser(username.text).Altitude) * 1;
        Vector diff = new Vector(londif, altdif, latdif);
        newContentObject.transform.position = diff.toVector3();
        //Debug.Log("Object Name: " + obj.name + "Pos: " + obj.transform.position);
    }

    private void AdjustScale(GameObject newContentObject)
    {
        float symbolDistance = Vector3.Distance(Camera.main.transform.position, newContentObject.transform.position);
        int symbolScaleDistanceRatio = 9;

        if (symbolDistance >= 0 && symbolDistance <= 100)
            symbolScaleDistanceRatio = 30;
        else if (symbolDistance > 100 && symbolDistance <= 150)
            symbolScaleDistanceRatio = 33;
        else if (symbolDistance > 150 && symbolDistance <= 200)
            symbolScaleDistanceRatio = 36;
        else if (symbolDistance > 200 && symbolDistance <= 300)
            symbolScaleDistanceRatio = 39;
        else if (symbolDistance > 300 && symbolDistance <= 400)
            symbolScaleDistanceRatio = 42;
        else if (symbolDistance > 400 && symbolDistance <= 500)
            symbolScaleDistanceRatio = 45;
        else if (symbolDistance > 500)
            symbolScaleDistanceRatio = 48;

        float properScaleValue = symbolDistance / symbolScaleDistanceRatio;

        newContentObject.transform.localScale = new Vector3(properScaleValue, properScaleValue, 1);
    }

    private void AdjustDistance(Symbol symbol, GameObject newContentObject)
    {
        //AttachDistanceInfo
        float distanceToSymbol =
                CoordinateManager.Instance.GetDistanceFromLatLonInMeter((float)UserManager.Instance.FindUser(username.text).Latitude, (float)UserManager.Instance.FindUser(username.text).Longitude, (float)symbol.Latitude, (float)symbol.Longitude);

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
        foreach (Transform _symbols in contentObject.transform)
        {
            if (_symbols.gameObject.name != "BaseSymbolObject")
            {
                Destroy(_symbols.gameObject);
            }
        }
    }
}
