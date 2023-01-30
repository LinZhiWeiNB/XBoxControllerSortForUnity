using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carinnor.XboxController
{
    public class ControllerSortCanvas : MonoBehaviour
    {
        private Transform Panel;
        private Transform[] PLayerGrids = new Transform[4];
        // Start is called before the first frame update
        void Awake()
        {
            Panel = transform.GetChild(0);
            Panel.gameObject.SetActive(false);
            for (int i = 0; i < 4; i++)
            {
                PLayerGrids[i] = Panel.Find("Players").GetChild(i);
            }
        }
    
        // Update is called once per frame
        public void ShowPanel(int playerNum)
        {
            for (int i = 0; i < playerNum; i++)
            {
                PLayerGrids[i].gameObject.SetActive(true);
                SwitchPlayerGridState(PlayerGridState.Empty, PLayerGrids[i]);
            }
    
            for (int i = playerNum; i < 4; i++)
            {
                PLayerGrids[i].gameObject.SetActive(false);
            }
            Panel.gameObject.SetActive(true);
        }
        
        
        public void SwitchPlayerGridState(PlayerGridState state, Transform grid)
        {
            switch (state)
            {
                case PlayerGridState.Empty:
                    grid.GetChild(0).gameObject.SetActive(false);
                    grid.GetChild(2).gameObject.SetActive(false);
                    grid.GetChild(3).gameObject.SetActive(true);
                    break;
                case PlayerGridState.Touched:
                    grid.GetChild(0).gameObject.SetActive(false);
                    grid.GetChild(2).gameObject.SetActive(true);
                    grid.GetChild(3).gameObject.SetActive(false);
                    break;
                case PlayerGridState.Confirmed:
                    grid.GetChild(0).gameObject.SetActive(true);
                    grid.GetChild(2).gameObject.SetActive(true);
                    grid.GetChild(3).gameObject.SetActive(false);
                    break;
            }
        }
        public void SwitchPlayerGridState(PlayerGridState state, int gridIndex)
        {
            var grid = PLayerGrids[gridIndex];
            switch (state)
            {
                case PlayerGridState.Empty:
                    grid.GetChild(0).gameObject.SetActive(false);
                    grid.GetChild(2).gameObject.SetActive(false);
                    grid.GetChild(3).gameObject.SetActive(true);
                    break;
                case PlayerGridState.Touched:
                    grid.GetChild(0).gameObject.SetActive(false);
                    grid.GetChild(2).gameObject.SetActive(true);
                    grid.GetChild(3).gameObject.SetActive(false);
                    break;
                case PlayerGridState.Confirmed:
                    grid.GetChild(0).gameObject.SetActive(true);
                    grid.GetChild(2).gameObject.SetActive(true);
                    grid.GetChild(3).gameObject.SetActive(false);
                    break;
            }
        }

        public void ClosePanel()
        {
            Panel.gameObject.SetActive(false);
        }
    }
    
    public enum PlayerGridState
    {
        Empty,
        Touched,
        Confirmed,
    }
}
