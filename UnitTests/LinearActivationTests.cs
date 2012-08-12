// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinearActivationTests.cs" company="The Logans Ferry Software Co.">
//   Copyright 2012, The Logans Ferry Software Co. 
// </copyright>
// <license>  
//   This file is part of neuro.net.
//   
//   neuro.net is free software: you can redistribute it and/or modify it under the terms
//   of the GNU General Public License as published by the Free Software Foundation, either
//   version 3 of the License, or (at your option) any later version.
//   
//   neuro.net is distributed in the hope that it will be useful, but WITHOUT ANY
//   WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
//   A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License along with
//   neuro.net. If not, see http://www.gnu.org/licenses/.
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace LogansFerry.NeuroDotNet.UnitTests
{
    using NUnit.Framework;
    
    /// <summary>
    /// A collection of tests for the LinearActivationFunction.cs class.
    /// </summary>
    [TestFixture]
    public class LinearActivationTests
    {
        /// <summary>
        /// Method:         Invoke
        /// Requirement:    Invoke returns the inputted value, unmodified.
        /// </summary>
        [Test]
        public void Invoke_DoesNotModifyInput()
        {
            const double Input1 = -1.234d;
            const double Input2 = 0.0d;
            const double Input3 = 1.234d;

            var function = new LinearActivationFunction();

            var actual1 = function.Invoke(Input1);
            var actual2 = function.Invoke(Input2);
            var actual3 = function.Invoke(Input3);

            Assert.AreEqual(Input1, actual1);
            Assert.AreEqual(Input2, actual2);
            Assert.AreEqual(Input3, actual3);
        }

        /// <summary>
        /// Method:         InvokeDerivative
        /// Requirement:    Always returns 1.0
        /// </summary>
        [Test]
        public void InvokeDerivative_AlwaysReturns1()
        {
            const double Input1 = -1.234d;
            const double Input2 = 0.0d;
            const double Input3 = 1.234d;

            var function = new LinearActivationFunction();

            var actual1 = function.InvokeDerivative(Input1);
            var actual2 = function.InvokeDerivative(Input2);
            var actual3 = function.InvokeDerivative(Input3);

            Assert.AreEqual(1.0d, actual1);
            Assert.AreEqual(1.0d, actual2);
            Assert.AreEqual(1.0d, actual3);
        }
    }
}
