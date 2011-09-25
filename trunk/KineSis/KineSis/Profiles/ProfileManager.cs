using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KineSis.Profiles {

    class ProfileManager {
        private static List<Profile> profiles;

        private static Profile activeProfile;

        private static String path = "Profiles.kinesis";

        public static List<Profile> Profiles {
            get {

                if (profiles == null) {

                    Deserialize();
                    if (profiles == null) {

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
                        profiles.Add(kinesis);
                        ActiveProfile = kinesis;
                    }
                }
                return profiles;
            }
        }

        public static void Serialize() {
            SerializableProfiles sProfiles = new SerializableProfiles(profiles, activeProfile);
            SerializableProfiles.Serialize(sProfiles, path);
            profiles = null;
            activeProfile = null;
        }

        public static void Deserialize() {
            try {
                SerializableProfiles sProfiles = SerializableProfiles.Deserialize(path);
                if (sProfiles != null) {
                    profiles = sProfiles.profiles;
                    ActiveProfile = sProfiles.activeProfile;
                }
            } catch (Exception) {
            }
        }

        public static void AddProfile(Profile profile) {
            for (int i = 0; i < Profiles.Count; i++) {
                if (Profiles[i].Name.Equals(profile.Name)) {
                    throw new Exception("Profile '" + profile.Name + "' already exists");
                }
            }
            profiles.Add(profile);
            //Serialize();
        }

        public static void SaveProfile(Profile profile) {
            if (profile.Name.Equals("KineSis")) {
                throw new Exception("You are not allowed to change the default '" + profile.Name + "' profile. Make your own one, with another name!");
            } else {
                for (int i = 0; i < Profiles.Count; i++) {
                    if (Profiles[i].Name.Equals(profile.Name)) {
                        Profiles[i] = profile;
                    }
                }
            }
            //Serialize();
        }

        public static void AddDocumentToActive(String documentName, String documentLocation) {
            Doc doc = new Doc();
            doc.Name = documentName;
            doc.Location = documentLocation;
            ActiveProfile.Documents.Add(doc);
        }

        public static Profile GetProfile(String name) {
            Profile profile = null;
            for (int i = 0; i < Profiles.Count; i++) {
                if (Profiles[i].Name.Equals(name)) {
                    profile =  Profiles[i];
                }
            }
            return profile;
        }

        public static Profile ActiveProfile {
            get {
                if (activeProfile == null) {
                    Deserialize();
                    if (activeProfile == null) {
                        activeProfile = Profiles[0];
                    }
                }
                return activeProfile;
            }

            set {
                for (int i = 0; i < Profiles.Count; i++) {
                    if (Profiles[i].Name.Equals(value.Name)) {
                        activeProfile = Profiles[i];
                    }
                }
            }
        }
    }

    [Serializable]
    public class SerializableProfiles {

        public List<Profile> profiles;
        public Profile activeProfile;

        public SerializableProfiles(List<Profile> profiles, Profile activeProfile) {
            this.profiles = profiles;
            this.activeProfile = activeProfile;
        }

        public static void Serialize(SerializableProfiles sProfiles, String path) {
            FileInfo fi = new FileInfo(path);
            if (fi.Exists) {
                fi.Delete();
            }
            Stream a = File.OpenWrite(path);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(a, sProfiles);
            a.Close();
        }

        public static SerializableProfiles Deserialize(String path) {
            //try {
                FileStream file = new FileStream(path, FileMode.Open);

                BinaryFormatter bf = new BinaryFormatter();
                SerializableProfiles sProfiles = bf.Deserialize(file) as SerializableProfiles;
                file.Close();
                
            //} catch (Exception) {
            //    return null;
            //}
                return sProfiles;
        }
    }
}
