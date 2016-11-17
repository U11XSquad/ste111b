using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Prototype.NetworkLobby
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour
    {
        public LobbyManager lobbyManager;

        public RectTransform lobbyPanel;

        public InputField ipInput;

        public void OnEnable()
        {
            lobbyManager.topPanel.ToggleVisibility(true);

            ipInput.onEndEdit.RemoveAllListeners();
            ipInput.onEndEdit.AddListener(onEndEditIP);
        }

        public void OnClickHostTimeUp()
        {
            lobbyManager.CurFormat = GameFormat.TimeUp;
            lobbyManager.maxPlayersPerConnection = 1;
            lobbyManager.StartHost();
        }

        public void OnClickHostFirstDown()
        {
            lobbyManager.CurFormat = GameFormat.FirstDown;
            lobbyManager.maxPlayersPerConnection = 1;
            lobbyManager.StartHost();
        }

        public void OnClickHostTraining()
        {
            lobbyManager.CurFormat = GameFormat.Training;
            lobbyManager.maxPlayersPerConnection = 2;
            lobbyManager.StartHost();
        }

        public void OnClickJoin()
        {
            lobbyManager.ChangeTo(lobbyPanel);

            lobbyManager.networkAddress = ipInput.text;
            lobbyManager.StartClient();

            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager.DisplayIsConnecting();

            lobbyManager.SetServerInfo("连线中……", lobbyManager.networkAddress);
        }

        void onEndEditIP(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickJoin();
            }
        }
    }
}
