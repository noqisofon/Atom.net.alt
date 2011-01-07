using System;
using System.Collections.Generic;
using System.Text;


namespace Atom.Utils {


    using Atom.Core;


    /// <summary>
    ///
    /// </summary>
    public sealed class DefaultValues {
        /// <summary>
        /// 
        /// </summary>
        internal const string AtomVersion = "0.3";
        /// <summary>
        /// 
        /// </summary>
        internal const string AtomNSPrefix = "atom";
        /// <summary>
        /// 
        /// </summary>
        internal static readonly Uri AtomNSUri = new Uri( "http://www.w3.org/2005/Atom" );
        /// <summary>
        /// 
        /// </summary>
        internal const string DCNSPrefix = "dc";
        /// <summary>
        /// 
        /// </summary>
        internal static readonly Uri DCNSUri = new Uri( "http://purl.org/dc/elements/1.1/" );


        /// <summary>
        /// 
        /// </summary>
        internal static MediaType atom_media_type = MediaType.ApplicationAtomXml;


        /// <summary>
        /// 
        /// </summary>
        internal static MediaType atomMediaType {
            get {
                return atom_media_type;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal static MediaType media_type = MediaType.TextPlain;


        /// <summary>
        /// 
        /// </summary>
        internal static MediaType mediaType {
            get {
                return media_type;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal static readonly EncodedMode encoded_mode_ = EncodedMode.Xml;


        /// <summary>
        /// 
        /// </summary>
        internal static EncodedMode encodedMode {
            get { return encoded_mode_; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal static Uri default_uri = new Uri( "http://www.intertwingly.net/wiki/pie/FrontPage" );


        /// <summary>
        /// 
        /// </summary>
        internal static Uri uri {
            get {
                return default_uri;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public const int intValue = -1;


        /// <summary>
        /// 
        /// </summary>
        internal static DateTime date_time = DateTime.MinValue;


        /// <summary>
        /// 
        /// </summary>
        internal static DateTime dateTime {
            get { return date_time; }
        }
    }


}
