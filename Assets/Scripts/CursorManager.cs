using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public GameObject cursor;

    public int scaleRate = 3500;

    Vector3 endPoint;
    Image cursorImage;
    TextMeshProUGUI kmCounter;
    float km;


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
            cursor.SetActive(!cursor.activeSelf);
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
