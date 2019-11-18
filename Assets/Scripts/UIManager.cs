using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI UserLatitudeDATA;
    public TextMeshProUGUI UserLongitudeDATA;
    public TextMeshProUGUI UserAltitudeDATA;
    public GameObject addSymbolPanel;
    public TextMeshProUGUI username;

    public Canvas canvas;
    private EventSystem m_EventSystem;
    private PointerEventData m_PointerEventData;
    private GraphicRaycaster m_Raycaster;

    List<RaycastResult> UIResults = new List<RaycastResult>();

    private void Awake()
    {
        Instance = this;
    }

    public string getUsername()
    {
        return username.text;
    }

    void Start()
    {
        m_Raycaster = canvas.GetComponent<GraphicRaycaster>();
        m_PointerEventData = new PointerEventData(m_EventSystem);
        username.text = PlayerPrefs.GetString("Username");
    }

    public bool FocusOnSymbolInfoPanel()
    {        
        m_PointerEventData.position = Input.mousePosition;
        m_Raycaster.Raycast(m_PointerEventData, UIResults);

        foreach(RaycastResult result in UIResults)
        {
            if (result.gameObject.name.Equals("SymbolInfo"))
            {
                return true;
            }
        }
        return false;
    }

    public void ClearUIResults()
    {
        UIResults.Clear();
    }

    public void AutoLoadLatLotAltPanel(User onlineUser)
    {
        UserLatitudeDATA.text = onlineUser.Latitude.ToString();
        UserLongitudeDATA.text = onlineUser.Longitude.ToString();
        UserAltitudeDATA.text = onlineUser.Altitude.ToString();
    }

    public void AutoLoadSymbolInfoPanel(Symbol symbol)
    {
        GameObject.Find("/Canvas/SymbolInfo/ScrollView/ContentPanel/SymbolNameDATA").GetComponent<TextMeshProUGUI>().text = symbol.SymbolName;
        GameObject.Find("/Canvas/SymbolInfo/ScrollView/ContentPanel/CategoryDATA").GetComponent<TextMeshProUGUI>().text = symbol.Category.ToString();
        GameObject.Find("/Canvas/SymbolInfo/ScrollView/ContentPanel/LatitudeDATA").GetComponent<TextMeshProUGUI>().text = symbol.Latitude.ToString();
        GameObject.Find("/Canvas/SymbolInfo/ScrollView/ContentPanel/LongitudeDATA").GetComponent<TextMeshProUGUI>().text = symbol.Longitude.ToString();
        GameObject.Find("/Canvas/SymbolInfo/ScrollView/ContentPanel/AltitudeDATA").GetComponent<TextMeshProUGUI>().text = symbol.Altitude.ToString();
        GameObject.Find("/Canvas/SymbolInfo/ScrollView/ContentPanel/MessageDATA").GetComponent<TextMeshProUGUI>().text = symbol.Message;
    }

    public void AutoLoadtoAddPanel(string textureName, LatLonH latlon)
    {
        addSymbolPanel.transform.Find("ScrollView/ContentPanel/CategoryDATA").GetComponent<InputField>().text = textureName;

        //TODO there need to some math functions for that.
        addSymbolPanel.transform.Find("ScrollView/ContentPanel/LatitudeDATA").GetComponent<InputField>().text = latlon.getLatitude().ToString();
        addSymbolPanel.transform.Find("ScrollView/ContentPanel/LongitudeDATA").GetComponent<InputField>().text = latlon.getLongitude().ToString();
        addSymbolPanel.transform.Find("ScrollView/ContentPanel/AltitudeDATA").GetComponent<InputField>().text = latlon.getAltitude().ToString();
    }


}
