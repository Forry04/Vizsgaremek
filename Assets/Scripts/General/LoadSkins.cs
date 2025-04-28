using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;

public class LoadSkins : MonoBehaviour
{
    List<SkinData> skins = new List<SkinData>();
    public async void Loadskins()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Add the Authorization header with the Bearer token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", PlayerPrefs.GetString("token"));

                // Send the GET request
                HttpResponseMessage response = await client.GetAsync($"{PlayerDataManager.BaseApiUrl}/user/skins");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log("User Skins: " + responseBody);

                    // Deserialize the JSON response into a list of SkinData objects
                    skins = JsonConvert.DeserializeObject<List<SkinData>>(responseBody);
                    
                    SetSkins();

                }
                else
                {
                    Debug.LogError("Failed to fetch user skins: " + response.StatusCode);
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    Debug.LogError("Error details: " + errorResponse);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error occurred: " + ex.Message);
            }
        }
    }

    private void SetSkins()
    {
        var skinList = Resources.LoadAll<Skindata>("Skins").ToList();
        foreach (var skin in skins)
        {
            var skinFound = skinList.FirstOrDefault(s=>s.skinName.Equals(skin.SkinName,StringComparison.OrdinalIgnoreCase));
            if (skinFound != null)
            {
                skinFound.isUnlocked = skin.IsOwned;
            }
        }


    }
}



// Define a class to represent the skin data
[Serializable]
public class SkinData
{
    [JsonProperty("skin_id")]
    public int SkinId { get; set; }

    [JsonProperty("skin_name")]
    public string SkinName { get; set; }

    [JsonProperty("rarity_name")]
    public string RarityName { get; set; }

    [JsonProperty("rarity_price")]
    public int RarityPrice { get; set; }

    [JsonProperty("skin_image_url")]
    public string SkinImageUrl { get; set; }

    [JsonProperty("is_owned")]
    public bool IsOwned { get; set; }
}
