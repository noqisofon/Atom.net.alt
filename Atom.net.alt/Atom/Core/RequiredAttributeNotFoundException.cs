/* -*- encoding: utf-8 -*- */
using System;


namespace Atom.Core {


    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class RequiredAttributeNotFoundException : ApplicationException {
        /// <summary>
        /// 
        /// </summary>
        public RequiredAttributeNotFoundException() {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public RequiredAttributeNotFoundException(string message)
            : base( message ) {
        }
        /// <summary>
        /// 
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public RequiredAttributeNotFoundException(string message, Exception inner)
            : base( message, inner ) {
        }
    }


}
