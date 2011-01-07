using System;
using System.Collections;


namespace Atom.Core.Collections {


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AtomContentCollection : CollectionBase {
        #region Collection-methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AtomContent this[int index] {
            get {
                return base.List[index] as AtomContent;
            }
            set {
                CheckInsertion( value as AtomContent );
                base.List[index] = value as AtomContent;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public int Add(AtomContent content) {
            CheckInsertion( content );

            return base.List.Add( content );
        }


        public bool Contains(AtomContent content)
            {
                return base.List.Contains( content );
            }
        #endregion Collection-methods
    }


}
