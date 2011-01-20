/* -*- encoding: utf-8 -*- */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Web;


namespace Atom.Utils {


    using Atom.Core;


    /// <summary>
    /// 
    /// </summary>
    public class AtomUtility {
        #region constructor
        /// <summary>
        /// 
        /// </summary>
        static AtomUtility() {
            //Assembly module = Assembly.GetExecutingAssembly();
            //Stream stream = module.GetManifestResourceStream( "Atom.NET.mediatypes.txt" );
            mediaTypes = new Dictionary<int, string>();

            //using ( StreamReader reader = new StreamReader( stream ) ) {
            //    string line = string.Empty;
            //    string type = string.Empty;

            //    int i = 0;

            //    line = reader.ReadLine();
            //    while ( line != null ) {
            //        type = line.Split( new char[] { ' ' }, 2 )[0];
            //        mediaTypes.Add( i, type );

            //        ++i;
            //        line = reader.ReadLine();
            //    }
            //}
        }
        #endregion constructor


        #region public-methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string Escape(string buffer) {
            return HttpUtility.HtmlEncode( buffer );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string Unescape(string buffer) {
            return HttpUtility.HtmlDecode( buffer );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="the_date"></param>
        /// <returns></returns>
        public static bool IsIso8601Date(string the_date) {
            string rexpression = @"\d\d\d\d(-\d\d(-\d\d(T\d\d:\d\d(:\d\d(\.\d*)?)?(Z|([+-]\d\d:\d\d))?)?)?)?$";
            Regex re = new Regex( rexpression, RegexOptions.IgnoreCase );

            if ( re.IsMatch( the_date ) )
                return true;

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="the_date"></param>
        /// <returns></returns>
        public static bool IsIso8601DateTZ(string the_date) {
            if ( IsIso8601Date( the_date ) ) {
                string rexpression = @"Z|([+-]\d\d:\d\d)$";
                Regex re = new Regex( rexpression, RegexOptions.IgnoreCase | RegexOptions.Multiline );

                if ( re.IsMatch( the_date ) )
                    return true;
                else if ( the_date.IndexOf( 'Z' ) != -1 )
                    return true;
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(string email) {
            string rexpression = @"([a-zA-Z0-9_\-\+\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}[0-9]{1,3})(\]?)$";

            Regex re = new Regex( rexpression, RegexOptions.IgnoreCase | RegexOptions.Multiline );

            return re.IsMatch( email ) ? true : false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetVersion() {
            Assembly assembly = Assembly.GetExecutingAssembly();

            return assembly.GetName().Version.ToString();
        }
        #endregion public-methods


        #region internal-methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        internal static Language parseLanguage(string lang) {
            if ( lang != null && lang.Length > 0 ) {
                string temp = lang.Replace( "-", "_" );

                return (Language)Enum.Parse( typeof( Language ), temp, true );
            }
            return Language.UnknownLanguage;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        internal static string languageAsString(Language lang) {
            if ( lang == Language.UnknownLanguage )
                return string.Empty;

            return lang.ToString().Replace( "_", "-" );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static MediaType parseMediaType(string type) {
            if ( mediaTypes.ContainsValue( type ) ) {
                MediaType media_type = MediaType.UnknownType;

                foreach ( KeyValuePair<int, string> entry in mediaTypes ) {
                    string val = entry.Value;

                    if ( val == type ) {
                        try {
                            int key = entry.Key;

                            media_type = (MediaType)key;
                        } catch {
                            return MediaType.UnknownType;
                        }
                    }
                }
                return media_type;
            }
            return MediaType.UnknownType;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="media_type"></param>
        /// <returns></returns>
        internal static string mediaTypeAsString(MediaType media_type) {
            string result = string.Empty;

            try {
                result = (string)mediaTypes[Convert.ToInt32( media_type )];
            } catch {
                result = "text/plain";
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="relation_ship"></param>
        /// <returns></returns>
        internal static string relationshipAsString(Relationship relation_ship) {
            string result = Enum.GetName( typeof( Relationship ), relation_ship ).ToLower();

            if ( result.IndexOf( "service" ) == 0 )
                result = result.Insert( "service".Length, "." );

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="relation_ship"></param>
        /// <returns></returns>
        internal static Relationship parseRelationship(string relation_ship) {
            Relationship result;

            switch ( relation_ship ) {
                case "start":
                    result = Relationship.Start;
                    break;

                case "next":
                    result = Relationship.Next;
                    break;

                case "prev":
                    result = Relationship.Prev;
                    break;

                case "service.edit":
                    result = Relationship.ServiceEdit;
                    break;

                case "service.post":
                    result = Relationship.ServicePost;
                    break;

                case "service.feed":
                    result = Relationship.ServiceFeed;
                    break;

                default:
                    result = Relationship.Alternate;
                    break;
            }
            return result;
        }
        #endregion internal-methods


        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<int, string> mediaTypes = null;
    }


}
