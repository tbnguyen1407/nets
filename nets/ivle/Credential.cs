namespace nets.ivle
{
    /// <summary>
    /// Author: Vu An Hoa
    /// </summary>
    class Credential
    {
        private const int UserNameFieldPosition = 6;

        public static void SetCredential(string username, string password)
        {
            FormValues[UserNameFieldPosition] = username;
            FormValues[UserNameFieldPosition + 1] = password;
        }

        // add the keys and values
        public static string[] FormFields = {
                                                "__LASTFOCUS",
                                                "__EVENTTARGET",
                                                "__EVENTARGUMENT",
                                                "__VIEWSTATE",
                                                "__SCROLLPOSITIONX",
                                                "__SCROLLPOSITIONY",
                                                "ctl00$userid",
                                                "ctl00$password",
                                                "ctl00$domain",
                                                "ctl00$loginimg1.x",
                                                "ctl00$loginimg1.y"
                                            };

        public static string[] FormValues = {
                                                "",
                                                "",
                                                "",
                                                "/wEPDwULLTE0Njg4NTU2MDhkGAEFHl9fQ29udHJvbHNSZXF1aXJlUG9zdEJhY2tLZXlfXxYCBRNjdGwwMCRjaGtSZW1lbWJlck1lBQ9jdGwwMCRsb2dpbmltZzE=",
                                                "0",
                                                "191",
                                                "",
                                                "",
                                                "NUSSTU",
                                                "30",
                                                "11"
                                            };
    }
}
