using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ButtonCustom : MonoBehaviour
{

    [SerializeField] private PlaneFinderBehaviour Planefinder;
    [SerializeField] private AnchorInputListenerBehaviour Listenerbehaviour;

    public GameObject Gobj1, Gobj2;
    public void Button1()
    {
        Gobj1.SetActive(true);
    }

    public void Button2()
    {
        Gobj2.SetActive(true);
    }

    public void Button3()
    {
        Planefinder.enabled = true;
        Listenerbehaviour.enabled = true;
    }
}
