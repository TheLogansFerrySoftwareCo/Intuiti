// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RmsCalculatorTests.cs" company="The Logans Ferry Software Co.">
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
    /// Unit tests for the RmsCalculator.cs class.
    /// </summary>
    [TestFixture]
    public class RmsCalculatorTests
    {
        /// <summary>
        /// Method:         All
        /// Requirement:    Keeps a running total of squared errors.  Calculates by returning Root-Mean of sum.  Clears running total only on Reset command.
        /// </summary>
        [Test]
        public void OverallTest()
        {
            // Setup
            const double Epsilon = 0.000000000001;
            const double Error1 = 1.234;
            const double Error2 = -1.234;
            const double Error3 = 9.756;

            const double Expected1 = Error1;
            double expected2 = Math.Pow((Math.Pow(Error1, 2) + Math.Pow(Error2, 2) + Math.Pow(Error3, 2)) / 3, 0.5);

            var target = new RmsCalculator();

            // Execute
            target.AddToErrorCalc(new[] { Error1 });
            var actual1 = target.Calculate();

            target.AddToErrorCalc(new[] { Error2, Error3 });
            var actual2 = target.Calculate();

            target.Reset();
            target.AddToErrorCalc(new[] { Error3 });
            var actual3 = target.Calculate();
            
            // Verify
            Assert.AreEqual(Expected1, actual1, Epsilon);
            Assert.AreEqual(expected2, actual2, Epsilon);
            Assert.AreEqual(Error3, actual3);
        }

        /// <summary>
        /// Method:         Calculate
        /// Requirement:    Requires at least one input to have been provided (to avoid a set size of zero).
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Calculate_ProtectsAgainstDivideByZero()
        {
            var target = new RmsCalculator();
            target.Calculate();
        }
    }
}
