namespace nets_wpf.DataStructures
{
    public enum RunningMode
    {
        MainApplication,
        ContextMenuSmartSync,
        ContextMenuSyncWith
    }

    public enum Tab
    {
        PageProfiles,
        PageIVLE,
        PageSettings,
        PageAbout
    }

    public enum FileType
    {
        File,
        Folder
    }

    public enum FileStatus
    {
        Updated,
        New,
        NoChange,
        NotExist,
        Deleted
    }

    public enum SyncMode
    {
        Mirror,
        Equalize
    }
    
    public enum LogType
    {
        INFO,
        ERROR,
        DEBUG,
        FATAL
    }
}
