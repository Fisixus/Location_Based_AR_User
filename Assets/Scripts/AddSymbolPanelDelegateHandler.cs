using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSymbolPanelDelegateHandler : MonoBehaviour
{
    public delegate void OnSpacePressedDelegate();
    public static event OnSpacePressedDelegate spacePressedDelegate;
   
}
