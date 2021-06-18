using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

namespace TCM.Tools
{
    public class Web : MonoBehaviour
    {
        //> CONVERT AN ID:DATA DICTIONARY TO A WWW FORM
        public static WWWForm CreateForm(Dictionary<string, string> dataContainer)
        {
            WWWForm response = new WWWForm();

            foreach (var entry in dataContainer)
            {
                string entryID = entry.Key;
                string data = entry.Value;
                response.AddField(entryID, data);
            }

            return response;
        }
        
        //> ATTEMPT TO SUBMIT RESPONSE TO FORM
        public void SubmitResponse(string formURL, WWWForm response) => StartCoroutine(SubmitResponseCoroutine(formURL, response));

        //> COROUTINE FOR ASYNCHRONOUS BEHAVIOUR
        private static IEnumerator SubmitResponseCoroutine(string formURL, WWWForm response)
        {
            using UnityWebRequest www = UnityWebRequest.Post(formURL, response);
            yield return www.SendWebRequest();
            Debug.Log("Response Sent!");
        }

        //> OPEN A LINK
        public static void OpenLink(string URL)
        {
            bool isGoogleSearch = URL.Contains("google.com/search");
            string properURL = URL.Replace(" ", (isGoogleSearch) ? "+" : "%20");
            Application.OpenURL(properURL);
        }

    }
}