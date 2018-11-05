namespace Odoo.XmlRpcAdapter.Domain.Operators.Mappers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Odoo.XmlRpcAdapter.Domain.Operators.Keys;

    #endregion //Using Directives

    /// <summary>
    /// Domain filters can be combined using logical operators in prefix form. Python Example:
    /// 
    /// To search for partners named ABC, from belgium or germany, whose language is not english:
    /// 
    /// [('name','=','ABC'),
    /// ('language.code','!=','en_US'),
    /// '|',('country_id.code','=','be'),
    /// ('country_id.code','=','de')]
    /// 
    /// This domain is interpreted as:
    /// 
    /// (name is 'ABC')
    /// AND(language is NOT english)
    /// AND(country is Belgium OR Germany)
    /// 
    /// How to use logical OR operator with XML-RPC ?
    /// https://www.odoo.com/forum/help-1/question/how-to-use-logical-or-operator-with-xml-rpc-25694
    /// </summary>
    public class OdooLogicalOperator : IOdooDomainMember
    {
        #region Constructors

        public OdooLogicalOperator(OdooLogicalOperatorKey key)
        {
            switch (key)
            {
                case OdooLogicalOperatorKey.And:
                    _value = "&";
                    break;
                case OdooLogicalOperatorKey.Or:
                    _value = "|";
                    break;
                case OdooLogicalOperatorKey.Not:
                    _value = "!";
                    break;
                default:
                    throw new Exception($"Invalid {nameof(OdooLogicalOperatorKey)} of {key.ToString()}.");
            }
            _key = key;
        }

        #endregion //Constructors

        #region Fields

        private OdooLogicalOperatorKey _key;
        private string _value;

        #endregion //Fields

        #region Properties

        public OdooLogicalOperatorKey Key
        {
            get { return _key; }
        }

        public string Value
        {
            get { return _value; }
        }

        #endregion //Properties

        #region Factory Properties

        /// <summary>
        /// "&" : Logical AND, default operation to combine criteria following one another. Arity 2 (uses the next 2 criteria or combinations).
        /// </summary>
        public static OdooLogicalOperator And
        {
            get { return new OdooLogicalOperator(OdooLogicalOperatorKey.And); }
        }

        /// <summary>
        /// "|" : logical OR, arity 2.
        /// </summary>
        public static OdooLogicalOperator Or
        {
            get { return new OdooLogicalOperator(OdooLogicalOperatorKey.Or); }
        }

        /// <summary>
        /// "!" : logical NOT, arity 1.
        /// </summary>
        public static OdooLogicalOperator Not
        {
            get { return new OdooLogicalOperator(OdooLogicalOperatorKey.Not); }
        }

        #endregion //Factory Properties

        #region Methods

        public override string ToString()
        {
            return Value;
        }

        public object ToOdooObject()
        {
            return Value;
        }

        #endregion //Methods
    }
}