/* -*- encoding: utf-8 -*- */

using System;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using MvpXml;


namespace Atom.Core {


    using Atom.Utils;


    /// <summary>
    ///
    /// </summary>
    public class AtomLink : AtomElement {
        #region constructor
        /// <summary>
        /// 
        /// </summary>
        public AtomLink()
            : this( DefaultValues.Uri, DefaultValues.Rel, DefaultValues.MediaType, string.Empty ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="href"></param>
        /// <param name="rel"></param>
        /// <param name="media_type"></param>
        public AtomLink(Uri href, Relationship rel, MediaType media_type)
            : this( href, rel, media_type, string.Empty ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="href"></param>
        /// <param name="rel"></param>
        /// <param name="media_type"></param>
        /// <param name="title"></param>
        public AtomLink(Uri href, Relationship rel, MediaType media_type, string title) {
            this.href_ = href;
            this.rel_ = rel;
            this.type_ = media_type;
            this.title_ = title;
        }
        #endregion constructor


        public override string LocalName {
            get { return "link"; }
        }


        #region properties
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>        
        public Uri HRef {
            get { return this.href_; }
            set { this.href_ = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public Relationship Rel {
            get { return this.rel_; }
            set { this.rel_ = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public MediaType Type {
            get { return this.type_; }
            set { this.type_ = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string Title {
            get { return this.title_; }
            set { this.title_ = value; }
        }
        #endregion properties


        #region ToString-method
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            try {
                this.writeStartElement();
                
                base.writeAttribute( "xml:lang", AtomUtility.languageAsString( base.XmlLang ), false, null );
                base.writeAttribute( "rel", AtomUtility.relationshipAsString( this.Rel ), false, null );

                if ( ( this.Type == MediaType.UnknownType )
                     || ( this.Type == MediaType.ApplicationAtomXml )
                     || ( this.Type == MediaType.ApplicationXAtomXml ) )
                    this.Type = DefaultValues.MediaType;

                base.writeAttribute( "type", AtomUtility.mediaTypeAsString( this.Type ), false, null );
                base.writeAttribute( "href", this.HRef, false, string.Empty );
                base.writeAttribute( "title", this.Title, false, string.Empty );
                this.writeEndElement();

                return base.buffer.ToString();
            } finally {
                base.buffer.Length = 0;
            }
        }
        #endregion ToString-method


        #region ToString-helper-methods
        /// <summary>
        /// 
        /// </summary>
        protected internal override void writeStartElement() {
            base.buffer.AppendFormat( "<{0}", base.LocalName );
        }


        /// <summary>
        /// 
        /// </summary>
        protected internal override void writeEndElement() {
            base.buffer.AppendLine( " />" );
        }
        #endregion ToString-helper-methods


        #region xpath-parsing-stuff
        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigator"></param>
        internal static AtomLink parse(XPathNavigator navigator) {
            AtomLink result = new AtomLink();

            XPathNavigator temp_navigator = navigator.Clone();
            XPathNodeIterator it = temp_navigator.SelectDescendants( XPathNodeType.Element, true );

            while ( it.MoveNext() ) {
                switch ( it.Current.Name.ToLower() ) {
                    case "link":
                        try {
                            result.XmlLang = AtomUtility.parseLanguage( it.Current.XmlLang );
                        } catch { }
                        break;
                }
            }

            it = temp_navigator.Select( "@*" );
            do {
                switch ( it.Current.Name.ToLower() ) {
                    case "rel":
                        result.Rel = AtomUtility.parseRelationship( it.Current.Value );
                        break;

                    case "type":
                        result.Type = AtomUtility.parseMediaType( it.Current.Value );
                        break;

                    case "href":
                        result.HRef = resolveUri( xmlBaseRootUri, it.Current.Value );
                        break;

                    case "title":
                        result.Title = it.Current.Value;
                        break;
                }
            } while ( it.MoveNext() );

            return result;
        }
        #endregion xpath-parsing-stuff


        #region fields
        /// <summary>
        /// 
        /// </summary>
        private Uri href_;
        /// <summary>
        /// 
        /// </summary>
        private Relationship rel_;
        /// <summary>
        /// 
        /// </summary>
        private MediaType type_;
        /// <summary>
        /// 
        /// </summary>
        private string title_;
        #endregion fields
    }


}
