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
    public class AtomContent : AtomContentBase {
        #region constructors
        /// <summary>
        /// 
        /// </summary>
        public AtomContent()
        : this( string.Empty, DefaultValues.mediaType, DefaultValues.encodedMode ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        public AtomContent(string local_name)
            : this( local_name, string.Empty ) {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="content"></param>
        public AtomContent(string local_name, string content)
            : this( local_name, content, DefaultValues.mediaType, DefaultValues.encodedMode ) {

        }
        public AtomContent(string local_name, string content, MediaType type)
            : this( local_name, content, type, DefaultValues.encodedMode ) {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="content"></param>
        /// <param name="type"></param>
        public AtomContent(string local_name, string content, EncodedMode mode)
            : this( local_name, content, DefaultValues.mediaType, mode ) {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="content"></param>
        /// <param name="type"></param>
        /// <param name="mode"></param>
        public AtomContent(string local_name, string content, MediaType type, EncodedMode mode) {
            base.LocalName = local_name;
            this.Content = content;
            this.Type = type;
            this.Mode = mode;
        }
        #endregion constructors


        public override string LocalName {
            get { return "content"; }
        }


        #region ToString-helper-methods
        protected internal override void writeStartElement() {
            this.buffer.AppendFormat( "<{0}", this.LocalName );

            if ( ( this.Type == MediaType.UnknownType )
                 || ( this.Type == MediaType.ApplicationAtomXml )
                 || ( this.Type == MediaType.ApplicationXAtomXml ) )
                this.Type = DefaultValues.mediaType;

            base.writeAttribute( "xml:base", base.XmlBase, false, null );

            if ( this.Type != DefaultValues.mediaType )
                base.writeAttribute( "type", AtomUtility.mediaTypeAsString( this.Type ), false, null );

            if ( this.Mode != DefaultValues.encodedMode )
                base.writeAttribute( "mode", this.Mode.ToString().ToLower(), false, null );

            base.writeAttribute( "xml:lang", AtomUtility.languageAsString( base.XmlLang ).ToString(), false, null );
            base.buffer.Append( ">" );
        }
        #endregion ToString-helper-methods


        #region xpath-parsing-stuff
        internal new static AtomContent parse(XPathNavigator navigator) {
            AtomContent result_content = new AtomContent();
            string content = string.Empty;

            XPathNavigator temp_navigator = navigator.Clone();
            XPathNodeIterator it = temp_navigator.SelectDescendants( XPathNodeType.Element, true );

            while ( it.MoveNext() ) {
                string name = it.Current.Name.ToLower();
                int colon_index = name.IndexOf( ":" );

                if ( colon_index != -1 )
                    name = name.Split( new char[] { ':' }, 2 )[1];

                switch ( name ) {
                    case "content":
                        try {
                            XPathNavigatorReader temp_reader = new XPathNavigatorReader( temp_navigator );
                            string base_uri = temp_reader.GetAttribute( "base", XmlNamespaces.Xml );

                            if ( base_uri != null && base_uri.Length > 0 )
                                result_content.XmlBase = new Uri( base_uri );
                        } catch { }
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
    }


}
