using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;


namespace MvpXml {


    /// <summary>
    /// 
    /// </summary>
    internal class XPathNavigatorReader : XmlTextReader, IXmlSerializable {
        #region constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigator"></param>
        public XPathNavigatorReader(XPathNavigator navigator)
            : this( navigator, false ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigator"></param>
        /// <param name="read_fragment"></param>
        public XPathNavigatorReader(XPathNavigator navigator, bool read_fragment)
            : base( new StringReader( string.Empty ) ) {
            this.navigator_ = navigator.Clone();
            this.original_ = navigator.Clone();
            this.fragment_ = read_fragment;
        }
        #endregion constructors


        #region property
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string this[string name] {
            get {
                return this[name, string.Empty];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="namespaceURI"></param>
        /// <returns></returns>
        public override string this[string name, string namespaceURI] {
            get {
                string attribute = string.Empty;

                if ( namespaceURI == XmlNamespaces.XmlNs )
                    attribute = this.navigator_.GetNamespace( name );
                else
                    attribute = this.navigator_.GetAttribute( name, namespaceURI );

                return attribute.Length != 0 ? attribute : null;
            }
        }
        #endregion property


        #region methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetAttribute(string name) {
            return this[name];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="namespaceURI"></param>
        /// <returns></returns>
        public override string GetAttribute(string name, string namespaceURI) {
            return this[name, namespaceURI];
        }
        #endregion methods


        #region IXmlSerializable-members
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema() {
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        void IXmlSerializable.ReadXml(XmlReader reader) {
            XPathDocument document = new XPathDocument( reader );
            this.navigator_ = document.CreateNavigator();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void IXmlSerializable.WriteXml(XmlWriter writer) {
            writer.WriteNode( this, false );
        }
        #endregion IXmlSerializable-members


        #region fields
        /// <summary>
        /// 
        /// </summary>
        XPathNavigator navigator_;
        /// <summary>
        /// 
        /// </summary>
        XPathNavigator original_;
        /// <summary>
        /// 
        /// </summary>
        bool fragment_ = false;
        /// <summary>
        /// 
        /// </summary>
        bool is_end_element_ = false;
        #endregion fields
    }


}
