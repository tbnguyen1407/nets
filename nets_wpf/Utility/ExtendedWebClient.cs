using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.IO;

namespace nets_wpf.Utility
{
    /// <summary>
    /// Extended (but different) version of WebClient object.
    /// More convenient to use POST method.
    /// Support a download manager to manage downloading of files.
    /// </summary>
    public class ExtendedWebClient
    {
        #region WEBCLIENT OBJECTS
        
        public CookieContainer _cookies { get; set; }
        HttpWebRequest _currentRequest;
        HttpWebResponse _theResponse;
        List<string> _postQueryData;
        
        #endregion

        public List<string> OutputMessage;

        public ExtendedWebClient()
        {
            _cookies = new CookieContainer();
        }

        public ExtendedWebClient(CookieContainer cookie)
        {
            _cookies = cookie;
        }

        /// <summary>
        /// Set the request and begin to request for data
        /// </summary>
        /// <param tag="url">Url to request</param>
        public void SetRequest(string url)
        {
            _currentRequest = (HttpWebRequest)WebRequest.Create(url);
            // Must enable cookie in order to access the IVLE pages.
            _currentRequest.CookieContainer = _cookies;
            _currentRequest.Method = "POST";
            _postQueryData = new List<string>();
        }

        /// <summary>
        /// Request a URL with a sequence of fields and values
        /// to be send by POST method.
        /// </summary>
        /// <param tag="url">Url to request.</param>
        /// <param tag="postKeys">Keys in the POST form (if any).</param>
        /// <param tag="postValues">Associated Values for the POST form.</param>
        public void SetRequest(string url, string[] postKeys, string[] postValues)
        {
            SetRequest(url);
            for (var i = 0; i < postKeys.Length; i++)
            {
                Add(postKeys[i], postValues[i]);
            }
        }

        /// <summary>
        /// Download a file.
        /// </summary>
        /// <param tag="url">Url to file to download.</param>
        /// <param tag="localPath">Full path to a local folder.</param>
        /// <param tag="folderName">Name of file to save.</param>
        public void DownloadFile(string url, string localPath, string fileName)
        {
            // Set request and response.
            SetRequest(url);
            GetResponse();

            // Find the file tag returned by the server
            //WebHeaderCollection headers = _theResponse.Headers;
            //string serverFileName = "";
            //string serverDate = "";
            //if (headers.GetKey(5).Equals("\nDate"))
            //{
            //    serverDate = headers.Get(5);
            //}
            //int i = 1;
            //if (!headers.Get(i).Contains("filename"))
            //{
            //    for (i = 0; i < headers.Count; i++)
            //    {
            //        if (headers.Get(i).Contains("filename"))
            //            break;
            //    }
            //}

            //serverFileName = headers.Get(i).Substring(headers.Get(i).IndexOf("filename="));
            //serverFileName = serverFileName.Substring(9).Replace("\"", "");
            //Console.WriteLine("File date = {0}\nFile tag = {1}\n", serverDate, serverFileName);

            // Download the file stream.
            Stream serverFileStream = _theResponse.GetResponseStream();
            // TODO: CHECK IF localPath ends with '\' or not.
            // BUG FOUND: CANNOT HANDLE FILE WITH SPECIAL CHARACTERS.
            // TODO: CHECK FILE NAME AGAINST THE RESPONSE STREAM.
            var localFileStream = new FileStream(localPath + "\\" + fileName, FileMode.Create);

            #region DOWNLOAD_BY_BYTE
            //while (true)
            //{
            //    int b = serverFileStream.ReadByte();
            //    if (b != -1)
            //    {
            //        localFileStream.WriteByte((byte)b);
            //    }
            //    else break;
            //}
            #endregion

            // REPLACE THE ABOVE CODE BY BUFFER DOWNLOADING
            byte[] buffer = new byte[1024];
            while (true)
            {
                int numBytesRead = serverFileStream.Read(buffer, 0, 1024);
                if (numBytesRead > 0)
                {
                    localFileStream.Write(buffer, 0, numBytesRead);
                }
                else break;
            }

            localFileStream.Close();
            serverFileStream.Close();            
        }

        /// <summary>
        /// Add a new key-value pair to the POST request.
        /// </summary>
        /// <param tag="key"></param>
        /// <param tag="value"></param>
        private void Add(string key, string value)
        {
            _postQueryData.Add(String.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)));
        }

        /// <summary>
        /// Get the response stream in string.
        /// </summary>
        /// <returns></returns>
        public string GetResponseString()
        {
            GetResponse();
            var sr = new StreamReader(_theResponse.GetResponseStream());
            return sr.ReadToEnd();
        }

        /// <summary>
        /// Get the response as an array of byte.
        /// </summary>
        /// <returns></returns>
        public byte[] GetResponseRaw()
        {
            var result = new List<byte>();
            GetResponse();
            Stream serverFileStream = _theResponse.GetResponseStream();
            while (true)
            {
                int b = serverFileStream.ReadByte();
                if (b != -1)
                {
                    result.Add((byte)b);
                }
                else break;
            }
            return result.ToArray();
        }


        /// <summary>
        /// Get the response from the server.
        /// After this, _theResponse will hold the response
        /// object that is ready to be used to download the 
        /// stream of response.
        /// </summary>
        private void GetResponse()
        {
            // Set the encoding type
            _currentRequest.ContentType = "application/x-www-form-urlencoded";

            // Build a string containing all the parameters
            string parameters = String.Join("&", _postQueryData.ToArray());
            _currentRequest.ContentLength = parameters.Length;

            // We write the parameters into the request
            var sw = new StreamWriter(_currentRequest.GetRequestStream());
            sw.Write(parameters);
            sw.Close();

            // Execute the query
            _theResponse = (HttpWebResponse)_currentRequest.GetResponse();
        }
    }

}
