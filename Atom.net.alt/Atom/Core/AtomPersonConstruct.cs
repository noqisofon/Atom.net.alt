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
    public class AtomPersonConstruct : AtomElement {
        #region constructor
        /// <summary>
        /// 
        /// </summary>
        public AtomPersonConstruct()
            : this( "author" ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        public AtomPersonConstruct(string local_name)
            : this( local_name, string.Empty ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="name"></param>
        public AtomPersonConstruct(string local_name, string name)
            : this( local_name, name, DefaultValues.Uri ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        public AtomPersonConstruct(string local_name, string name, Uri url)
            : this( local_name, name, url, string.Empty ) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="email"></param>
        public AtomPersonConstruct(string local_name, string name, Uri url, string email) {
            base.LocalName = local_name;
            this.Name = name;
            this.Url = url;
            this.Email = email;
        }
        #endregion constructor


        #region properties
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string Name {
            get { return this.name_; }
            set { this.name_ = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public Uri Url {
            get { return this.url_; }
            set { this.url_ = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public string Email {
            get { return this.email_; }
            set {
                if ( AtomUtility.IsEmail( value ) )
                    this.email_ = value;
            }
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
                base.writeElement( "name", this.Name, true, "The name of author must be specified." );
                base.writeElement( "url", this.Url, false, null );
                base.writeElement( "email", this.Email, false, null );
                this.writeEndElement();

                return base.buffer.ToString();
            } finally {
                base.buffer.Length = 0;
            }
        }
        #endregion ToString-method


        #region ToString-helper-method
        /// <summary>
        /// 
        /// </summary>
        protected internal override void writeStartElement() {
            base.buffer.AppendFormat( "<{0}", base.LocalName );
            base.writeAttribute( "xml:lang", AtomUtility.languageAsString( base.XmlLang ), false, null );
            base.buffer.AppendLine( ">" );
        }


        /// <summary>
        /// 
        /// </summary>
        protected internal override void writeEndElement() {
            base.buffer.AppendFormat( "</{0}>", base.LocalName ).AppendLine();
        }
        #endregion ToString-helper-method


        #region xpath-parsing-stuff
        /// <summary>
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns></returns>
        internal static AtomPersonConstruct parse(XPathNavigator navigator) {
            AtomPersonConstruct result = new AtomPersonConstruct();

            XPathNavigator temp_navigator = navigator.Clone();
            XPathNodeIterator it = temp_navigator.SelectDescendants( XPathNodeType.Element, true );

            while ( it.MoveNext() ) {
                string name = it.Current.Name.ToLower();
                int colon_index = name.IndexOf( ":" );

                if ( colon_index != -1 )
                    name = name.Split( new char[] { ':' }, 2 )[1];

                switch ( name ) {
                    case "contributor":
                    case "author":
                        try {
                            result.XmlLang = AtomUtility.parseLanguage( it.Current.XmlLang );
                        } catch { }
                        result.LocalName = name;
                        break;

                    case "name":
                        result.Name = it.Current.Value;
                        break;

                    case "location":
                    case "url":
                        result.Url = resolveUri( xmlBaseRootUri, it.Current.Value );
                        break;

                    case "email":
                        result.Email = it.Current.Value;
                        break;
                }
            }
            return result;
        }
        #endregion xpath-parsing-stuff


        #region fields
        /// <summary>
        /// 
        /// </summary>
        private string name_;
        /// <summary>
        /// 
        /// </summary>
        private Uri url_;
        /// <summary>
        /// 
        /// </summary>
        private string email_;
        #endregion fields
    }


}
