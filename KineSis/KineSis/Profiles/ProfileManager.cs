/*
   Copyright 2011 Alexandru Albu - http://code.google.com/p/kinesis/

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KineSis.Profiles
{
    /// <summary>
    /// Profile Manager is responsable with KineSis profiles. Always keeps an ActiveProfile, handle the adding and saving profiles
    /// </summary>
    class ProfileManager
    {
        private static List<Profile> profiles;
        private static Profile activeProfile;
        private static String path = "Profiles.kinesis";
        private static Boolean minimalView = false;

        /// <summary>
        /// Minimal View is for running KineSis on only one screen
        /// </summary>
        public static Boolean MinimalView
        {
            get
            {
                return minimalView;
            }

            set
            {
                minimalView = value;
            }
        }

        /// <summary>
        /// All profiles
        /// </summary>
        public static List<Profile> Profiles
        {
            get
            {
                if (profiles == null)
                {
                    Deserialize();
                    if (profiles == null)
                    {

                        profiles = new List<Profile>();
                        Profile kinesis = new Profile();
                        kinesis.Name = "KineSis";
                        kinesis.PresentationScreen = 1;
                        kinesis.UserScreen = 0;
                        kinesis.PrimaryColor = new SolidColorBrush(Color.FromRgb(86, 58, 150));
                        kinesis.SecondaryColor = new SolidColorBrush(Color.FromRgb(138, 194, 49));
                        kinesis.BackgroundColor = Brushes.White;
                        kinesis.SkeletonColor = Brushes.Black;
                        kinesis.TempFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString() + "\\KineSis";
                        kinesis.SlideWidth = 1280;
                        kinesis.ChartWidth = 800;
                        kinesis.ChartHorizontalFaces = 8;
                        kinesis.ChartVerticalFaces = 2;
                        kinesis.TouchDistance = 0.45;
                        kinesis.UntouchDistance = 0.3;
                        profiles.Add(kinesis);
                        ActiveProfile = kinesis;

                    }
                }
                return profiles;
            }
        }

        /// <summary>
        /// serialize all profiles
        /// </summary>
        public static void Serialize()
        {
            SerializableProfiles sProfiles = new SerializableProfiles(profiles, activeProfile);
            SerializableProfiles.Serialize(sProfiles, path);
            profiles = null;
            activeProfile = null;
        }

        /// <summary>
        /// deserialize all profiles
        /// </summary>
        public static void Deserialize()
        {
            try
            {
                SerializableProfiles sProfiles = SerializableProfiles.Deserialize(path);
                if (sProfiles != null)
                {
                    profiles = sProfiles.profiles;
                    ActiveProfile = sProfiles.activeProfile;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// add a new profile
        /// </summary>
        /// <param name="profile"></param>
        public static void AddProfile(Profile profile)
        {
            for (int i = 0; i < Profiles.Count; i++)
            {
                if (Profiles[i].Name.Equals(profile.Name))
                {
                    throw new Exception("Profile '" + profile.Name + "' already exists");
                }
            }
            profiles.Add(profile);
        }

        /// <summary>
        /// save/update a profile
        /// </summary>
        /// <param name="profile"></param>
        public static void SaveProfile(Profile profile)
        {
            if (profile.Name.Equals("KineSis"))
            {
                throw new Exception("You are not allowed to change the default '" + profile.Name + "' profile. Make your own one, with another name!");
            }
            else
            {
                for (int i = 0; i < Profiles.Count; i++)
                {
                    if (Profiles[i].Name.Equals(profile.Name))
                    {
                        Profiles[i] = profile;
                    }
                }
            }
        }

        /// <summary>
        /// add a document to active profile. because documents are opened by different users, each user (profile) has his own set of documents
        /// </summary>
        /// <param name="documentName"></param>
        /// <param name="documentLocation"></param>
        public static void AddDocumentToActive(String documentName, String documentLocation)
        {
            Doc doc = new Doc();
            doc.Name = documentName;
            doc.Location = documentLocation;
            ActiveProfile.Documents.Add(doc);
        }

        /// <summary>
        /// get a profile by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Profile GetProfile(String name)
        {
            Profile profile = null;
            for (int i = 0; i < Profiles.Count; i++)
            {
                if (Profiles[i].Name.Equals(name))
                {
                    profile = Profiles[i];
                }
            }
            return profile;
        }

        /// <summary>
        /// active profile
        /// </summary>
        public static Profile ActiveProfile
        {
            get
            {
                if (activeProfile == null)
                {
                    Deserialize();
                    if (activeProfile == null)
                    {
                        activeProfile = Profiles[0];
                    }
                }
                return activeProfile;
            }

            set
            {
                for (int i = 0; i < Profiles.Count; i++)
                {
                    if (Profiles[i].Name.Equals(value.Name))
                    {
                        activeProfile = Profiles[i];
                    }
                }
            }
        }
    }

    /// <summary>
    /// representation of a list of profiles which can be serialized
    /// </summary>
    [Serializable]
    public class SerializableProfiles
    {

        public List<Profile> profiles;
        public Profile activeProfile;

        public SerializableProfiles(List<Profile> profiles, Profile activeProfile)
        {
            this.profiles = profiles;
            this.activeProfile = activeProfile;
        }

        /// <summary>
        /// serialize
        /// </summary>
        /// <param name="sProfiles"></param>
        /// <param name="path"></param>
        public static void Serialize(SerializableProfiles sProfiles, String path)
        {
            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                fi.Delete();
            }
            Stream a = File.OpenWrite(path);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(a, sProfiles);
            a.Close();
        }

        /// <summary>
        /// deserialize
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SerializableProfiles Deserialize(String path)
        {
            FileStream file = new FileStream(path, FileMode.Open);

            BinaryFormatter bf = new BinaryFormatter();
            SerializableProfiles sProfiles = bf.Deserialize(file) as SerializableProfiles;
            file.Close();
            return sProfiles;
        }
    }
}
