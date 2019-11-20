using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    public GameObject cursor;

    public int scaleRate = 3500;

    Vector3 endPoint;
    Image cursorImage;
    TextMeshProUGUI kmCounter;
    float km;

    public float getKM()
    {
        return km;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cursorImage = cursor.GetComponentInChildren<Image>();
        kmCounter = cursor.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void LateUpdate()
    {
        if (GameObject.Find("/Canvas/AddSymbolPanel")) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!UIManager.Instance.FocusOnSymbolInfoPanel())
            {
                cursor.SetActive(!cursor.activeSelf);
            }
            ///If click focus on symbolInfoPanel then close the cursor
            else
            {
                cursor.SetActive(false);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            UIManager.Instance.ClearUIResults();
        }

        if (cursor.activeSelf)
        {
            endPoint = cursor.transform.position;

            if (Input.GetKey(KeyCode.UpArrow) && km <= 20f)
            {
                cursor.transform.Translate(new Vector3(0, 0, Time.deltaTime * 5));
                km += Time.deltaTime * 5;
                cursorImage.transform.localScale = new Vector3(cursorImage.transform.localScale.x + (km / scaleRate), cursorImage.transform.localScale.y + (km / scaleRate), 1);
            }

            if (Input.GetKey(KeyCode.DownArrow) && km >= 0.03f)
            {
                cursor.transform.Translate(new Vector3(0, 0, -Time.deltaTime * 5));
                km -= Time.deltaTime * 5;
                cursorImage.transform.localScale = new Vector3(cursorImage.transform.localScale.x - (km / scaleRate), cursorImage.transform.localScale.y - (km / scaleRate), 1);
            }

            kmCounter.text = km.ToString("0.00") + " KM";
        }
    }

}
