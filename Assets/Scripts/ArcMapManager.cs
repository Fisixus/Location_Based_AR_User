using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcMapManager : MonoBehaviour
{
    public static ArcMapManager Instance;

    public GameObject contentSymbols;
    public GameObject arcCircleMain;

    GameObject arcCircleUserOrigin;
    GameObject miniSymbolTemplate;

    List<GameObject> allMiniSymbols = new List<GameObject>();
    float angleDiff;
    bool isMiniSymbolsCreated = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Initialize()
    {
        arcCircleUserOrigin = arcCircleMain.transform.GetChild(0).gameObject;
        miniSymbolTemplate = arcCircleMain.transform.GetChild(1).gameObject;
    }

    private void Start()
    {
        Initialize();
    }

    public void MiniSymbolCreator()
    {
        DestroyArcMapIcons();
        foreach (Transform _symbols in contentSymbols.transform)
        {
            if (_symbols.gameObject.name != "BaseSymbolObject")
            {
                GameObject gObj = Instantiate(miniSymbolTemplate, arcCircleMain.transform) as GameObject;
                gObj.name = "mini_" + _symbols.name;
                Image _img = gObj.transform.GetChild(0).transform.GetComponent<Image>();

                Texture2D _tex = _symbols.GetComponent<Renderer>().material.mainTexture as Texture2D;
                Sprite _sprite = Sprite.Create(_tex, new Rect(0, 0, _tex.width, _tex.height), _img.transform.position);
                Debug.Log("Sprite Name: " + _tex.name);
                _img.sprite = _sprite;

                allMiniSymbols.Add(gObj);

                Vector3 target = _symbols.transform.position - Camera.main.transform.parent.position;
                Vector3 camera = Camera.main.transform.parent.forward;

                angleDiff = Vector3.SignedAngle(camera, target, -Vector3.up);
                gObj.transform.localEulerAngles = new Vector3(0, 0, angleDiff);
            }
        }

        isMiniSymbolsCreated = true;
    }


    public void DestroyArcMapIcons()
    {
        allMiniSymbols.Clear();
        for (int i = 2; i < arcCircleMain.transform.childCount; i++)
        {
            Destroy(arcCircleMain.transform.GetChild(i).gameObject);
        }
        isMiniSymbolsCreated = false;
    }

    //When user rotates then arcmap icons rotates with him
    void Update()
    {
        if (isMiniSymbolsCreated)
        {
            foreach (Transform _symbols in contentSymbols.transform)
            {
                if (_symbols.gameObject.name != "BaseSymbolObject")
                {
                    Vector3 target = _symbols.transform.position - Camera.main.transform.parent.position;
                    Vector3 camera = Camera.main.transform.parent.forward;

                    angleDiff = Vector3.SignedAngle(camera, target, -Vector3.up);

                    GameObject miniObj = allMiniSymbols.Find(x => x.name.Contains(_symbols.gameObject.name));
                    miniObj.transform.localEulerAngles = new Vector3(0, 0, angleDiff);

                }
            }

        }
    }


}
