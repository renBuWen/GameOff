﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InGameUI : UIPanel
{
    enum PLAYER_ID { ONE, TWO, THREE, FOUR };
    private GameObject m_HPPrefab;
    
    private int m_PlayerCount = 0;

    public InGameUI()
    {
        SetupUI();
    }

    public override void SetupUI()
    {
        PanelObj = UIManager.MainCanvas.transform.Find("InGameUI").gameObject;
        m_HPPrefab = Resources.Load<GameObject>("PlayerHP");
    }

    public void CreatePanel(string playerName)
    {
        PlayerPanel Panel = new PlayerPanel(m_HPPrefab, this.PanelObj.transform);

        m_PlayerCount++;

        switch (m_PlayerCount)
        {
            case 1:
                Panel.SetPosition(PanelPositions.Player1Center);
                break;
            case 2:
                Panel.SetPosition(PanelPositions.Player2Center);
                break;
            case 3:
                Panel.SetPosition(PanelPositions.Player3Center);
                break;
            case 4:
                Panel.SetPosition(PanelPositions.Player4Center);
                break;
        }

    }

    private class PlayerPanel
    {
        private GameObject m_ParentObj;
        private Image HPbar;
        private TMPro.TextMeshProUGUI PlayerName;
        public Vector2 PlayerPosition;

        public PlayerPanel(GameObject prefab, Transform parent)
        {
            m_ParentObj = GameObject.Instantiate(prefab);
            m_ParentObj.transform.parent = parent;
            PlayerName = m_ParentObj.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
            HPbar = m_ParentObj.transform.Find("Bar level").GetComponent<Image>();

        }

        public void SetPosition(Vector2 Position)
        {
            PlayerPosition = Position;
        }

        private void SetPlayerName(string Player_ID)
        {
            PlayerName.text = "Player " + Player_ID;
        }

        private void UpdateHPBar(float hpPercentage)
        { 
            HPbar.fillAmount = hpPercentage;
        }
        
    }

    private struct PanelPositions
    {
        public static Vector2 Player1Center = new Vector2(370 / 2, Screen.height - (55 / 2));
        public static Vector2 Player2Center = new Vector2(Screen.width - 370 / 2, Screen.height - (55 / 2));
        public static Vector2 Player3Center = new Vector2(370 / 2, 55 / 2);
        public static Vector2 Player4Center = new Vector2(Screen.width - 370 / 2, 55 / 2);
    }

}

