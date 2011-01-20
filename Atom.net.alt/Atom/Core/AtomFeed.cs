/* -*- encoding: utf-8 -*- */
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using MvpXml;


namespace Atom.Core {


    using Atom.Core.Collections;
    using Atom.Utils;


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AtomFeed : AtomElement {
        /// <summary>
        /// 
        /// </summary>
        public AtomFeed() { }


        #region properties
        /// <summary>
        /// 
        /// </summary>
        public string Version {
            get { return DefaultValues.AtomVersion; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public AtomContentConstruct Title {
            get {
                return this.title_;
            }
            set {
                this.title_ = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public AtomLinkCollection Links {
            get { return this.links_; }
        }


        /// <summary>
        /// 
        /// </summary>
        public AtomPersonConstruct Author {
            get {
                return this.author_;
            }
            set {
                this.author_ = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public AtomPersonConstructCollection Contributors {
            get {
                return this.contributors_;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public AtomContentConstruct Tagline {
            get {
                return this.tagline_;
            }
            set {
                this.tagline_ = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Uri Id {
            get {
                return this.id_;
            }
            set {
                this.id_ = value;
            }
        }


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
        public AtomContentConstruct Copyright {
            get {
                return this.copyright_;
            }
            set {
                this.copyright_ = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public AtomContentConstruct Info {
            get {
                return this.info_;
            }
            set {
                this.info_ = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public AtomDateConstruct Updated {
            get {
                return this.updated_;
            }
            set {
                this.updated_ = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Encoding Encoding {
            get { return this.encoding_; }
            set { this.encoding_ = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string LocalName {
            get { return "feed"; }
        }
        #endregion properties


        #region ToString-methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            this.writeStartElement();
            {
                if ( this.Title == null )
                    throw new RequiredElementNotFoundException( "The title element must be specified." );


                foreach ( AtomLink link in this.Links )
                    base.buffer.Append( link.ToString() );

                foreach ( AtomEntry entry in this.Entries ) {
                    if ( !entry.doAuthor ) {
                        if ( this.Author == null )
                            throw new RequiredElementNotFoundException( "The author element must be specified." );

                        base.buffer.Append( this.Author.ToString() );

                        break;
                    } else
                        break;
                }

                foreach ( AtomPersonConstruct contributor in this.Contributors )
                    base.buffer.Append( contributor.ToString() );

                if ( this.Tagline != null )
                    base.buffer.Append( this.Tagline.ToString() );
                if ( this.Id != DefaultValues.Uri )
                    base.writeElement( "id", this.Id, false, null );

                base.buffer.Append( this.generator_.ToString() );

                if ( this.Info != null )
                    base.buffer.Append( this.Info.ToString() );
                if ( this.Updated == null )
                    throw new RequiredElementNotFoundException( "The updated element must be specified." );

                base.buffer.Append( this.Updated.ToString() );

                foreach ( ScopedElement element in AddtionalElements )
                    base.buffer.Append( element.ToString() );
            }
            this.writeEndElement();
            try {
                return base.buffer.ToString();
            } finally {
                base.buffer.Length = 0;
            }
        }
        #endregion ToString-methods


        #region writing-stuff
        /// <summary>
        /// 
        /// </summary>
        protected internal override void writeStartElement() {
            base.buffer.AppendFormat( "<{0}", this.LocalName );
            {
                base.writeAttribute( "version", DefaultValues.AtomVersion, false, null );
                if ( this.XmlLang != Language.UnknownLanguage )
                    base.writeAttribute( "xml:lang", AtomUtility.languageAsString( this.XmlLang ), false, null );
                if ( this.NamespaceUri != null && !string.IsNullOrEmpty( this.NamespaceUri.ToString() ) )
                    base.writeAttribute( "xmlns", this.NamespaceUri, false, null );
            }
            base.buffer.AppendLine( ">" );
        }


        /// <summary>
        /// 
        /// </summary>
        protected internal override void writeEndElement() {
            base.buffer.AppendFormat( "</{0}>", this.LocalName ).AppendLine();
        }
        #endregion writing-stuff


        #region save-methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream) {
            writer = new AtomWriter( stream, this.Encoding );

            writer.write( this );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename) {
            writer = new AtomWriter( filename, this.Encoding );

            writer.write( this );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text_writer"></param>
        public void Save(TextWriter text_writer) {
            writer = new AtomWriter( text_writer );

            writer.write( this );
        }
        #endregion save-methods


        #region load-methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static AtomFeed Load(string uri) {
            reader = new AtomReader( uri );

            return parse( reader.navigator );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static AtomFeed Load(Uri uri) {
            return Load( uri.ToString() );
        }
        #endregion load-methods


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
                        AtomContentConstruct content = AtomContentConstruct.parse( it.Current );
                        result_feed.Title = content;
                        break;

                    case "link":
                        result_feed.Links.Add( AtomLink.parse( it.Current ) );
                        break;

                    case "author":
                        result_feed.Author = AtomPersonConstruct.parse( it.Current );
                        break;

                    case "contributor":
                        result_feed.Contributors.Add( AtomPersonConstruct.parse( it.Current ) );
                        break;

                    case "tagline":
                        result_feed.Tagline = AtomContent.parse( it.Current );
                        break;

                    case "id":
                        result_feed.Id = new Uri( it.Current.Value );
                        break;

                    case "copyright":
                        result_feed.Copyright = AtomContentConstruct.parse( it.Current );
                        break;

                    case "info":
                        result_feed.Info = AtomContentConstruct.parse( it.Current );
                        break;

                    case "updated":
                        result_feed.Updated = AtomDateConstruct( it.Current );
                        break;

                    case "entry":
                        result_feed.Add( AtomEntry.Parse( it.Current ) );
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


        #region fields
        /// <summary>
        /// 
        /// </summary>
        private AtomContentConstruct title_ = null;
        /// <summary>
        /// 
        /// </summary>
        private AtomLinkCollection links_ = new AtomLinkCollection();
        /// <summary>
        /// 
        /// </summary>
        private AtomPersonConstruct author_ = null;
        /// <summary>
        /// 
        /// </summary>
        private AtomPersonConstructCollection contributors_ = new AtomPersonConstructCollection();
        /// <summary>
        /// 
        /// </summary>
        private AtomContentConstruct tagline_ = null;
        /// <summary>
        /// 
        /// </summary>
        private Uri id_ = DefaultValues.Uri;
        /// <summary>
        /// 
        /// </summary>
        private Uri feed_uri_ = null;
        /// <summary>
        /// 
        /// </summary>
        private AtomGenerator generator_ = new AtomGenerator();
        /// <summary>
        /// 
        /// </summary>
        private AtomContentConstruct copyright_ = null;
        /// <summary>
        /// 
        /// </summary>
        private AtomContentConstruct info_ = null;
        /// <summary>
        /// 
        /// </summary>
        private AtomDateConstruct updated_ = null;
        /// <summary>
        /// 
        /// </summary>
        private Encoding encoding_ = DefaultValues.Encoding;
        #endregion fields


        /// <summary>
        /// 
        /// </summary>
        private static AtomReader reader = null;
        /// <summary>
        /// 
        /// </summary>
        private static AtomWriter writer = null;

    }


}
