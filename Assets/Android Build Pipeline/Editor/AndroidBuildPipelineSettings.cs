﻿using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LuviKunG.BuildPipeline.Android
{
    public sealed class AndroidBuildPipelineSettings
    {
        private static AndroidBuildPipelineSettings instance;
        public static AndroidBuildPipelineSettings Instance
        {
            get
            {
                if (instance == null)
                    instance = new AndroidBuildPipelineSettings();
                return instance;
            }
        }

        private const string ALIAS = "unity.editor.luvikung.buildpipeline.android.";
        private const string PREFS_SETTINGS_BUILD_PATH = ALIAS + "buildpath";
        private const string PREFS_SETTINGS_NAME_FORMAT = ALIAS + "nameformat";
        private const string PREFS_SETTINGS_DATE_TIME_FORMAT = ALIAS + "datetimeformat";
        private const string PREFS_SETTINGS_INCREMENT_BUNDLE = ALIAS + "incrementbundle";
        private const string PREFS_SETTINGS_BUILD_OPTIONS = ALIAS + "buildOptions";
        private const string PREFS_SETTINGS_USE_KEYSTORE = ALIAS + "usekeystore";
        private const string PREFS_SETTINGS_IS_BUILD_APP_BUNDLE = ALIAS + "isBuildAppBundle";
        private const string PREFS_SETTINGS_IS_SPLIT_APP_BINARY = ALIAS + "isSplitAppBinary";
        private const string PREFS_SETTINGS_KEYSTORE_NAME = ALIAS + "keystorename";
        private const string PREFS_SETTINGS_KEYSTORE_PASS = ALIAS + "keystorepass";
        private const string PREFS_SETTINGS_KEYALIAS_NAME = ALIAS + "keyaliasname";
        private const string PREFS_SETTINGS_KEYALIAS_PASS = ALIAS + "keyaliaspass";

        public string buildPath;
        public string nameFormat;
        public string dateTimeFormat;
        public bool incrementBundle;
        public BuildOptions buildOptions;
        public bool useKeystore;
        public bool isBuildAppBundle;
        public bool isSplitAppBinary;

        public string keystoreName;
        public string keystorePass;
        public string keyaliasName;
        public string keyaliasPass;

        public AndroidBuildPipelineSettings()
        {
            Load();
        }

        public void Load()
        {
            // Define 'BUILD_PIPELINE_ANDROID_UNITY_DEFAULT' if you want to set a build location as same as default of Unity Editor
            // But it's buggy because it will include default file name too.
#if BUILD_PIPELINE_ANDROID_UNITY_DEFAULT
            buildPath = PlayerPrefs.GetString(PREFS_SETTINGS_BUILD_PATH, EditorUserBuildSettings.GetBuildLocation(BuildTarget.Android));
#else
            buildPath = PlayerPrefs.GetString(PREFS_SETTINGS_BUILD_PATH, string.Empty);
#endif
            nameFormat = PlayerPrefs.GetString(PREFS_SETTINGS_NAME_FORMAT, "{package}_{date}");
            dateTimeFormat = PlayerPrefs.GetString(PREFS_SETTINGS_DATE_TIME_FORMAT, "yyyyMMddHHmmss");
            incrementBundle = PlayerPrefs.GetString(PREFS_SETTINGS_INCREMENT_BUNDLE, bool.FalseString) == bool.TrueString;
            buildOptions = (BuildOptions)PlayerPrefs.GetInt(PREFS_SETTINGS_BUILD_OPTIONS, 0);
            
            useKeystore = PlayerPrefs.GetInt(PREFS_SETTINGS_USE_KEYSTORE, 0) != 0;
            isBuildAppBundle = PlayerPrefs.GetInt(PREFS_SETTINGS_IS_BUILD_APP_BUNDLE, EditorUserBuildSettings.buildAppBundle ? 1 : 0) != 0;
            isSplitAppBinary = PlayerPrefs.GetInt(PREFS_SETTINGS_IS_SPLIT_APP_BINARY, PlayerSettings.Android.useAPKExpansionFiles ? 1 : 0) != 0;

            keystoreName = PlayerPrefs.GetString(PREFS_SETTINGS_KEYSTORE_NAME, PlayerSettings.Android.keystoreName);

            keyaliasName = PlayerPrefs.GetString(PREFS_SETTINGS_KEYALIAS_NAME, string.Empty);
            
            var keystorePassEncoded = PlayerPrefs.GetString(PREFS_SETTINGS_KEYSTORE_PASS, string.Empty);
            if (!string.IsNullOrEmpty(keystorePassEncoded))
                keystorePass = Encoding.ASCII.GetString(Convert.FromBase64String(keystorePassEncoded));

            var keyaliasPassEncoded = PlayerPrefs.GetString(PREFS_SETTINGS_KEYALIAS_PASS, string.Empty);
            if (!string.IsNullOrEmpty(keyaliasPassEncoded))
                keyaliasPass = Encoding.ASCII.GetString(Convert.FromBase64String(keyaliasPassEncoded));
        }

        public void Save()
        {
            PlayerPrefs.SetString(PREFS_SETTINGS_BUILD_PATH, buildPath);
            PlayerPrefs.SetString(PREFS_SETTINGS_NAME_FORMAT, nameFormat);
            PlayerPrefs.SetString(PREFS_SETTINGS_DATE_TIME_FORMAT, dateTimeFormat);
            PlayerPrefs.SetString(PREFS_SETTINGS_INCREMENT_BUNDLE, incrementBundle ? bool.TrueString : bool.FalseString);
            PlayerPrefs.SetInt(PREFS_SETTINGS_BUILD_OPTIONS, (int)buildOptions);
            PlayerPrefs.SetInt(PREFS_SETTINGS_USE_KEYSTORE, useKeystore ? 1 : 0);
            PlayerPrefs.SetInt(PREFS_SETTINGS_IS_BUILD_APP_BUNDLE, isBuildAppBundle ? 1 : 0);
            PlayerPrefs.SetInt(PREFS_SETTINGS_IS_SPLIT_APP_BINARY, isSplitAppBinary ? 1 : 0);   

            if (!string.IsNullOrEmpty(keystoreName))
                PlayerPrefs.SetString(PREFS_SETTINGS_KEYSTORE_NAME, keystoreName);
            else if (PlayerPrefs.HasKey(PREFS_SETTINGS_KEYSTORE_NAME))
                PlayerPrefs.DeleteKey(PREFS_SETTINGS_KEYSTORE_NAME);

            if (!string.IsNullOrEmpty(keyaliasName))
                PlayerPrefs.SetString(PREFS_SETTINGS_KEYALIAS_NAME, keyaliasName);
            else if (PlayerPrefs.HasKey(PREFS_SETTINGS_KEYALIAS_NAME))
                PlayerPrefs.DeleteKey(PREFS_SETTINGS_KEYALIAS_NAME);

            if (!string.IsNullOrEmpty(keystorePass))
            {
                var keystorePassEncoded = Convert.ToBase64String(Encoding.ASCII.GetBytes(keystorePass));
                PlayerPrefs.SetString(PREFS_SETTINGS_KEYSTORE_PASS, keystorePassEncoded);
            }
            else if (PlayerPrefs.HasKey(PREFS_SETTINGS_KEYSTORE_PASS))
                PlayerPrefs.DeleteKey(PREFS_SETTINGS_KEYSTORE_PASS);

            if (!string.IsNullOrEmpty(keyaliasPass))
            {
                var keyaliasPassEncoded = Convert.ToBase64String(Encoding.ASCII.GetBytes(keyaliasPass));
                PlayerPrefs.SetString(PREFS_SETTINGS_KEYALIAS_PASS, keyaliasPassEncoded);
            }
            else if (PlayerPrefs.HasKey(PREFS_SETTINGS_KEYALIAS_PASS))
                PlayerPrefs.DeleteKey(PREFS_SETTINGS_KEYALIAS_PASS);
        }

        public string GetFileName()
        {
            StringBuilder s = new StringBuilder(nameFormat);
            s.Replace("{name}", Application.productName);
            s.Replace("{package}", PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android));
            s.Replace("{version}", Application.version);
            s.Replace("{bundle}", PlayerSettings.Android.bundleVersionCode.ToString());
            s.Replace("{date}", DateTime.Now.ToString(dateTimeFormat));
            s.Append(isBuildAppBundle ? ".aab" : ".apk");
            return s.ToString();
        }
    }
}