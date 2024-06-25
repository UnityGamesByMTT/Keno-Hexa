using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private Button Random_Button;
    [SerializeField]
    private Button Play_Button;
    [SerializeField]
    private Button AutoPlay_Button;
    [SerializeField]
    private Button StakePlus_Button;
    [SerializeField]
    private Button StakeMinus_Button;
    [SerializeField]
    private Button Reset_Button;
    [SerializeField]
    private Button Delete_Button;
    [SerializeField]
    private Button GameExit_Button;

    [Header("Texts")]

    [SerializeField]
    private TMP_Text Stake_Text;
    //[SerializeField]
    //private TMP_Text Win_Text;

    [Header("Lists")]
    [SerializeField]
    private List<TMP_Text> Payout_Text;
    [SerializeField]
    private List<TMP_Text> Hits_Text;
    [SerializeField]
    private List<GameObject> Win_Objects;

    [Header("GameObjects")]
    [SerializeField]
    private GameObject Reset_Object;
    [SerializeField]
    private GameObject PlayAnim_Object;
    [SerializeField]
    private GameObject CoinValueDisable_object;

    [Header("Scripts")]
    [SerializeField]
    private KenoBehaviour KenoManager;

    private int stake = 5;
    private int winning = 0;
    internal bool isReset = false;

    void Start()
    {
        if (Random_Button) Random_Button.onClick.RemoveAllListeners();
        if (Random_Button) Random_Button.onClick.AddListener(PickRandomIndices);

        if (Play_Button) Play_Button.onClick.RemoveAllListeners();
        if (Play_Button) Play_Button.onClick.AddListener(DummyPlay);

        CheckPlayButton(false);

        if (StakePlus_Button) StakePlus_Button.onClick.RemoveAllListeners();
        if (StakePlus_Button) StakePlus_Button.onClick.AddListener(delegate { ChangeStake(true); });

        if (StakeMinus_Button) StakeMinus_Button.onClick.RemoveAllListeners();
        if (StakeMinus_Button) StakeMinus_Button.onClick.AddListener(delegate { ChangeStake(false); });

        if (Reset_Button) Reset_Button.onClick.RemoveAllListeners();
        if (Reset_Button) Reset_Button.onClick.AddListener(ResetGame);

        if (Delete_Button) Delete_Button.onClick.RemoveAllListeners();
        if (Delete_Button) Delete_Button.onClick.AddListener(CleanButtons);

        if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
        if (GameExit_Button) GameExit_Button.onClick.AddListener(CallOnExitFunction);

        stake = 5;
        winning = 0;
        if (Stake_Text) Stake_Text.text = stake.ToString();
        //if (Win_Text) Win_Text.text = winning.ToString();
        Application.ExternalCall("window.parent.postMessage", "OnEnter", "*");
    }

    private void CallOnExitFunction()
    {
        Application.ExternalCall("window.parent.postMessage", "onExit", "*");
    }

    private void DummyPlay()
    {
        if(isReset)
        {
            ResetGame();
        }
        isReset = true;
        CheckPlayButton(false);
        if (Delete_Button) Delete_Button.interactable = false;
        if (CoinValueDisable_object) CoinValueDisable_object.SetActive(true);
        KenoManager.PlayDummyGame();
    }

    private void ChangeStake(bool type)
    {
        if (type)
        {
            if (stake < 20)
            {
                stake += 5;
                if (Stake_Text) Stake_Text.text = stake.ToString();
            }
        }
        else
        {
            if (stake > 5)
            {
                stake -= 5;
                if (Stake_Text) Stake_Text.text = stake.ToString();
            }
        }
    }

    private void PickRandomIndices()
    {
        if(isReset)
        {
            ResetGame();
        }
        KenoManager.PickRandoms();
    }

    internal void CheckPlayButton(bool isActive)
    {
        if (Play_Button) Play_Button.interactable = isActive;
        if (Random_Button) Random_Button.interactable = isActive;
        if (AutoPlay_Button) AutoPlay_Button.interactable = isActive;
        if (PlayAnim_Object) PlayAnim_Object.SetActive(isActive);

    }

    internal void UpdateSelectedText(int count)
    {
        BetAmountUpdate(count);
    }

    internal void CheckWinnings(int count)
    {
        //if(count >= 2)
        //{
        //    if (Win_Objects[count - 2]) Win_Objects[count - 2].SetActive(true);
        //    //winning += int.Parse(Amount_Text[count - 2].text);
        //}
        //else
        //{
        //    for (int i = 0; i < Win_Objects.Count; i++)
        //    {
        //        if (Win_Objects[i]) Win_Objects[i].SetActive(false);
        //    }
        //    winning = 0;
        //}
        ////if (Win_Text) Win _Text.text = winning.ToString();
    }

    internal void BetAmountUpdate(int count)
    {

        for (int i = 0; i < Payout_Text.Count; i++)
        {
            if (Payout_Text[i]) Payout_Text[i].text = string.Empty;
        }

        for (int i = 0; i < Hits_Text.Count; i++)
        {
            if (Hits_Text[i]) Hits_Text[i].text = string.Empty;
        }
        if (count >= 2)
        {
            for (int i = 2; i <= count; i++)
            {
                if (Hits_Text[i - 2]) Hits_Text[i - 2].text = i.ToString();
                switch (i)
                {
                    case 2:
                        if (Payout_Text[0]) Payout_Text[0].text = (stake * i).ToString();
                        break;
                    case 3:
                        if (Payout_Text[1]) Payout_Text[1].text = (stake * i).ToString();
                        break;
                    case 4:
                        if (Payout_Text[2]) Payout_Text[2].text = (stake * i).ToString();
                        break;
                    case 5:
                        if (Payout_Text[3]) Payout_Text[3].text = (stake * i).ToString();
                        break;
                    case 6:
                        if (Payout_Text[4]) Payout_Text[4].text = (stake * i).ToString();
                        break;
                    case 7:
                        if (Payout_Text[5]) Payout_Text[5].text = (stake * i).ToString();
                        break;
                    case 8:
                        if (Payout_Text[6]) Payout_Text[6].text = (stake * i).ToString();
                        break;
                    case 9:
                        if (Payout_Text[7]) Payout_Text[7].text = (stake * i).ToString();
                        break;
                    case 10:
                        if (Payout_Text[8]) Payout_Text[8].text = (stake * i).ToString();
                        break;
                }
            }
        }
        else
        {
            for (int i = 0; i < Payout_Text.Count; i++)
            {
                if (Payout_Text[i]) Payout_Text[i].text = string.Empty;
            }

            for (int i = 0; i < Hits_Text.Count; i++)
            {
                if (Hits_Text[i]) Hits_Text[i].text = string.Empty;
            }
        }
    }

    internal void EnableReset()
    {
        if (Reset_Object) Reset_Object.SetActive(true);
        if (Delete_Button) Delete_Button.interactable = true;
        if (CoinValueDisable_object) CoinValueDisable_object.SetActive(false);
        CheckPlayButton(true);
    }

    private void ResetGame()
    {
        KenoManager.ResetButtons();
        if (Reset_Object) Reset_Object.SetActive(false);
        isReset = false;
        CheckWinnings(0);
    }

    private void CleanButtons()
    {
        BetAmountUpdate(0);
        UpdateSelectedText(0);
        CheckWinnings(0);
        KenoManager.CleanPage();
        CheckPlayButton(false);
    }

}
