// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HyperbolicTangentTests.cs" company="The Logans Ferry Software Co.">
//   Copyright 2012, The Logans Ferry Software Co. 
// </copyright>
// <license>  
//   This file is part of Intuiti.
//   
//   Intuiti is free software: you can redistribute it and/or modify it under the terms
//   of the GNU General Public License as published by the Free Software Foundation, either
//   version 3 of the License, or (at your option) any later version.
//   
//   Intuiti is distributed in the hope that it will be useful, but WITHOUT ANY
//   WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
//   A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License along with
//   Intuiti. If not, see http://www.gnu.org/licenses/.
// </license>
// --------------------------------------------------------------------------------------------------------------------


namespace LogansFerry.Intuiti.UnitTests
{
    using System;

    using NUnit.Framework;

    /// <summary>
    /// A collection of tests for the HyperbolicTangentFunction.cs class.
    /// </summary>
    [TestFixture]
    public class HyperbolicTangentTests
    {
        /// <summary>
        /// Method:         Invoke
        /// Requirement:    Returns the hyperbolic tangent of provided value.
        /// </summary>
        [Test]
        public void Invoke_ReturnsHyperbolicTangent()
        {
            const double Input1 = -1.234d;
            const double Input2 = 0.0d;
            const double Input3 = 12.34d;

            var expected1 = Math.Tanh(Input1);
            var expected2 = Math.Tanh(Input2);
            var expected3 = Math.Tanh(Input3);

            var function = new HyperbolicTangentFunction();

            var actual1 = function.Invoke(Input1);
            var actual2 = function.Invoke(Input2);
            var actual3 = function.Invoke(Input3);

            Assert.AreEqual(expected1, actual1, 0.0000001);
            Assert.AreEqual(expected2, actual2, 0.0000001);
            Assert.AreEqual(expected3, actual3, 0.0000001);
        }

        /// <summary>
        /// Method:         InvokeDerivative
        /// Requirement:    Returns the proper derivative of the hyperbolic tangent
        /// </summary>
        [Test]
        public void InvokeDerivative_ReturnsHyperbolicDerivative()
        {
            const double Input1 = -1.234d;
            const double Input2 = 0.0d;
            const double Input3 = 12.34d;

            var expected1 = 1 / Math.Pow(Math.Cosh(Input1), 2);
            var expected2 = 1 / Math.Pow(Math.Cosh(Input2), 2);
            var expected3 = 1 / Math.Pow(Math.Cosh(Input3), 2);

            var function = new HyperbolicTangentFunction();

            var actual1 = function.InvokeDerivative(Input1);
            var actual2 = function.InvokeDerivative(Input2);
            var actual3 = function.InvokeDerivative(Input3);

            Assert.AreEqual(expected1, actual1, 0.0000001);
            Assert.AreEqual(expected2, actual2, 0.0000001);
            Assert.AreEqual(expected3, actual3, 0.0000001);
        }
    }
}
