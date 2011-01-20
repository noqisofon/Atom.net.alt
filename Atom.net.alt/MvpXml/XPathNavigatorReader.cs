/* -*- encoding: utf-8 -*- */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
            this.original_navigator_ = navigator.Clone();
            this.fragment_ = read_fragment;
        }
        #endregion constructors


        #region public-properties
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
        /// <param name="index"></param>
        /// <returns></returns>
        public override string this[int index] {
            get {
                KeyValuePair<string, string> attribute
                    = ( OrderedAttributes as List<KeyValuePair<string, string>> )[index];

                return this[attribute.Key, attribute.Value];
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


        /// <summary>
        /// 
        /// </summary>
        public override string LocalName {
            get {
                if ( this.navigator_.NodeType == XPathNodeType.Namespace
                     && this.navigator_.LocalName.Length == 0 )
                    return XmlNamespaces.XmlNsPrefix;

                if ( this.attribute_value_read_ )
                    return string.Empty;

                return this.navigator_.LocalName;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string Name {
            get {
                if ( this.navigator_.NodeType == XPathNodeType.Namespace ) {
                    string name = this.navigator_.Name;

                    if ( name.Length == 0 )
                        name = XmlNamespaces.XmlNsPrefix;
                    else
                        name = string.Concat( XmlNamespaces.XmlNsPrefix, ":", name );

                    return name;
                }
                if ( this.attribute_value_read_ )
                    return string.Empty;

                return this.navigator_.Name;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string NamespaceURI {
            get {
                return this.navigator_.NodeType == XPathNodeType.Namespace
                    ? XmlNamespaces.XmlNs
                    : this.navigator_.NamespaceURI;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override XmlNameTable NameTable {
            get { return this.navigator_.NameTable; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override XmlNodeType NodeType {
            get {
                if ( this.read_state_ != ReadState.Interactive )
                    return XmlNodeType.None;
                if ( this.is_end_element_ )
                    return XmlNodeType.EndElement;
                if ( this.attribute_value_read_ )
                    return XmlNodeType.Text;

                switch ( this.navigator_.NodeType ) {
                    case XPathNodeType.Attribute:
                    case XPathNodeType.Namespace:
                        return XmlNodeType.Attribute;

                    case XPathNodeType.Comment:
                    case XPathNodeType.Element:
                    case XPathNodeType.ProcessingInstruction:
                    case XPathNodeType.SignificantWhitespace:
                    case XPathNodeType.Text:
                    case XPathNodeType.Whitespace:
                        return (XmlNodeType)Enum.Parse( typeof( XmlNodeType ),
                                                        Enum.GetName( typeof( XPathNodeType ),
                                                                      this.navigator_.NodeType ) );

                    case XPathNodeType.Root:
                        return XmlNodeType.Document;

                    default:
                        return XmlNodeType.None;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string Prefix {
            get { return this.navigator_.Prefix; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override char QuoteChar {
            get { return '"'; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override ReadState ReadState {
            get { return this.read_state_; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string Value {
            get { return HasValue ? this.navigator_.Value : string.Empty; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string XmlLang {
            get { return this.navigator_.XmlLang; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override XmlSpace XmlSpace {
            get { return XmlSpace.Default; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override int AttributeCount {
            get { return OrderedAttributes.Count; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string BaseURI {
            get { return this.navigator_.BaseURI; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override int Depth {
            get { return this.depth_; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool EOF {
            get { return this.eof_; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool HasValue {
            get {
                return ( this.navigator_.NodeType == XPathNodeType.Namespace
                        || this.navigator_.NodeType == XPathNodeType.Attribute
                        || this.navigator_.NodeType == XPathNodeType.Comment
                        || this.navigator_.NodeType == XPathNodeType.ProcessingInstruction
                        || this.navigator_.NodeType == XPathNodeType.Text
                        || this.navigator_.NodeType == XPathNodeType.Whitespace );
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool HasAttributes {
            get { return this.navigator_.HasAttributes; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsDefault {
            get { return false; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsEmptyElement {
            get { return !this.navigator_.HasChildren; }
        }
        #endregion public-properties


        #region private-properties
        /// <summary>
        /// 
        /// </summary>
        private ICollection<KeyValuePair<string, string>> OrderedAttributes {
            get {
                if ( this.ordered_attributes_ != null )
                    return this.ordered_attributes_;

                this.ordered_attributes_ = new List<KeyValuePair<string, string>>();

                if ( this.is_end_element_ )
                    return this.ordered_attributes_;

                XPathNavigator temp_navigator = null;
                if ( this.navigator_.HasAttributes ) {
                    temp_navigator = this.navigator_.Clone();
                    this.ordered_attributes_ = new List<KeyValuePair<string, string>>();

                    if ( temp_navigator.NodeType == XPathNodeType.Attribute
                         || temp_navigator.NodeType == XPathNodeType.Namespace )
                        temp_navigator.MoveToParent();

                    if ( temp_navigator.MoveToFirstAttribute() ) {
                        this.ordered_attributes_.Add(
                            new KeyValuePair<string, string>( temp_navigator.LocalName,
                                                              temp_navigator.NamespaceURI ) );

                        while ( temp_navigator.MoveToNextAttribute() )
                            this.ordered_attributes_.Add(
                                new KeyValuePair<string, string>( temp_navigator.LocalName,
                                                                  temp_navigator.NamespaceURI ) );
                    }

                }

                temp_navigator = this.navigator_.Clone();
                if ( temp_navigator.MoveToFirstNamespace( XPathNamespaceScope.Local ) ) {
                    if ( temp_navigator.Value == XmlNamespaces.Xml )
                        this.ordered_attributes_.Add(
                            new KeyValuePair<string, string>( temp_navigator.LocalName,
                                                              XmlNamespaces.XmlNs ) );

                    while ( temp_navigator.MoveToNextNamespace( XPathNamespaceScope.Local ) ) {
                        if ( temp_navigator.Value == XmlNamespaces.Xml )
                            this.ordered_attributes_.Add(
                                new KeyValuePair<string, string>( temp_navigator.LocalName,
                                                                  XmlNamespaces.XmlNs ) );
                    }
                }
                return this.ordered_attributes_;
            }

        }
        #endregion private-properties


        #region public-methods
        /// <summary>
        /// 
        /// </summary>
        public override void Close() {
            this.read_state_ = ReadState.Closed;
            this.eof_ = true;
        }


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
        /// <param name="index"></param>
        /// <returns></returns>
        public override string GetAttribute(int index) {
            return this[index];
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public override string LookupNamespace(string prefix) {
            string result_namespace = this.navigator_.GetNamespace( prefix );

            return result_namespace.Length == 0 ? null : result_namespace;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override bool MoveToAttribute(string name) {
            return MoveToAttribute( name, string.Empty );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        public override void MoveToAttribute(int offset) {
            KeyValuePair<string, string> pair =
                ( OrderedAttributes as List<KeyValuePair<string, string>> )[offset];

            MoveToAttribute( pair.Key, pair.Value );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="namespace_uri"></param>
        /// <returns></returns>
        public override bool MoveToAttribute(string name, string namespace_uri) {
            if ( this.navigator_.NodeType == XPathNodeType.Attribute
                 || this.navigator_.NodeType == XPathNodeType.Namespace ) {
                this.navigator_.MoveToParent();
                --this.depth_;
            }

            bool moved = this.navigator_.MoveToAttribute( name, namespace_uri );

            if ( moved )
                ++this.depth_;

            return moved;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool MoveToElement() {
            if ( this.navigator_.NodeType == XPathNodeType.Attribute
                 || this.navigator_.NodeType == XPathNodeType.Namespace ) {
                this.navigator_.MoveToParent();
                --this.depth_;
                this.attribute_value_read_ = false;

                return true;
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool MoveToFirstAttribute() {
            if ( this.is_end_element_ )
                return false;

            bool moved = this.navigator_.MoveToFirstAttribute();

            if ( !moved )
                moved = this.navigator_.MoveToFirstNamespace( XPathNamespaceScope.Local );
            if ( moved && this.navigator_.Value == XmlNamespaces.Xml )
                return MoveToNextAttribute();
            if ( moved ) {
                ++this.depth_;
                this.attribute_value_read_ = false;
            }
            return moved;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool MoveToNextAttribute() {
            bool moved = false;

            if ( this.navigator_.NodeType == XPathNodeType.Attribute ) {
                moved = this.navigator_.MoveToNextAttribute();

                if ( moved ) {
                    XPathNavigator last_attributes = this.navigator_.Clone();

                    this.navigator_.MoveToParent();
                    moved = this.navigator_.MoveToFirstNamespace( XPathNamespaceScope.Local );

                    if ( !moved )
                        this.navigator_.MoveTo( last_attributes );
                }
            } else if ( this.navigator_.NodeType == XPathNodeType.Namespace )
                moved = this.navigator_.MoveToNextNamespace( XPathNamespaceScope.Local );
            else if ( this.navigator_.NodeType == XPathNodeType.Element )
                return MoveToFirstAttribute();

            if ( moved && this.navigator_.Value == XmlNamespaces.Xml )
                return MoveToNextAttribute();

            if ( moved )
                this.attribute_value_read_ = false;

            return moved;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Read() {
            if ( this.read_state_ == ReadState.Closed || this.read_state_ == ReadState.EndOfFile )
                return false;
            if ( this.read_state_ == ReadState.Initial ) {
                this.read_state_ = ReadState.Interactive;
                if ( this.navigator_.NodeType == XPathNodeType.Root ) {
                    this.original_navigator_.MoveToFirstChild();

                    return this.navigator_.MoveToFirstChild();
                }
                return true;
            }
            this.ordered_attributes_ = null;
            this.attribute_value_read_ = false;

            if ( this.navigator_.NodeType == XPathNodeType.Attribute
                 || this.navigator_.NodeType == XPathNodeType.Namespace ) {
                this.navigator_.MoveToParent();
                --this.depth_;
            }

            if ( this.is_end_element_ ) {
                if ( this.flagment_ ) {

                    if ( this.navigator_.IsSamePosition( this.original_navigator_ )
                         && this.original_navigator_.MoveToNext() ) {
                        this.is_end_element_ = false;
                        this.navigator_.MoveToNext();

                        return true;
                    }
                } else if ( this.navigator_.IsSamePosition( this.original_navigator_ ) ) {
                    this.eof_ = true;
                    this.read_state_ = ReadState.EndOfFile;

                    return false;
                }

                if ( this.navigator_.MoveToNext() ) {
                    this.is_end_element_ = false;

                    return true;
                } else {
                    this.navigator_.MoveToParent();
                    --this.depth_;

                    return true;
                }
            } else {
                if ( this.navigator_.HasChildren ) {
                    ++this.depth_;

                    return this.navigator_.MoveToFirstChild();
                }

                if ( this.navigator_.MoveToNext() )
                    return true;
                else {
                    this.navigator_.MoveToParent();
                    --this.depth_;
                    this.is_end_element_ = true;

                    return true;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool ReadAttributeValue() {
            if ( !this.attribute_value_read_
                 && ( this.navigator_.NodeType == XPathNodeType.Attribute
                      || this.navigator_.NodeType == XPathNodeType.Namespace ) ) {
                this.attribute_value_read_ = true;

                return true;
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ReadInnerXml() {
            return this.Read() ? ReadFragmentXml() : string.Empty;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ReadOuterXml() {
            if ( this.read_state_ != ReadState.Interactive )
                return string.Empty;
            else {
                StringWriter text_writer = new StringWriter( CultureInfo.CurrentCulture );
                XmlTextWriter xml_writer = new XmlTextWriter( text_writer );

                xml_writer.WriteNode( this, false );
                text_writer.Flush();

                return text_writer.ToString();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ReadFragmentXml() {
            XPathNavigator current = this.navigator_.Clone();

            StringWriter text_writer = new StringWriter( CultureInfo.CurrentCulture );
            XmlTextWriter xml_writer = new XmlTextWriter( text_writer );

            do {
                xml_writer.WriteNode( this, false );
            } while ( current.MoveToNext() && !base.EOF );

            text_writer.Flush();

            return text_writer.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        public override void ResolveEntity() {
        }
        #endregion public-methods


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
        XPathNavigator original_navigator_;
        /// <summary>
        /// 
        /// </summary>
        bool fragment_ = false;
        /// <summary>
        /// 
        /// </summary>
        bool is_end_element_ = false;
        /// <summary>
        /// 
        /// </summary>
        ICollection<KeyValuePair<string, string>> ordered_attributes_;
        /// <summary>
        /// 
        /// </summary>
        int depth_ = 0;
        /// <summary>
        /// 
        /// </summary>
        bool eof_ = false;
        /// <summary>
        /// 
        /// </summary>
        bool attribute_value_read_ = false;
        /// <summary>
        /// 
        /// </summary>
        ReadState read_state_ = ReadState.Initial;
        /// <summary>
        /// 
        /// </summary>
        bool flagment_ = false;
        #endregion fields
    }


}
