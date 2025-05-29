namespace SoftwareDeveloperCase.Domain.Common
{
    /// <summary>
    /// Base class for value objects that provide value equality semantics.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Determines whether two value objects are equal.
        /// </summary>
        /// <param name="left">The left value object to compare.</param>
        /// <param name="right">The right value object to compare.</param>
        /// <returns>True if the value objects are equal; otherwise, false.</returns>
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (left is null ^ right is null)
            {
                return false;
            }

            return left?.Equals(right!) != false;
        }

        /// <summary>
        /// Determines whether two value objects are not equal.
        /// </summary>
        /// <param name="left">The left value object to compare.</param>
        /// <param name="right">The right value object to compare.</param>
        /// <returns>True if the value objects are not equal; otherwise, false.</returns>
        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !(EqualOperator(left, right));
        }

        /// <summary>
        /// Gets the equality components used to determine value equality.
        /// </summary>
        /// <returns>An enumerable of objects that represent the equality components.</returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        /// <summary>
        /// Determines whether the specified object is equal to the current value object.
        /// </summary>
        /// <param name="obj">The object to compare with the current value object.</param>
        /// <returns>True if the specified object is equal to the current value object; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current value object.</returns>
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }
    }
}
