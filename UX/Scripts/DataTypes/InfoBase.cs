using UnityEngine;
using System;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

namespace EyeSkills
{
    public class InfoBase
    {
        //Only public variables are serialised by Newtonsoft.Json!
        public string userID = "unknown", uuid, buildID, timestamp, buildVersion, timeZone;

        public InfoBase()
        {
            //Make sure our values are set
            timestamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            timeZone = TimeZone.CurrentTimeZone.StandardName;
            uuid = GetUUID();
            buildID = GetBuildID();
            userID = GetUserID();
            buildVersion = new Version().version;
        }

        public string GetClassname()
        {
            return GetType().FullName;
        }

        public string GetUUID()
        {
            uuid = PlayerPrefs.GetString("EyeSkills.UUID", "");
            if ((null == uuid) || (uuid == ""))
            {
                uuid = System.Guid.NewGuid().ToString();
                PlayerPrefs.SetString("EyeSkills.UUID", uuid);
            }

            return uuid;
        }

        private string FullSceneAndClassID()
        {
            return this.GetType().FullName + "," + SceneManager.GetActiveScene().name;
        }

        public string GetUserID()
        {
            return userID;
        }

        public string GetTimestamp()
        {
            return timestamp;
        }

        public string GetBuildID()
        {

            if (buildID == null)
            {
                //Type t;

                try
                {
                    //Look for custom physics... 
                    //string classname = "Version";
                    //t = Type.GetType(classname, true);
                    //Version v = (Version)Activator.CreateInstance(t);
                    Version v = new Version();
                    buildID = v.version;

                }
                catch (TypeLoadException exception)
                {
                    Debug.Log("Could not load Version " + exception.ToString());
                    return null;
                }
            }

            return buildID;
        }

        public string GetAsJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string CurrentDataKey(){
            return "EyeSkills.InfoBase," + GetUUID() + "," + FullSceneAndClassID();
        }

        public void Save()
        {
            var json = GetAsJSON();
            Debug.Log("Saving InfoBase: " + json);

            //Now we need to store it chronologically in the PlayPrefs.
            //We will be able to iterate over the entries using EyeSkillsInfoBase_ prefix

            //For historical logging
            //TODO: Better to send this to a server actually!
            //PlayerPrefs.SetString(CurrentDataKey() + "_Historical_" + timestamp, json);

            //As last used data
            PlayerPrefs.SetString(CurrentDataKey(), json);

        }

        public void Load()
        {
            string json = PlayerPrefs.GetString(CurrentDataKey());
            if (json != "")
                JsonConvert.PopulateObject(json, this);
        }
    }
}
