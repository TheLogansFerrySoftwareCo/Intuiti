// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActivationFunction.cs" company="The Logans Ferry Software Co.">
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

namespace LogansFerry.NeuroDotNet
{
    /// <summary>
    /// Provides a function for determining the activation value of a neuron, 
    /// as well as the derivative of the function for use in backpropagation.
    /// </summary>
    public interface IActivationFunction
    {
        /// <summary>
        /// Invokes the activation function on the provided input.
        /// </summary>
        /// <param name="input">The input to the function.</param>
        /// <returns>
        /// The output value from the function.
        /// </returns>
        double Invoke(double input);

        /// <summary>
        /// Invokes the derivative function of the activation function on the provided input.
        /// </summary>
        /// <param name="input">The input to the derivative function.</param>
        /// <returns>
        /// The output value from the derivative function.
        /// </returns>
        double InvokeDerivative(double input);
    }
}
