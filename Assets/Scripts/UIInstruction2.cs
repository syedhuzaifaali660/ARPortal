﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInstruction2 : MonoBehaviour
{
    [SerializeField] private GameObject Scrollrect;
    [SerializeField] private GameObject BackButton;
    [SerializeField] private GameObject InstructionPanel;
    [SerializeField] private GameObject PlaneFinder;
    [SerializeField] private GameObject GotoMainButton;
    [SerializeField] private GameObject InfoButton;
    [SerializeField] private AudioSource ButtonTapAudio;



    public void BackButtonLogic()
    {
        StartCoroutine(Wait());
        Scrollrect.SetActive(true);
        PlaneFinder.SetActive(true);
        GotoMainButton.SetActive(true);
        InfoButton.SetActive(true);

        BackButton.SetActive(false);
        InstructionPanel.SetActive(false);

    }

    public void InfoButtonFunction()
    {
        StartCoroutine(Wait());
        Scrollrect.SetActive(false);
        PlaneFinder.SetActive(false);
        GotoMainButton.SetActive(false);
        InfoButton.SetActive(false);

        BackButton.SetActive(true);
        InstructionPanel.SetActive(true);

    }


    IEnumerator Wait()
    {
        ButtonTapAudio.Play();
        yield return new WaitForSeconds(0.01f);
    }
}
