using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Events;


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

[System.Serializable]
public struct FolderNames
{
    public string[] foldernames;

    public FolderNames(string[] foldernames)
    {
        this.foldernames = foldernames;
    }
}

public interface IIndieAd
{
    public void SetTexture(Texture2D texture);
    public void SetURL(string url);
    public AdResolutions Resolution { get; set; }
    public bool FetchLink { get; set; }
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


    private static List<IIndieAd> ads = new List<IIndieAd>();
    [Header("If you want the ads to refresh. <=0 means disabled")]
    [SerializeField] private float secondsBetweenAdRefresh = 0f;
    [Header("Write your game name to avoid it displaying ads for itself\nAll lowercase, no spaces. Example: \"spacevoyage\"")]
    [SerializeField] private string myGame = "gametitle";

    private Coroutine refreshRoutine;

    public static void SubmitIndieAd(IIndieAd ad)
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
            AdFetch(ads[i]);
        }
    }

    private async void AdFetch(IIndieAd ad)
    {
        string folder = await GetAdFolderAsync(myGame);
        //print(folder);

        StartCoroutine(SetSpriteTexture((tx) => ad.SetTexture(tx), resolutions[(int)ad.Resolution], folder));
        //print("Fetched ad and closed thread");

        if (ad.FetchLink)
            StartCoroutine(GetHref($"https://indieads.github.io/stnemesitrevda/{folder}/href", (x) => ad.SetURL(x)));
    }

    private async Task<string> GetAdFolderAsync(string mygame)
    {
        HttpClient httpClient = new HttpClient();
        HttpResponseMessage response = await httpClient.GetAsync("https://indieads.github.io/foldernames.json");
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
    private IEnumerator SetSpriteTexture(UnityAction<Texture2D> setTexture, AdResolution res, string folder)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture($"https://indieads.github.io/stnemesitrevda/{folder}/{res.width}x{res.height}.png");


        //print(www.uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            print(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            setTexture((Texture2D)myTexture);
        }
    }


    private IEnumerator GetHref(string fileURL, UnityAction<string> executeOnSuccess)
    {
        UnityWebRequest req = UnityWebRequest.Get(fileURL);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            print(req.error);
        }
        else
        {
            var adURL = req.downloadHandler.text;
            executeOnSuccess(adURL);
        }

    }

}



