using System;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.XPath;


namespace Atom.Core {


    using Utils;


    /// <summary>
    /// 
    /// </summary>
    public class AtomReader {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        public AtomReader(string uri) {
            this.document_ = new XPathDocument( uri );
            initialize();
        }


        /// <summary>
        /// 
        /// </summary>
        internal XPathNavigator navigator {
            get {
                return this.navigator_;
            }
        }


        #region private-methods
        /// <summary>
        /// 
        /// </summary>
        private void initialize() {
            this.navigator_ = this.document_.CreateNavigator();

            XmlNamespaceManager manager = new XmlNamespaceManager( this.navigator_.NameTable );
            manager.AddNamespace( DefaultValues.AtomNSPrefix, DefaultValues.AtomNSUri.ToString() );
            manager.AddNamespace( DefaultValues.DCNSPrefix, DefaultValues.DCNSUri.ToString() );

            XPathExpression expression = this.navigator_.Compile( "/atom:feed" );
            expression.SetContext( manager );

            XPathNodeIterator iterator = this.navigator_.Select( expression );
            if ( iterator.CurrentPosition == 0 )
                iterator.MoveNext();

            this.navigator_ = iterator.Current;
        }
        #endregion


        /// <summary>
        /// 
        /// </summary>
        private XPathDocument document_ = null;
        /// <summary>
        /// 
        /// </summary>
        private XPathNavigator navigator_ = null;
    }


}
