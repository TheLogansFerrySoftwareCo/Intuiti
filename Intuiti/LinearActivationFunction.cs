// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinearActivationFunction.cs" company="The Logans Ferry Software Co.">
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
    /// <summary>
    /// An activation function with a linear output graph.
    /// </summary>
    /// <remarks>
    /// This function is used in situations where inputs should pass through a neuron un-modified,
    /// such as in Input Layer neurons.
    /// </remarks>
    public class LinearActivationFunction : IActivationFunction
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
            return input;
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
            // The linear function has no derivative.
            return 1;
        }
    }
}
