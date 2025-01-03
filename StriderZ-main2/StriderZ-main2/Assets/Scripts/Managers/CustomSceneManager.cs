using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes { TitleMenu, Lobby, GameMode }

public static class CustomSceneManager
{
    public static List<string> SceneNames = new List<string>() { "Main Menu", "Main Game"};
    public static bool IsFirstTimeInMenu = true;

    public static void ChangeScene(Scenes scene)
    {
        SceneManager.LoadSceneAsync(SceneNames[(int)scene]);
    }
    public static void ChangeScene(int num)
    {
        SceneManager.LoadSceneAsync(num);
    }
    public static void ReloadScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public static void ReturnToLobby()
    {
        EventManager.InvokeReturnToLobby();
        //SceneManager.LoadSceneAsync((int)Scenes.Lobby);
        SceneManager.LoadSceneAsync("LobbyToReturn");
    }
    public static int GetCurrentSceneID()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
