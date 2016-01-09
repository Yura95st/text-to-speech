﻿namespace TextToSpeech.Utils
{
    using System;

    /// <summary>
    ///     A static helper class that includes various parameter checking routines.
    /// </summary>
    public static class Guard
    {
        #region Public Methods

        /// <summary>
        ///     Throws <see cref="ArgumentOutOfRangeException" /> if the given argument is negative.
        /// </summary>
        /// <param name="argumentIntValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void IntNonNegative(int argumentIntValue, string argumentName)
        {
            if (argumentIntValue < 0)
            {
                throw new ArgumentOutOfRangeException("The argument must be non-negative.", argumentName);
            }
        }

        /// <summary>
        ///     Throws <see cref="ArgumentOutOfRangeException" /> if the given argument is less or equal to zero.
        /// </summary>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void GreaterThanZero(double argumentValue, string argumentName)
        {
            if (argumentValue <= 0)
            {
                throw new ArgumentOutOfRangeException("The argument must be greater than 0.", argumentName);
            }
        }

        /// <summary>
        ///     Throws <see cref="ArgumentNullException" /> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void NotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName, "The argument can't be null.");
            }
        }

        #endregion
    }
}