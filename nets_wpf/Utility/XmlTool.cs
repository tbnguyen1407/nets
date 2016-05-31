using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace nets_wpf.Utility
{
    /// <summary>
    /// Helper class to support XML data manipulation.
    /// </summary>
    public static class XmlTool
    {
        /// <summary>
        /// Append a new element as child of an XmlNode
        /// </summary>
        /// <param tag="node">Parent node</param>
        /// <param tag="tag">Node tag</param>
        /// <param tag="innerText">Inner text [data]</param>
        /// <returns>The newly added child node.</returns>
        public static XmlElement AddElementToDoc(XmlElement node, string tag, string innerText)
        {
            XmlElement newNode = node.OwnerDocument.CreateElement(tag);
            newNode.InnerText = innerText;
            node.AppendChild(newNode);

            return newNode;
        }

        /// <summary>
        /// Find the datum node with a specific tag and datum under the root.
        /// </summary>
        /// <param tag="root">Root node.</param>
        /// <param tag="tagPath">Data tag.</param>
        /// <param tag="value">Data value.</param>
        /// <returns>The first element node satisfying the condition. Null otherwise.</returns>
        public static XmlElement FindDataNode(XmlNode root, string tagPath, string value)
        {
            foreach (XmlElement node in root.SelectNodes("./" + tagPath))
            {
                if (node.InnerText.Equals(value))
                    // return the node if found!
                    return node;
            }

            return null;
        }

        /// <summary>
        /// Get the inner text representing the
        /// attribute with a tag inside the node.
        /// </summary>
        /// <param tag="node">Node to get data.</param>
        /// <param tag="tagPath">Data tag.</param>
        /// <returns>Inner text</returns>
        public static string GetDatum(XmlNode node, string tagPath)
        {
            XmlElement dataNode = (XmlElement) node.SelectSingleNode("./" + tagPath);

            if (dataNode != null)
            {
                return dataNode.InnerText;
            }
            else return null;
        }

        /// <summary>
        /// Get all inner text of all the first level child
        /// of the current node.
        /// </summary>
        /// <param tag="node">The element node containing data.</param>
        /// <param tag="tag">Data tag (path to tag)</param>
        /// <returns></returns>
        public static List<string> GetAllData(XmlElement node, string tagPath)
        {
            var result = new List<string>();
            if (node == null)

                return null;

            else
            {
                foreach (XmlElement dataNode in node.SelectNodes("./" + tagPath))
                {
                    result.Add(dataNode.InnerText);
                }
            }

            return result;
        }

        /// <summary>
        /// Load the Xml document at a specific path or
        /// create a new one (if it does not exists).
        /// </summary>
        /// <param tag="path">Path to the XML file.</param>
        /// <returns>Xml document or a blank document.</returns>
        public static XmlDocument LoadXmlDoc(string filepath, string defaultXmlDoc)
        {
            if (!File.Exists(filepath))
            {
                File.WriteAllText(filepath, defaultXmlDoc);
            }

            XmlDocument doc = new XmlDocument();
            string xmlText = File.ReadAllText(filepath);
            doc.LoadXml(xmlText);

            return doc;
        }
    }
}
