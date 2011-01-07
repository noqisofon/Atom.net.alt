/* -*- encoding: utf-8 -*- */
using System;
using System.Text;
using System.Xml.XPath;

using MvpXml;


namespace Atom.Core {


    using Atom.Utils;


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AtomContentConstruct : AtomElement {
        #region constructors
        /// <summary>
        /// 
        /// </summary>
        public AtomContentConstruct()
            : this( "title" ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        public AtomContentConstruct(string local_name)
            : this( local_name, string.Empty ) {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="content"></param>
        public AtomContentConstruct(string local_name, string content)
            : this( local_name, content, DefaultValues.mediaType, DefaultValues.encodedMode ) {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="content"></param>
        /// <param name="type"></param>
        public AtomContentConstruct(string local_name, string content, MediaType type)
            : this( local_name, content, type, DefaultValues.encodedMode ) {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="content"></param>
        /// <param name="type"></param>
        /// <param name="mode"></param>
        public AtomContentConstruct(string local_name, string content, MediaType type, EncodedMode mode) {
            base.LocalName = local_name;
            this.Content = content;
            this.Type = type;
            this.Mode = mode;
        }
        #endregion constructors


        #region properties
        /// <summary>
        /// 
        /// </summary>
        public MediaType Type {
            get { return this.type_; }
            set {
                if ( value == MediaType.UnknownType )
                    value = MediaType.TextPlain;
                else
                    this.type_ = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public EncodedMode Mode {
            get { return this.mode_; }
            set { this.mode_ = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public string Content {
            get { return this.content_; }

            set { this.content_ = value; }
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

                if ( this.Content.Length == 0 )
                    throw new RequiredElementNotFoundException( "The content cannot be empty" );

                if ( this.Mode == EncodedMode.Escaped )
                    this.Content = Utils.AtomUtility.Escape( this.Content );

                base.buffer.Append( this.Content );
                this.writeEndElement();

                return this.buffer.ToString();
            } finally {
                base.buffer.Length = 0;
            }
        }
        #endregion ToString-method


        #region ToString-helper-methods
        protected internal override void writeStartElement() {
            base.buffer.AppendFormat( "<{0}", this.LocalName );

            if ( ( this.Type == MediaType.UnknownType )
                 || ( this.Type == MediaType.ApplicationAtomXml )
                 || ( this.Type == MediaType.ApplicationXAtomXml ) )
                this.Type = DefaultValues.mediaType;

            if ( this.Type != DefaultValues.mediaType )
                base.writeAttribute( "type", AtomUtility.mediaTypeAsString( this.Type ), false, null );

            if ( this.Mode != DefaultValues.encodedMode )
                base.writeAttribute( "mode", this.Mode.ToString().ToLower(), false, null );

            base.writeAttribute( "xml:lang", AtomUtility.languageAsString( base.XmlLang ).ToString(), false, null );
            base.buffer.Append( ">" );
        }


        /// <summary>
        /// 
        /// </summary>
        protected internal override void writeEndElement() {
            this.buffer.AppendFormat( "</{0}>", this.LocalName ).AppendLine();
        }
        #endregion ToString-helper-methods


        #region xpath-parsing-stuff
        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns></returns>
        internal static AtomContentConstruct parse(XPathNavigator navigator) {
            AtomContentConstruct result_content = new AtomContentConstruct();
            string content = string.Empty;

            XPathNavigator temp_navigator = navigator.Clone();
            XPathNodeIterator it = temp_navigator.SelectDescendants( XPathNodeType.Element, true );

            while ( it.MoveNext() ) {
                string name = it.Current.Name.ToLower();
                int colon_index = name.IndexOf( ":" );

                if ( colon_index != -1 )
                    name = name.Split( new char[] { ':' }, 2 )[1];

                switch ( name ) {
                    case "title":
                    case "copyright":
                    case "info":
                    case "tagline":
                    case "summary":
                    case "content":
                        try {
                            result_content.XmlLang = AtomUtility.parseLanguage( it.Current.XmlLang );
                        } catch { }
                        result_content.LocalName = name;

                        XPathNavigatorReader reader = new XPathNavigatorReader( it.Current );
                        reader.Read();
                        content = reader.ReadInnerXml();
                        break;
                }
            }

            it = temp_navigator.Select( "@*" );
            do {
                switch ( it.Current.Name.ToLower() ) {
                    case "type":
                        result_content.Type = AtomUtility.parseMediaType( it.Current.Value );
                        break;

                    case "mode":
                        switch ( it.Current.Value.ToLower() ) {
                            case "escaped":
                                result_content.Mode = EncodedMode.Escaped;
                                break;

                            case "base64":
                                result_content.Mode = EncodedMode.Base64;
                                break;
                        }
                        break;
                }
            } while ( it.MoveNext() );

            switch ( result_content.Mode ) {
                case EncodedMode.Escaped:
                    content = AtomUtility.Unescape( content );
                    break;

                case EncodedMode.Base64:
                    content = Encoding.Unicode.GetString( Base64.Decode( content ) );
                    break;
            }
            result_content.Content = content;

            return result_content;
        }
        #endregion xpath-parsing-stuff


        #region fields
        /// <summary>
        /// 
        /// </summary>
        private MediaType type_ = DefaultValues.mediaType;
        /// <summary>
        /// 
        /// </summary>
        private EncodedMode mode_ = DefaultValues.encodedMode;
        /// <summary>
        /// 
        /// </summary>
        private string content_ = string.Empty;
        #endregion fields
    }


}
