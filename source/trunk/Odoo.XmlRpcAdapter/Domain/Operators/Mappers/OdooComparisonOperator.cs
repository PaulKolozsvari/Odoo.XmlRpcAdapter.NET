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
    /// An operator used to compare the field_name with the value. Valid operators are:
    /// </summary>
    public class OdooComparisonOperator : IOdooDomainMember
    {
        #region Constructors

        public OdooComparisonOperator(OdooComparisonOperatorKey key)
        {
            switch (key)
            {
                case OdooComparisonOperatorKey.Equals:
                    _value = "=";
                    break;
                case OdooComparisonOperatorKey.NotEqualsTo:
                    _value = "!=";
                    break;
                case OdooComparisonOperatorKey.GreaterThan:
                    _value = ">";
                    break;
                case OdooComparisonOperatorKey.GreaterThanOrEqualTo:
                    _value = ">=";
                    break;
                case OdooComparisonOperatorKey.LessThan:
                    _value = "<";
                    break;
                case OdooComparisonOperatorKey.LessThanOrEqualsTo:
                    _value = "<=";
                    break;
                case OdooComparisonOperatorKey.UnsetOrEqualsTo:
                    _value = "=?";
                    break;
                case OdooComparisonOperatorKey.EqualsLike:
                    _value = "=like";
                    break;
                case OdooComparisonOperatorKey.Like:
                    _value = "like";
                    break;
                case OdooComparisonOperatorKey.NotLike:
                    _value = "not like";
                    break;
                case OdooComparisonOperatorKey.ILike:
                    _value = "ilike";
                    break;
                case OdooComparisonOperatorKey.NotILike:
                    _value = "not ilike";
                    break;
                case OdooComparisonOperatorKey.EqualsILike:
                    _value = "=ilike";
                    break;
                case OdooComparisonOperatorKey.In:
                    _value = "in";
                    break;
                case OdooComparisonOperatorKey.NotIn:
                    _value = "not in";
                    break;
                case OdooComparisonOperatorKey.ChildOf:
                    _value = "child of";
                    break;
                default:
                    throw new Exception($"Invalid {nameof(OdooComparisonOperatorKey)} of {key.ToString()}.");
            }
            _key = key;
        }

        #endregion //Constructors

        #region Fields

        private OdooComparisonOperatorKey _key;
        private string _value;

        #endregion //Fields

        #region Properties

        public OdooComparisonOperatorKey Key
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
        /// "=" : Equals
        /// </summary>
        public new static OdooComparisonOperator Equals
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.Equals); }
        }

        /// <summary>
        /// "!=" : Not equals to
        /// </summary>
        public static OdooComparisonOperator NotEqualsTo
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.NotEqualsTo); }
        }

        /// <summary>
        /// ">" : Greater than
        /// </summary>
        public static OdooComparisonOperator GreaterThan
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.GreaterThan); }
        }

        /// <summary>
        /// ">=" : Greater than or equal to
        /// </summary>
        public static OdooComparisonOperator GreaterThanOrEqualTo
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.GreaterThanOrEqualTo); }
        }

        /// <summary>
        /// "<" : Less than
        /// </summary>
        public static OdooComparisonOperator LessThan
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.LessThan); }
        }

        /// <summary>
        /// "<=" : Less than or equals to
        /// </summary>
        public static OdooComparisonOperator LessThanOrEqualsTo
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.LessThanOrEqualsTo); }
        }

        /// <summary>
        /// "=?" : Unset or equals to (returns true if value is either None or False, otherwise behaves like =)
        /// </summary>
        public static OdooComparisonOperator UnsetOrEqualsTo
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.UnsetOrEqualsTo); }
        }

        /// <summary>
        /// "=like" : Matches field_name against the value pattern. An underscore _ in the pattern stands for (matches) any single character; a percent sign % matches any string of zero or more characters.
        /// </summary>
        public static OdooComparisonOperator EqualsLike
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.EqualsLike); }
        }

        /// <summary>
        /// "like" : Matches field_name against the %value% pattern. Similar to =like but wraps value with ‘%’ before matching
        /// </summary>
        public static OdooComparisonOperator Like
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.Like); }
        }

        /// <summary>
        /// "not like" : Doesn’t match against the %value% pattern
        /// </summary>
        public static OdooComparisonOperator NotLike
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.NotLike); }
        }

        /// <summary>
        /// "ilike" : Case insensitive like
        /// </summary>
        public static OdooComparisonOperator ILike
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.ILike); }
        }

        /// <summary>
        /// "not ilike" : Case insensitive not like
        /// </summary>
        public static OdooComparisonOperator NotILike
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.NotILike); }
        }

        /// <summary>
        /// "=ilike" : Case insensitive =like
        /// </summary>
        public static OdooComparisonOperator EqualsILike
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.EqualsILike); }
        }

        /// <summary>
        /// "in" : Is equal to any of the items from value, value should be a list of items
        /// </summary>
        public static OdooComparisonOperator In
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.In); }
        }

        /// <summary>
        /// "not in" : Is unequal to all of the items from value
        /// </summary>
        public static OdooComparisonOperator NotIn
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.NotIn); }
        }

        /// <summary>
        /// "child_of" : Is a child (descendant) of a value record. Takes the semantics of the model into account (i.e following the relationship field named by _parent_name).
        /// </summary>
        public static OdooComparisonOperator ChildOf
        {
            get { return new OdooComparisonOperator(OdooComparisonOperatorKey.ChildOf); }
        }

        #endregion //Factory Properties

        #region Methods

        public override string ToString()
        {
            return _value;
        }

        public object ToOdooObject()
        {
            return new object[] { Value };
        }

        #endregion //Methods
    }
}
