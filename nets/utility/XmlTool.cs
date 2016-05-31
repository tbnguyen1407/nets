#region USING DIRECTIVES

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;

#endregion

namespace nets.utility
{
    /// <summary>
    /// Helper class to support XML data manipulation.
    /// Author: Vu An Hoa
    /// </summary>
    public static class XmlTool
    {
        #region MAIN METHODS

        /// <summary>
        /// Append a new element as child of an XmlNode
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="tag">Node tag</param>
        /// <param name="innerText">Inner text [data]</param>
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
        /// <param name="root">Root node.</param>
        /// <param name="tagPath">Data tag.</param>
        /// <param name="value">Data value.</param>
        /// <returns>The first element node satisfying the condition. Null otherwise.</returns>
        public static XmlElement FindDataNode(XmlNode root, string tagPath, string value)
        {
            return root.SelectNodes("./" + tagPath).Cast<XmlElement>().FirstOrDefault(node => node.InnerText.Equals(value));
        }

        /// <summary>
        /// Get the inner text representing the
        /// attribute with a tag inside the node.
        /// </summary>
        /// <param name="node">Node to get data.</param>
        /// <param name="tagPath">Data tag.</param>
        /// <returns>Inner text</returns>
        public static string GetDatum(XmlNode node, string tagPath)
        {
            XmlElement dataNode = (XmlElement) node.SelectSingleNode("./" + tagPath);
            return dataNode != null ? dataNode.InnerText : null;
        }

        /// <summary>
        /// Get all inner text of all the first level child
        /// of the current node.
        /// </summary>
        /// <param name="node">The element node containing data.</param>
        /// <param name="tag">Data tag (path to tag)</param>
        /// <returns></returns>
        public static List<string> GetAllData(XmlElement node, string tagPath)
        {
            return node == null ? null : (from XmlElement dataNode in node.SelectNodes("./" + tagPath) select dataNode.InnerText).ToList();
        }

        /// <summary>
        /// Load the Xml document at a specific path or
        /// create a new one (if it does not exists).
        /// </summary>
        /// <param name="filepath">Path to the XML file.</param>
        /// <param name="defaultXmlDoc"></param>
        /// <returns>Xml document or a blank document.</returns>
        public static XmlDocument LoadXmlDoc(string filepath, string defaultXmlDoc)
        {
            if (!File.Exists(filepath))
                File.WriteAllText(filepath, defaultXmlDoc);

            XmlDocument doc = new XmlDocument();
            string xmlText = File.ReadAllText(filepath);
            doc.LoadXml(xmlText);

            return doc;
        }

        #endregion
    }
}
