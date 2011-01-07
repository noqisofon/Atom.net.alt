/* -*- encoding: utf-8 -*- */
using System;
using System.Text;


namespace Atom.Utils {


    /// <summary>
    /// 
    /// </summary>
    public static class Base64 {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="byte_array"></param>
        /// <returns></returns>
        public static string Encode(byte[] byte_array) {
            return Convert.ToBase64String( byte_array );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="byte_array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Encode(byte[] byte_array, int offset, int length) {
            return Convert.ToBase64String( byte_array );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string Encode(string buffer) {
            return Encode( Encoding.ASCII.GetBytes( buffer ) );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public static byte[] Decode(char[] cs) {
            return Convert.FromBase64CharArray( cs, 0, cs.Length );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] Decode(char[] cs, int offset, int length) {
            return Convert.FromBase64CharArray( cs, offset, length );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] Decode(string buffer) {
            return Convert.FromBase64String( buffer );
        }
    }


}
