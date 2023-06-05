using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Threading;


//[Serializable]
public struct AdResolution
{
    public int width;
    public int height;

    public AdResolution(int w, int h)
    {
        width = w; 
        height = h;
    }
}

[Serializable]
public struct ImageResPair
{
    public Image image;
    public AdResolutions res;
}

[System.Serializable]
public struct FolderNames
{
    public string[] foldernames;

    public FolderNames(string[] foldernames)
    {
        this.foldernames = foldernames;
    }
}


public enum AdResolutions
{
    // Standard mobile banner size
    Banner_320x50 = 0,
    // These you will already have if you release on steam
    Landscape_231x87 = 1,
    Landscape_460x215 = 2,
    Landscape_616x253 = 3,
    Portrait_374x448 = 4,
    Portrait_600x900 = 5,
}
public class a1_IndieAds : MonoBehaviour
{
    private AdResolution[] resolutions = new AdResolution[]
    {
        new AdResolution (320, 50),
        new AdResolution (231, 87),
        new AdResolution (460, 215),
        new AdResolution (616, 353),
        new AdResolution (374, 448),
        new AdResolution (600, 900),
    };


    private static List<ImageResPair> ads = new List<ImageResPair>();
    [Header("If you want the ads to refresh. <=0 means disabled")]
    [SerializeField] private float secondsBetweenAdRefresh = 0f;
    [Header("Write your game name to avoid it displaying ads for itself")]
    [SerializeField] private string myGame = "gametitle";

    private Coroutine refreshRoutine;

    public static void SubmitIndieAd(ImageResPair ad)
    {
        ads.Add(ad);
    }

    private void OnDisable()
    {
        StopCoroutine(refreshRoutine);
    }

    private void Start()
    {
        FetchAllAds();

        if (secondsBetweenAdRefresh > 0)
        {
            refreshRoutine = StartCoroutine(RefreshAdsRoutine());
        }
    }

    private IEnumerator RefreshAdsRoutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(secondsBetweenAdRefresh);
            FetchAllAds();
        }
    }

    private void FetchAllAds()
    {
        for (int i = 0; i < ads.Count; i++)
        {
            // need to use thread because coroutine does not support async await
            var ad = ads[i];
            AdFetch(ad);
        }
    }

    private async void AdFetch(ImageResPair ad)
    {
        string folder = await GetAdFolderAsync(myGame);
        //print(folder);
        StartCoroutine(SetSpriteTexture(ad.image, resolutions[(int)ad.res], folder));
        //print("Fetched ad and closed thread");
    }

    private async Task<string> GetAdFolderAsync(string mygame)
    {
        HttpClient httpClient = new HttpClient();
        HttpResponseMessage response = await httpClient.GetAsync("https://indieads.github.io//foldernames.json");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();

        var folders = JsonUtility.FromJson<FolderNames>(responseBody).foldernames;

        // UnityEngine.Random.Range() can only be used on the main thread, so we use System.Eandom()
        var rnd = new System.Random();
        var rndindex = rnd.Next(0, folders.Length);


        while (folders[rndindex] == mygame)
        {
            //print("foldername is not valid");
            rndindex = rnd.Next(0, folders.Length);
        }
        
            

        var selectedFolder = folders[rndindex];

        return selectedFolder;
    }

    // GetTexture() can only be called on main thread, so this has to be a coroutine
    private IEnumerator SetSpriteTexture(Image image, AdResolution res, string folder)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture($"https://indieads.github.io//ads/{folder}/{res.width}x{res.height}.png");
        //print(www.uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            print(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            image.sprite = Sprite.Create((Texture2D)myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        }
    }
}



