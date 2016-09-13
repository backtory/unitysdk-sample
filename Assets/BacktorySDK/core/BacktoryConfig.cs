using UnityEngine;

namespace Assets.BacktorySDK.core
{
    public class BacktoryConfig : ScriptableObject
    {
        public const string gamesparksSettingsAssetName = "BacktorySettings";
        public const string gamesparksSettingsPath = "Backtory/Resources";
        public const string gamesparksSettingsAssetExtension = ".asset";
        //internal readonly IStorage Storage;
        [SerializeField]
        private string _backtoryAuthInstanceId;
        [SerializeField]
        private string _backtoryAuthClientKey;
        [SerializeField]
        private string _backtoryCloudcodeInstanceId;
        [SerializeField]
        private string _backtoryGameInstanceId;
        [SerializeField]
        private string _backtoryConnectivityInstanceId;

        private static BacktoryConfig instance;
        public static void SetInstance(BacktoryConfig settings)
        {
            instance = settings;
        }

        public static BacktoryConfig Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = Resources.Load(gamesparksSettingsAssetName) as BacktoryConfig;
                    if (ReferenceEquals(instance, null))
                    {
                        // If not found, autocreate the asset object.
                        instance = CreateInstance<BacktoryConfig>();
                    }
                }
                return instance;
            }
        }


        public static string BacktoryAuthInstanceId
        {
            get { return Instance._backtoryAuthInstanceId; }
            set { Instance._backtoryAuthInstanceId = value; }
        }

        public static string BacktoryAuthClientKey
        {
            get { return Instance._backtoryAuthClientKey; }
            set { Instance._backtoryAuthClientKey = value; }
        }

        public static string BacktoryCloudcodeInstanceId
        {
            get { return Instance._backtoryCloudcodeInstanceId; }
            set { Instance._backtoryCloudcodeInstanceId = value; }
        }

        public static string BacktoryGameInstanceId
        {
            get { return Instance._backtoryGameInstanceId; }
            set { Instance._backtoryGameInstanceId = value; }
        }

        public static string BacktoryConnectivityInstanceId
        {
            get { return Instance._backtoryConnectivityInstanceId; }
            set { Instance._backtoryConnectivityInstanceId = value; }
        }

        /*
        public interface StorageStep
        {
            AuthStep storage(IStorage storage);
        }

        public interface AuthStep
        {
            ModuleStep initAuth(string authInstanceId, string authClientKey);
        }

        public interface ModuleStep
        {
            ModuleStep initGame(string gameInstanceId);

            ModuleStep initCloudCode(string lambdaInstanceId);

            Config build();
        }

        public static StorageStep NewBuilder()
        {
            return new BacktoryConfigBuilder();
        }

        public class BacktoryConfigBuilder : StorageStep, AuthStep, ModuleStep
        {
            internal BacktoryConfigBuilder()
            {
            }

            private IStorage Storage;

            private string BacktoryAuthInstanceId;
            private string BacktoryAuthClientKey;
            private string BacktoryCloudcodeInstanceId;
            private string BacktoryGameInstanceId;

            public ModuleStep initAuth(string authInstanceId, string authClientKey)
            {
                BacktoryAuthInstanceId = authInstanceId;
                BacktoryAuthClientKey = authClientKey;
                return this;
            }

            public ModuleStep initGame(string gameInstanceId)
            {
                BacktoryGameInstanceId = gameInstanceId;
                return this;
            }

            public ModuleStep initCloudCode(string lambdaInstanceId)
            {
                BacktoryGameInstanceId = lambdaInstanceId;
                return this;
            }

            public Config build()
            {
                return new Config(Storage, BacktoryAuthInstanceId, BacktoryAuthClientKey,
                    BacktoryCloudcodeInstanceId, BacktoryGameInstanceId);
            }

            public AuthStep storage(IStorage storage)
            {
                Storage = storage;
                return this;
            }
        }
        */
    }
}