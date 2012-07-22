// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NeuralConnection.cs" company="The Logans Ferry Software Co.">
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
    using System;

    /// <summary>
    /// A connection between two nodes of a neural network.
    /// </summary>
    /// <remarks>
    /// Connections are used to fire a signal from an output of one node to the input of another.
    /// Each connection has a weight value that is used to modify the strength of the signal. Nodes
    /// that might be connected with a neural connection can be neurons or networks.
    /// </remarks>
    public class NeuralConnection : INeuralConnection
    {
        /// <summary>
        /// The random number utility that will be used to generate connection weights.
        /// </summary>
        /// <remarks>
        /// This is a static member so that it is only seeded once, thus ensuring a different
        /// number for each assignment.  Otherwise, connection objects that are instantiated
        /// too quickly in sequence would receive the same time-based seed value, which would
        /// result in them having the same "random" weight assignment.
        /// </remarks>
        private static readonly Random Randomizer = new Random();

        /// <summary>
        /// The source node for this connection.  This is the node that will fire a signal over the connection.
        /// </summary>
        private readonly INeuralNode sourceNode;

        /// <summary>
        /// The target node for this connection.  This is the node that will receive a signal from the source node.
        /// </summary>
        private readonly INeuralNode targetNode;

        /// <summary>
        /// The connection's name (for logging purposes).
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuralConnection"/> class.
        /// </summary>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="targetNode">The target node.</param>
        public NeuralConnection(INeuralNode sourceNode, INeuralNode targetNode)
        {
            // Initialize the class name that will appear in log files.
            this.name = sourceNode.Name + "->" + targetNode.Name;
            const string MethodName = "ctor";
            Logger.TraceIn(this.name, MethodName);

            this.Initialize();

            this.sourceNode = sourceNode;
            this.targetNode = targetNode;
            
            // Ensure that the end-point nodes have a reference to this connection object.
            this.sourceNode.AddOutboundConnection(this);
            this.targetNode.AddInboundConnection(this);

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Gets the connection's output value.
        /// </summary>
        /// <value>
        /// The output value.
        /// </value>
        /// <remarks>
        /// The output value is equal to the input from the source node multiplied by the connection's weight:
        /// <code>
        /// output = cachedInput * weight
        /// </code>
        /// </remarks>
        public double Output { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this connection has fired.
        /// </summary>
        /// <value>
        /// <c>true</c> if this connection fired; otherwise, <c>false</c>.
        /// </value>
        public bool IsFired { get; private set; }

        /// <summary>
        /// Gets the source node of the connection.
        /// </summary>
        /// <value>
        /// The source node.
        /// </value>
        protected INeuralNode SourceNode
        {
            get
            {
                return this.sourceNode;
            }
        }

        /// <summary>
        /// Gets the target node of the connection.
        /// </summary>
        /// <value>
        /// The target node.
        /// </value>
        protected INeuralNode TargetNode
        {
            get
            {
                return this.targetNode;
            }
        }

        /// <summary>
        /// Gets or sets the connection's weight value.
        /// </summary>
        /// <value>
        /// The weight value.
        /// </value>
        protected double Weight { get; set; }

        /// <summary>
        /// Gets the cached input, which is the value that was provided by the source node the last time that it fired.
        /// </summary>
        protected double CachedInput { get; private set; }

        /// <summary>
        /// Fires the connection with the provided input.
        /// </summary>
        /// <param name="input">The input from the source node.</param>
        public void Fire(double input)
        {
            const string MethodName = "Fire";
            Logger.TraceIn(this.name, MethodName);

            if (this.IsFired)
            {
                throw new InvalidOperationException("This connection has already fired and must be cleared before it can fire again.");
            }

            // Cache the input and compute the output.
            this.CachedInput = input;
            this.Output = input * this.Weight;
            Logger.Debug(this.name, MethodName, "Weight", this.Weight);
            Logger.Debug(this.name, MethodName, "Output", this.Output);

            // Fire the modified signal to the target node.
            this.IsFired = true;
            this.TargetNode.Fire(this.Output);

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Clears the connection's IsFired status.
        /// </summary>
        public void ClearFire()
        {
            const string MethodName = "Fire";
            Logger.TraceIn(this.name, MethodName);

            this.IsFired = false;

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Initializes the connection by randomly assigning a weight and clearing all cached values.
        /// </summary>
        private void Initialize()
        {
            const string MethodName = "Initialize";
            Logger.TraceIn(this.name, MethodName);

            // Weights will be values between -1 and 1.
            const double Max = 1.0d;
            const double Min = -1.0d;

            this.Weight = (Randomizer.NextDouble() * (Max - Min)) + Min;
            Logger.Debug(this.name, MethodName, "Weight", this.Weight);

            Logger.TraceOut(this.name, MethodName);
        }
    }
}
