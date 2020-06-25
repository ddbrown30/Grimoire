using Newtonsoft.Json;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomScriptReader : MonoBehaviour
{
    public RoleSelectionManager RoleSelectionManager;

    public void OpenCustomScript()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "json", false);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }

    private IEnumerator OutputRoutine(string url)
    {
        var loader = new UnityWebRequest(url);
        loader.downloadHandler = new DownloadHandlerBuffer();
        yield return loader.SendWebRequest();

        ParseCustomScript(loader.downloadHandler.text);
    }

    [System.Serializable]
    class IdWrapper
    {
        public string id { get; set; }
    }

    void ParseCustomScript(string json)
    {
        RoleSelectionManager.CustomScriptData.Clear();

        List<string> missingIds = new List<string>();
        var idList =  JsonConvert.DeserializeObject<List<IdWrapper>>(json);
        foreach (var id in idList)
        {
            RoleData roleData = Array.Find(RoleSelectionManager.RoleDataList, x => x.ScriptToolId == id.id);
            if(roleData == null)
            {
                missingIds.Add(id.id);
                continue;
            }

            RoleSelectionManager.CustomScriptData.Add(roleData);
        }

        RoleSelectionManager.OnCustomScriptLoaded();

        if (missingIds.Count > 0)
        {
            string errorMessage = "Failed to parse the following roles from the .json file:\n";
            errorMessage += string.Join(", ", missingIds);
            ModalManager.Instance().MessageBox(errorMessage, null, null, null, null, "Ok");
        }
    }
}
