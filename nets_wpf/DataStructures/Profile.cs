using System.Text;

namespace nets_wpf.DataStructures
{
    public class Profile
    {
        public string ProfileName { private set; get; }
        public string SrcFolder { set; get; }
        public string DesFolder { private set; get; }
        public SyncMode SyncMode { private set; get; }
        //public FilterPattern FilterPattern { get; private set; }
        public string IncludePattern;
        public string ExcludePattern;

        public Profile()
            : this("defaultProfile", "", "", SyncMode.Equalize, "", "")
        {
        }

        /*
        public Profile()
        {
            ProfileName = "defaultProfile";
            SrcFolder = "";
            DesFolder = "";
            SyncMode = SyncMode.Equalize;
            //FilterPattern = new FilterPattern("", "");
            IncludePattern = "";
            ExcludePattern = "";
        }
        */

        /*
        public Profile(string profileName, string srcFolder, string desFolder, SyncMode syncMode, FilterPattern filterPattern)
        {
            ProfileName = profileName;
            SyncMode = syncMode;
            SrcFolder = srcFolder;
            DesFolder = desFolder;
            //FilterPattern = filterPattern;
            IncludePattern = "";
            ExcludePattern = "";
        }
        */

        /*
        public Profile(string profileName, string srcFolder, string desFolder, SyncMode syncMode, string includePattern, string excludePattern)
            : this(profileName, srcFolder, desFolder, syncMode, new FilterPattern(includePattern, excludePattern))
        {
        }
        */

        public Profile(string profileName, string srcFolder, string desFolder, SyncMode syncMode, string includePattern, string excludePattern)
        {
            ProfileName = profileName;
            SyncMode = syncMode;
            SrcFolder = srcFolder;
            DesFolder = desFolder;
            IncludePattern = includePattern;
            ExcludePattern = excludePattern;
        }

        public bool Equals(Profile p)
        {
            if (p == null)
                return false;
            
            return this.ToString() == p.ToString();
        }

        /*
        public override string ToString()
        {
            StringBuilder profileData = new StringBuilder();
            profileData.Append(ProfileName + "|"
                               + SrcFolder + "|"
                               + DesFolder + "|"
                               + SyncMode + "|"
                               + ((FilterPattern == null) ? "|" : FilterPattern.ToString())
                );
            return profileData.ToString();
        }*/

        public override string ToString()
        {
            StringBuilder profileData = new StringBuilder();
            profileData.Append(ProfileName + "|"
                               + SrcFolder + "|"
                               + DesFolder + "|"
                               + SyncMode + "|"
                               + IncludePattern + "|"
                               + ExcludePattern);
            return profileData.ToString();
        }
    }
}