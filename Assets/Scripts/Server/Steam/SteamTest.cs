using Steamworks;
using UnityEngine;

public class SteamTest : MonoBehaviour
{
    void Start()
    {
        if (!SteamAPI.Init())
        {
            Debug.LogError("Steam Init 실패");
            return;
        }

        Debug.Log("Steam Init 성공");
        Debug.Log($"SteamID: {SteamUser.GetSteamID()}");
        Debug.Log($"SteamName: {SteamFriends.GetPersonaName()}");
    }

    void Update()
    {
        SteamAPI.RunCallbacks();
    }

    void OnDestroy()
    {
        SteamAPI.Shutdown();
    }
}