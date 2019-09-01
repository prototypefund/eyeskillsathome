using UnityEngine;
using NUnit.Framework;
using System;

namespace EyeSkills
{
   
    public class TestCustomInfoBase : InfoBase
    {
        public string myAdditionalField;

    }

    public class TestMyInfoBase
    {

        [Test]
        public void TestInfoBaseGetsBasicContext()
        {
            TestCustomInfoBase data = new TestCustomInfoBase();
            //Write a function to setup the InfoBase

            Assert.IsNotNull(data.GetUUID());

            //You may need to run a build first to generate the Version class
            Assert.IsNotNull(data.GetBuildID());
            Assert.AreEqual(data.GetClassname(), "EyeSkills.TestCustomInfoBase");
            Assert.IsNotNull(data.GetTimestamp());

            //Now set a fixed timestamp so we can have repeatable JSON output
            //UUID[user[buildId[time -> obj]]]

            data.myAdditionalField = "needtosetthis";

            string json = data.GetAsJSON();
            //Assert.AreEqual(json, "{\"myAdditionalField\":\"needtosetthis\",\"userID\":\"unknown\",\"uuid\":\"5a0377e2-9f6a-4553-b989-f541b6d04199\",\"buildID\":\"b1c9fba\",\"timestamp\":{}}");
            Assert.IsNotEmpty(json);

            //Store data in the custom fields of TestCustomInfoBase

            data.Save();

            //Now check that the data is stored in PlayerPrefs as we expect 
            String location = "EyeSkillsInfoBase_" + data.GetTimestamp();
            String playerPrefsData = PlayerPrefs.GetString(location);
            Assert.AreEqual(playerPrefsData, json);
        }


    }
}