namespace Odoo.XmlRpcAdapter.Domain.Operators.Keys
{
    /// <summary>
    /// An operator used to compare the field_name with the value. Valid operators are:
    /// </summary>
    public enum OdooComparisonOperatorKey
    {
        /// <summary>
        /// "=" : Equals
        /// </summary>
        Equals,
        /// <summary>
        /// "!=" : No equals to
        /// </summary>
        NotEqualsTo,
        /// <summary>
        /// ">" : Greater than
        /// </summary>
        GreaterThan,
        /// <summary>
        /// ">=" : Greater than or equal to
        /// </summary>
        GreaterThanOrEqualTo,
        /// <summary>
        /// "<" : Less than
        /// </summary>
        LessThan,
        /// <summary>
        /// "<=" : Less than or equals to
        /// </summary>
        LessThanOrEqualsTo,
        /// <summary>
        /// "=?" : Unset or equals to (returns true if value is either None or False, otherwise behaves like =)
        /// </summary>
        UnsetOrEqualsTo,
        /// <summary>
        /// "=like" : Matches field_name against the value pattern. An underscore _ in the pattern stands for (matches) any single character; a percent sign % matches any string of zero or more characters.
        /// </summary>
        EqualsLike,
        /// <summary>
        /// "like" : Matches field_name against the %value% pattern. Similar to =like but wraps value with ‘%’ before matching
        /// </summary>
        Like,
        /// <summary>
        /// "not like" : Doesn’t match against the %value% pattern
        /// </summary>
        NotLike,
        /// <summary>
        /// "ilike" : Case insensitive like
        /// </summary>
        ILike,
        /// <summary>
        /// "not ilike" : Case insensitive not like
        /// </summary>
        NotILike,
        /// <summary>
        /// "=ilike" : Case insensitive =like
        /// </summary>
        EqualsILike,
        /// <summary>
        /// "in" : Is equal to any of the items from value, value should be a list of items
        /// </summary>
        In,
        /// <summary>
        /// "not in" : Is unequal to all of the items from value
        /// </summary>
        NotIn,
        /// <summary>
        /// "child_of" : Is a child (descendant) of a value record. Takes the semantics of the model into account (i.e following the relationship field named by _parent_name).
        /// </summary>
        ChildOf,
    }
}
