/**************************************************************************
* Copyright (C) echoAR, Inc. 2018-2020.                                   *
* echoAR, Inc. proprietary and confidential.                              *
*                                                                         *
* Use subject to the terms of the Terms of Service available at           *
* https://www.echoar.xyz/terms, or another agreement                      *
* between echoAR, Inc. and you, your company or other organization.       *
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARFaceManager))]
public class CustomBehaviour : MonoBehaviour
{
    [HideInInspector]
    public Entry entry;
    private GameObject _arFace;
    private Material face_material;

    /// <summary>
    /// EXAMPLE BEHAVIOUR
    /// Queries the database and names the object based on the result.
    /// </summary>

    // Use this for initialization
    void Start()
    {
        // Add RemoteTransformations script to object and set its entry
        this.gameObject.AddComponent<RemoteTransformations>().entry = entry;

        // Qurey additional data to get the name
        string value = "";
        if (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("name", out value))
        {
            // Set name
            this.gameObject.name = value;
        }
        _arFace = GameObject.Find("AR Default Face");

        // file storage id could be found in https://console.echoar.xyz/query
        string file_storage_id = "a91f3f38-492f-4a3b-b83a-e51be33563e2";
        string url = "https://console.echoAR.xyz/query?key=" + GameObject.Find("echoAR").GetComponent<echoAR>().APIKey + "&file="
            + file_storage_id;

        // downloading face mask
        StartCoroutine(DownloadImage(url));

    }
    IEnumerator DownloadImage(string MediaUrl)
    {
        face_material = new Material(Shader.Find("Standard"));
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            face_material.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            _arFace.GetComponent<MeshRenderer>().material = face_material;
        }

    }
}