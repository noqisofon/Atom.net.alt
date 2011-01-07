/* -*- encoding: utf-8 -*- */
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
    public abstract class AtomElement {
        /// <summary>
        /// 
        /// </summary>
        protected AtomElement() {
            this.buffer_ = new StringBuilder();
        }


        #region properties
        /// <summary>
        /// 
        /// </summary>
        public virtual string LocalName {
            get { return this.name_; }
            set { this.name_ = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual string NamespacePrefix {
            get { return DefaultValues.AtomNSPrefix; }
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual Uri NamespaceUri {
            get { return DefaultValues.AtomNSUri; }
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual string FullName {
            get { return string.Concat( NamespacePrefix, ":", LocalName ); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Language XmlLang {
            get { return this.xmllang_; }
            set { this.xmllang_ = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Uri XmlBase {
            get { return this.xmlbase_; }
            set {
                this.xmlbase_ = value;
                if ( this is AtomFeed )
                    xmlBaseRootUri = value.ToString();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected internal StringBuilder buffer {
            get { return this.buffer_; }
        }
        #endregion property


        #region writing-stuff
        /// <summary>
        /// 
        /// </summary>
        protected internal virtual void writeStartElement() { }


        /// <summary>
        /// 
        /// </summary>
        protected internal virtual void writeEndElement() { }
        #endregion writing-stuff


        #region writeElement-helper-methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="input"></param>
        /// <param name="required"></param>
        /// <param name="message"></param>
        protected internal void writeElement(string local_name, DateTime input, bool required, string message) {
            if ( input != DefaultValues.DateTime ) {
                this.buffer.AppendFormat( "<{0}>", local_name );
                this.buffer.Append( Convert.ToString( input ) );
                this.buffer.AppendFormat( "</{0}>", local_name );
                this.buffer.AppendLine();
            } else if ( required )
                throw new RequiredElementNotFoundException( string.Concat( local_name, " ", message ) );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="input"></param>
        /// <param name="required"></param>
        /// <param name="message"></param>
        protected internal void writeElement(string local_name, int input, bool required, string message) {
            if ( input != 0 ) {
                this.buffer.AppendFormat( "<{0}>", local_name );
                this.buffer.Append( Convert.ToString( input ) );
                this.buffer.AppendFormat( "</{0}>", local_name );
                this.buffer.AppendLine();
            } else if ( required )
                throw new RequiredElementNotFoundException( string.Concat( local_name, " ", message ) );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="input"></param>
        /// <param name="required"></param>
        /// <param name="message"></param>
        protected internal void writeElement(string local_name, string input, bool required, string message) {
            if ( input.Length != 0 ) {
                this.buffer.AppendFormat( "<{0}>", local_name );
                this.buffer.Append( input );
                this.buffer.AppendFormat( "</{0}>", local_name );
                this.buffer.AppendLine();
            } else if ( required )
                throw new RequiredElementNotFoundException( string.Concat( local_name, " ", message ) );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="input"></param>
        /// <param name="required"></param>
        /// <param name="message"></param>
        protected internal void writeElement(string local_name, Uri input, bool required, string message) {
            if ( input != DefaultValues.Uri ) {
                this.buffer.AppendFormat( "<{0}>", local_name );
                this.buffer.Append( Convert.ToString( input ) );
                this.buffer.AppendFormat( "</{0}>", local_name );
                this.buffer.AppendLine();
            } else if ( required )
                throw new RequiredElementNotFoundException( string.Concat( local_name, " ", message ) );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="input"></param>
        /// <param name="required"></param>
        /// <param name="message"></param>
        protected internal void writeElement(string local_name, object input, bool required, string message) {
            if ( input as Uri != DefaultValues.Uri ) {
                this.buffer.AppendFormat( "<{0}>", local_name );
                this.buffer.Append( Convert.ToString( input ) );
                this.buffer.AppendFormat( "</{0}>", local_name );
                this.buffer.AppendLine();
            } else if ( required )
                throw new RequiredElementNotFoundException( string.Concat( local_name, " ", message ) );
        }
        #endregion writeElement-helper-methods


        #region writeAttribute-helper-methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="input"></param>
        /// <param name="required"></param>
        /// <param name="message"></param>
        protected internal void writeAttribute(string local_name, DateTime input, bool required, string message) {
            if ( input != DefaultValues.DateTime )
                this.buffer.AppendFormat( "{0}=\"{1}\"", local_name, Convert.ToString( input ) );
            else if ( required )
                throw new RequiredAttributeNotFoundException( string.Concat( local_name, " ", message ) );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="input"></param>
        /// <param name="required"></param>
        /// <param name="message"></param>
        protected internal void writeAttribute(string local_name, int input, bool required, string message) {
            if ( input != DefaultValues.Int )
                this.buffer.AppendFormat( "{0}=\"{1}\"", local_name, Convert.ToString( input ) );
            else if ( required )
                throw new RequiredAttributeNotFoundException( string.Concat( local_name, " ", message ) );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="input"></param>
        /// <param name="required"></param>
        /// <param name="message"></param>
        protected internal void writeAttribute(string local_name, string input, bool required, string message) {
            if ( input.Length != 0 )
                this.buffer.AppendFormat( "{0}=\"{1}\"", local_name, input );
            else if ( required )
                throw new RequiredAttributeNotFoundException( string.Concat( local_name, " ", message ) );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="input"></param>
        /// <param name="required"></param>
        /// <param name="message"></param>
        protected internal void writeAttribute(string local_name, Uri input, bool required, string message) {
            if ( input != DefaultValues.Uri )
                this.buffer.AppendFormat( "{0}=\"{1}\"", local_name, Convert.ToString( input ) );
            else if ( required )
                throw new RequiredAttributeNotFoundException( string.Concat( local_name, " ", message ) );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local_name"></param>
        /// <param name="input"></param>
        /// <param name="required"></param>
        /// <param name="message"></param>
        protected internal void writeAttribute(string local_name, object input, bool required, string message) {
            if ( input as Uri != DefaultValues.Uri )
                this.buffer.AppendFormat( "{0}=\"{1}\"", local_name, Convert.ToString( input ) );
            else if ( required )
                throw new RequiredAttributeNotFoundException( string.Concat( local_name, " ", message ) );
        }
        #endregion writeAttribute-helper-methods


        #region xml-base-processing
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseURI"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        protected internal static Uri resolveUri(string baseURI, string path) {
            if ( baseURI.Length > 0 ) {
                XmlUrlResolver resolver = new XmlUrlResolver();

                return resolver.ResolveUri( new Uri( baseURI ), path );
            }
            return new Uri( path );
        }
        #endregion xml-base-processing


        #region fields
        /// <summary>
        /// 
        /// </summary>
        private StringBuilder buffer_;
        /// <summary>
        /// 
        /// </summary>
        private string name_;
        /// <summary>
        /// 
        /// </summary>
        private Language xmllang_;
        /// <summary>
        /// 
        /// </summary>
        private Uri xmlbase_ = DefaultValues.Uri;
        #endregion fields


        /// <summary>
        /// 
        /// </summary>
        protected internal static string xmlBaseRootUri = string.Empty;
    }


}
