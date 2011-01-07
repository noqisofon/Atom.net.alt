using System;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using MvpXml;


namespace Atom.Core {


    using Utils;


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AtomFeed : AtomElement {
        /// <summary>
        /// 
        /// </summary>
        public Uri Uri {
            get {
                return this.feed_uri_;
            }
            set {
                this.feed_uri_ = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static AtomFeed Load(string uri) {
            Reader = new AtomReader( uri );

            return parse( Reader.navigator );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static AtomFeed Load(Uri uri) {

            return Load( uri.ToString() );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns></returns>
        internal static AtomFeed parse(XPathNavigator navigator) {
            AtomFeed result_feed = new AtomFeed();

            XPathNavigator temp_navigator = navigator.Clone();
            XPathNodeIterator it = temp_navigator.SelectChildren( XPathNodeType.All );

            do {
                string name = it.Current.Name.ToLower();
                int colon_index = name.IndexOf( ":" );

                if ( colon_index != -1 )
                    name = name.Split( new char[] { ':' }, 2 )[1];

                switch ( name ) {
                    case "feed":
                        try {
                            XPathNavigatorReader reader = new XPathNavigatorReader( temp_navigator );
                            string base_uri = reader.GetAttribute( "base", XmlNamespaces.Xml );

                            if ( base_uri != null && base_uri.Length > 0 )
                                result_feed.XmlBase = new Uri( base_uri );
                        } catch { }

                        try {
                            result_feed.Uri = findAlternateUri( it.Current );
                        } catch { }

                        try {
                            result_feed.XmlLang = Utils.AtomUtility.parseLanguage( it.Current.XmlLang );
                        } catch { }

                        XPathNodeIterator attr_it = temp_navigator.Select( "@*" );
                        while ( attr_it.MoveNext() ) {
                            if ( attr_it.Current.Name.ToLower() == "version" ) {
                                if ( attr_it.Current.Value != DefaultValues.AtomVersion ) {
                                    string errmsg = string.Format( "Atom {0} version is not supported!", attr_it.Current.Value );

                                    throw new InvalidOperationException( errmsg );
                                }
                            }
                        }
                        break;

                    case "title":
                        AtomContentBase content = AtomContentBase.parse( it.Current );
                        result_feed.Title = content;
                        break;

                    case "link":
                        result_feed.Links.Add( AtomLink.parse( it.Current ) );
                        break;

                    case "author":
                        result_feed.Author = AtomPerson.parse( it.Current );
                        break;

                    case "contributor":
                        result_feed.Contributors.Add( AtomPerson.parse( it.Current ) );
                        break;

                    case "tagline":
                        result_feed.Tagline = AtomContent.parse( it.Current );
                        break;
                }
            } while ( it.MoveNext() );

            return result_feed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigate"></param>
        /// <returns></returns>
        private static Uri findAlternateUri(XPathNavigator navigator) {
            Uri result_uri = null;
            XPathNavigator temp_navigator = navigator.Clone();
            XPathNodeIterator it;

            XmlNamespaceManager manager = new XmlNamespaceManager( temp_navigator.NameTable );
            manager.AddNamespace( DefaultValues.AtomNSPrefix, DefaultValues.AtomNSUri.ToString() );

            XPathExpression expression = temp_navigator.Compile( "child::atom:link[@type=\"text/html\" and @rel=\"alternate\"]" );
            expression.SetContext( manager );
            it = navigator.Select( expression );

            if ( it.Count == 0 ) {
                expression = temp_navigator.Compile( "child::atom:link[\"text/plain\" and @rel=\"alternate\"]" );
                expression.SetContext( manager );
                it = navigator.Select( expression );
            }

            if ( it.CurrentPosition == 0 )
                it.MoveNext();

            do {
                switch ( it.Current.Name.ToLower() ) {
                    case "href":
                        try {
                            result_uri = resolveUri( xmlBaseRootUri, it.Current.Value );
                        } catch { }
                        break;
                }
            } while ( it.MoveNext() );

            return result_uri;
        }


        /// <summary>
        /// 
        /// </summary>
        private Uri feed_uri_ = null;


        /// <summary>
        /// 
        /// </summary>
        private static AtomReader Reader = null;
    }


}
