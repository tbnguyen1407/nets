namespace nets.dataclass
{
    /// <summary>
    /// Author: Hoang Nguyen Nhat Tao + Nguyen Hoang Hai + Nguyen Thi Yen Duong + Tran Binh Nguyen
    /// </summary>
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
        OneWay,
        TwoWay
    }
    
    public enum LogType
    {
        INFO,
        ERROR,
        DEBUG,
        FATAL
    }

    public enum SyncStatus
    {
        READY,
        INITIALIZE,
        DETECT,
        RECONCILE,
        FINALIZE
    }
}
