namespace Odoo.XmlRpcAdapter.Domain
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Server.Toolkit.Data;
    using Odoo.XmlRpcAdapter.Domain.Operators;
    using Odoo.XmlRpcAdapter.Domain.Operators.Mappers;

    #endregion //Using Directives

    /// <summary>
    /// A domain is a list of criteria, each criterion being a triple (either a list or a tuple) of (field_name, operator, value) where:
    /// https://www.odoo.com/documentation/12.0/reference/orm.html#domains
    /// </summary>
    public class OdooDomainExpression : IOdooDomainMember
    {
        #region Constructors

        public OdooDomainExpression(string fieldName, OdooComparisonOperator comparisonOperator, object value)
        {
            FieldName = fieldName;
            ComparisonOperator = comparisonOperator;
            Value = value;
        }

        #endregion //Constructors

        #region Fields

        private string _fieldName;
        private OdooComparisonOperator _comparisonOperatior;
        private object _value;

        #endregion //Fields

        #region Properties

        /// <summary>
        /// field_name (str) a field name of the current model, or a relationship traversal through a Many2one using dot-notation e.g. 'street' or 'partner_id.country'
        /// </summary>
        public string FieldName
        {
            get { return _fieldName; }
            set
            {
                DataValidator.ValidateStringNotEmptyOrNull(value, EntityReader<OdooDomainExpression>.GetPropertyName(p => p.FieldName, false), nameof(OdooDomainExpression));
                _fieldName = value;
            }
        }


        /// <summary>
        /// operator (str)an operator used to compare the field_name with the value.
        /// </summary>
        public OdooComparisonOperator ComparisonOperator
        {
            get { return _comparisonOperatior; }
            set
            {
                DataValidator.ValidateObjectNotNull(value, EntityReader<OdooDomainExpression>.GetPropertyName(p => p.ComparisonOperator, false), nameof(OdooDomainExpression));
                _comparisonOperatior = value;
            }
        }

        /// <summary>
        /// Variable type, must be comparable (through operator) to the named field
        /// </summary>
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion //Properties

        #region Methods

        public object ToOdooObject()
        {
            return new object[] { FieldName, ComparisonOperator.Value, Value };
        }

        #endregion //Methods
    }
}