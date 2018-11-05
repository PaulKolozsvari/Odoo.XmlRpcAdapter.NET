namespace Odoo.XmlRpcAdapter.Results
{
    using Figlut.Server.Toolkit.Data;
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    /// <summary>
    /// Contains the result of the name_get Odoo XML RPC method call.
    /// </summary>
    public class OdooNameGetResult
    {
        #region Constructors

        /// <summary>
        /// Creates a result of the name_get Odoo XML RPC method call.
        /// </summary>
        /// <param name="recordIdentifier">The identifier of the record.</param>
        /// <param name="name">The name of the record.</param>
        public OdooNameGetResult(int recordIdentifier, string name)
        {
            DataValidator.ValidateIntegerNotNegative(recordIdentifier, nameof(recordIdentifier), nameof(OdooNameGetResult));
            DataValidator.ValidateStringNotEmptyOrNull(name, nameof(recordIdentifier), nameof(OdooNameGetResult));
            _recordIdentifier = recordIdentifier;
            _name = name;
        }

        #endregion //Constructors

        #region Fields

        private int _recordIdentifier;
        private string _name;

        #endregion //Fields

        #region Properties

        /// <summary>
        /// The identifier of the record.
        /// </summary>
        public int RecordIdentifier
        {
            get { return _recordIdentifier; }
        }

        /// <summary>
        /// The name of the record.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        #endregion //Properties
    }
}
