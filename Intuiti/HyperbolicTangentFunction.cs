// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HyperbolicTangentFunction.cs" company="The Logans Ferry Software Co.">
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

namespace LogansFerry.Intuiti
{
    using System;

    /// <summary>
    /// A sigmoid-shaped activation function that used the Hyperbolic Tangent Function to produce output values
    /// between -1.0 and 1.0.  The Hyperbolic Tangent Function and its derivative look as follows:
    /// <para>
    /// Function:
    ///  <code>
    ///                     2x
    ///                    e   - 1
    /// output = tanh(x) = --------
    ///                     2x
    ///                    e   + 1
    /// </code>
    /// </para>
    /// <para>
    /// Derivative:
    /// <code>
    ///           d            2
    /// output = --- = 1 - tanh  x
    ///          d(x)
    /// </code>
    /// </para>
    /// </summary>
    public class HyperbolicTangentFunction : IActivationFunction
    {
        /// <summary>
        /// Invokes the activation function on the provided input.
        /// </summary>
        /// <param name="input">The input to the function.</param>
        /// <returns>
        /// The output value from the function.
        /// </returns>
        public double Invoke(double input)
        {
            return (Math.Exp(2.0d * input) - 1.0d) / (Math.Exp(2.0d * input) + 1.0d);
        }

        /// <summary>
        /// Invokes the derivative function of the activation function on the provided input.
        /// </summary>
        /// <param name="input">The input to the derivative function.</param>
        /// <returns>
        /// The output value from the function.
        /// </returns>
        public double InvokeDerivative(double input)
        {
            var tanh = this.Invoke(input);
            return 1 - Math.Pow(tanh, 2.0d);
        }
    }
}
