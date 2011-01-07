using System;


namespace Atom.Core {

    
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RequiredElementNotFoundException : ApplicationException {
        /// <summary>
        /// 
        /// </summary>
        public RequiredElementNotFoundException() { 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public RequiredElementNotFoundException(string message)
            : base( message ) 
        {
        }
        /// <summary>
        /// 
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public RequiredElementNotFoundException(string message, Exception inner)
            : base( message, inner ) 
        {
        }
    }


}
