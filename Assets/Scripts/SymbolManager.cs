﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SymbolManager : MonoBehaviour
{
    public static SymbolManager Instance;

    public List<Texture> textureList;
    public GameObject changeSymbolCategoryIcon;
    public GameObject addSymbolPanel;

    int textureIndex = 0;
    RawImage changedImage;
    TextMeshProUGUI changedImageCategoryName;

    private void Awake()
    {
        Instance = this;
    }

    private void Initiliaze()
    {
        changedImage = changeSymbolCategoryIcon.GetComponentInChildren<RawImage>();
        changedImageCategoryName = changeSymbolCategoryIcon.GetComponentInChildren<TextMeshProUGUI>();

        changedImage.texture = textureList[0];
        changedImageCategoryName.text = Category.Ambulance.ToString();
    }

    private void Start()
    {
        Initiliaze();      
        ///It is repeating because admin assign or delete symbol from user too
        InvokeRepeating("RefreshUserSymbols", 1f, 5f);
    }

    public void RefreshUserSymbols()
    {
        ContentObjectsManager.Instance.isContentObjectCreated = false;
        //ArcMapManager.Instance.isMiniSymbolsCreated = false;
        CancelInvoke("RefreshContentIcons");
        CancelInvoke("RefreshArcMap");
        WebServiceManager.Instance.GetSymbols(UserManager.Instance.FindUserUUIDbyUsername(UIManager.Instance.getUsername()));
        Invoke("RefreshContentIcons", 3.5f);
        Invoke("RefreshArcMap", 4.5f);
    }

    private void RefreshContentIcons()
    {
        ContentObjectsManager.Instance.ContentObjectCreator();
    }

    private void RefreshArcMap()
    {
        ArcMapManager.Instance.MiniSymbolCreator();
    }

    /// When the clicked to add button event, in another name AddContentObject
    public void AddSymbol()
    {
        Category category;
        decimal result;

        string symbolNameDATA = addSymbolPanel.transform.Find("ScrollView/ContentPanel/SymbolNameDATA").GetComponent<InputField>().text.Trim();

        string latitudeDATA = addSymbolPanel.transform.Find("ScrollView/ContentPanel/LatitudeDATA").GetComponent<InputField>().text.Trim();
        string longitudeDATA = addSymbolPanel.transform.Find("ScrollView/ContentPanel/LongitudeDATA").GetComponent<InputField>().text.Trim();
        string altitudeDATA = addSymbolPanel.transform.Find("ScrollView/ContentPanel/AltitudeDATA").GetComponent<InputField>().text.Trim();
        string messageDATA = addSymbolPanel.transform.Find("ScrollView/ContentPanel/MessageDATA").GetComponentInChildren<InputField>().text.Trim();

        ///categorydata is non-editable in the panel
        string categoryDATA = textureList[textureIndex].name;

        ///control for unique symbol names for the online user
        bool nameIsValid = ControlNameIsValid(symbolNameDATA);


        bool postControl = (decimal.TryParse(longitudeDATA.ToString().Trim(), out result)) && (decimal.TryParse(latitudeDATA.ToString().Trim(), out result)) && (decimal.TryParse(altitudeDATA.ToString().Trim(), out result)) && (Enum.TryParse(categoryDATA, out category)) && (nameIsValid) && (!symbolNameDATA.Equals(""));
        /*
        Debug.Log("longControl:" + (decimal.TryParse(longitudeDATA.ToString().Trim(), out result)));
        Debug.Log("latControl:" + (decimal.TryParse(latitudeDATA.ToString().Trim(), out result)));
        Debug.Log("altControl:" + (decimal.TryParse(altitudeDATA.ToString().Trim(), out result)));
        Debug.Log("categoryControl:" + (Enum.TryParse(categoryDATA, out category)));
        Debug.Log("NameNull:" + (symbolNameDATA != null));
        Debug.Log("NameValid:" + (nameIsValid));
        */
        if (postControl)
        {
            Symbol symbol = new Symbol();
            symbol.SymbolName = symbolNameDATA;
            //Debug.Log("SymbolName:" + symbolNameDATA);
            symbol.Latitude = decimal.Parse(latitudeDATA.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat);
            //Debug.Log("LatData:" + latitudeDATA);
            symbol.Longitude = decimal.Parse(longitudeDATA.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat);
            //Debug.Log("LongData:" + longitudeDATA);
            symbol.Altitude = decimal.Parse(altitudeDATA.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat);
            symbol.Category = (Category)Enum.Parse(typeof(Category), categoryDATA);
            if (messageDATA.Equals("")) messageDATA = "-";
            symbol.Message = messageDATA;
            //Debug.Log("MessageData:" + messageDATA);
            //Debug.Log("SymbolOwner:" + symbolOwnerDATA);
            symbol.UserUUID = UserManager.Instance.FindUserUUIDbyUsername(UIManager.Instance.getUsername());
            //Debug.Log("UserUUID:" + symbol.UserUUID);

            //WebServiceManager.Instance.AddSymbol(symbol);
            //Invoke("RefreshUserSymbols", 3f);
        }

        else
        {
            Debug.Log("This request is not eligible!");
        }
    }

    public void ChangeSymbolImage()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (textureIndex == 0)
            {
                textureIndex = textureList.Count - 1;
            }
            else
            {
                textureIndex--;
            }
            UpdateImage();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (textureIndex == (textureList.Count - 1))
            {
                textureIndex = 0;
            }
            else
            {
                textureIndex++;
            }
            UpdateImage();
        }
    }

    private void UpdateImage()
    {
        changedImage.texture = textureList[textureIndex];
        changedImageCategoryName.text = textureList[textureIndex].name;
    }

    public void CloseAddSymbolPanel()
    {
        addSymbolPanel.SetActive(false);
    }

    private bool ControlNameIsValid(string symbolNameDATA)
    {
        bool nameIsValid = true;
       
        string data = WebServiceManager.Instance.getSymbolsData();
        List<Symbol> allUserSymbols= JsonConvert.DeserializeObject<List<Symbol>>(data);

        foreach(Symbol symbol in allUserSymbols)
        {
            if (symbol.SymbolName.ToLower().Trim().Equals(symbolNameDATA.ToLower().Trim()))
            {
                nameIsValid = false;
                break;
            }
        }

        return nameIsValid;
    }

    public Symbol FindSymbolByName(string name)
    {
        Symbol symbol = null;
        string data = WebServiceManager.Instance.getSymbolsData();
        List<Symbol> userSymbols = JsonConvert.DeserializeObject<List<Symbol>>(data);
        for (int i = 0; i < userSymbols.Count; i++)
        {
            if (userSymbols[i].SymbolName.ToLower().Equals(name.ToLower()))
            {
                symbol = userSymbols[i];
            }
        }
        return symbol;
    }

    private void Update()
    {
        if (addSymbolPanel.activeSelf) return;

        ///For opening and closing changeImage
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonDown(0))
#elif (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
#endif
        {
            if (!UIManager.Instance.FocusOnSymbolInfoPanel())
            {
                changeSymbolCategoryIcon.SetActive(!changeSymbolCategoryIcon.activeSelf);
            }
            ///If click focus on symbolInfoPanel then close the changeSymbolCategoryIcon
            else
            {
                changeSymbolCategoryIcon.SetActive(false);
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

        if (changeSymbolCategoryIcon.activeSelf)
        {
            ChangeSymbolImage();

            ///For adding symbol 
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Space))
            {
                addSymbolPanel.SetActive(true);
                LatLonH latlon = new LatLonH(31.43434f, 43.345454545f, 0.0f);
                User user = UserManager.Instance.FindUser(UIManager.Instance.getUsername());
                //LatLonH latlon2 = CoordinateManager.Instance.GetSecondLatLonPosByDistanceBearingAndFirstLatLonPos((float)user.Latitude, (float)user.Longitude, 7000f, 90f);
                UIManager.Instance.AutoLoadtoAddPanel(textureList[textureIndex].name, latlon);
            }
        }
    }
}
