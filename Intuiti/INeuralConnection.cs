// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INeuralConnection.cs" company="The Logans Ferry Software Co.">
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
    /// A weighted connection that exists between two nodes of a neural network.
    /// </summary>
    public interface INeuralConnection
    {
        /// <summary>
        /// Gets the connection's output value.
        /// </summary>
        /// <value>
        /// The output value.
        /// </value>
        double Output { get; }

        /// <summary>
        /// Gets a value indicating whether this connection has fired.
        /// </summary>
        /// <value>
        /// <c>true</c> if this connection fired; otherwise, <c>false</c>.
        /// </value>
        bool IsFired { get; }

        /// <summary>
        /// Fires the connection with the provided input.
        /// </summary>
        /// <param name="input">The input.</param>
        void Fire(double input);

        /// <summary>
        /// Clears the connection's IsFired status.
        /// </summary>
        void ClearFire();
    }
}
