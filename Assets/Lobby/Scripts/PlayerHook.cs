using UnityEngine;
using System.Collections;

namespace Prototype.NetworkLobby
{
    public class PlayerHook : LobbyHook
    {
        public override void OnLobbyServerSceneLoadedForPlayer(UnityEngine.Networking.NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
        {
            var lp = lobbyPlayer.GetComponent<LobbyPlayer>();
            var gp = gamePlayer.GetComponent<Yorisiro>();

            gp.CharacterName = CharacterManager.Avatars[lp.characterNo].avatarName;
            gp.PlayerIndex = lp.playerIndex;
            gp.IsCpuPlayer = lp.IsCpuPlayer;
        }
    }
}