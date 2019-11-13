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
        InvokeRepeating("RefreshUserSymbols", 1f, 20f);
    }

    private void RefreshUserSymbols()
    {
        WebServiceManager.Instance.GetSymbols(UserManager.Instance.FindUserUUIDbyUsername(UIManager.Instance.getUsername()));
        Invoke("RefreshContentIcons", 4f);
        Invoke("RefreshArcMap", 7f);
    }

    public void RefreshContentIcons()
    {
        ContentObjectsManager.Instance.ContentObjectCreator();
    }

    public void RefreshArcMap()
    {
        ArcMapManager.Instance.MiniSymbolCreator();
    }

    private void Update()
    {
        if (addSymbolPanel.activeSelf) return;

        ///For opening and closing changeImage
        if (Input.GetMouseButtonDown(1))
        {
            changeSymbolCategoryIcon.SetActive(!changeSymbolCategoryIcon.activeSelf);
        }

        if (changeSymbolCategoryIcon.activeSelf)
        {
            ChangeSymbolImage();

            ///For adding symbol 
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Space))
            {
                addSymbolPanel.SetActive(true);
                UIManager.Instance.AutoLoadtoAddPanel(textureList[textureIndex].name);
                ///There is an add button event for the final process
            }
        }
    }

    


    /// When the clicked to add button event
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

            WebServiceManager.Instance.AddSymbol(symbol);
            //TODO NEED ARCMAP REFRESH
            //TODO NEED CONTENT REFRESH
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
}
