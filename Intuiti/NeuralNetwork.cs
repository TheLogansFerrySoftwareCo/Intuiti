﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NeuralNetwork.cs" company="The Logans Ferry Software Co.">
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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A basic neural network capable of computing sets of output values from sets of input values.
    /// </summary>
    /// <remarks>
    /// A neural network is a collection of neural nodes, meaining that the network can be simple and only contain neurons,
    /// or it can be complex and contain one or more sub-networks.
    /// </remarks>
    public class NeuralNetwork : INeuralNetwork
    {
        /// <summary>
        /// The networks ID number (for logging purposes).
        /// </summary>
        protected readonly long Id;
 
        /// <summary>
        /// The name of the class (for logging purposes).
        /// </summary>
        private const string ClassName = "NeuralNetwork";

        /// <summary>
        /// The next ID number that will be assigned to an object of this class.
        /// </summary>
        private static long nextId;

        /// <summary>
        /// A listing of the network's inbound connections.
        /// </summary>
        private readonly List<INeuralConnection> inboundConnections;

        /// <summary>
        /// A listing of the network's outbound connections.
        /// </summary>
        private readonly List<INeuralConnection> outboundConnections;

        /// <summary>
        /// A listing of the network's input nodes (Input Layer).
        /// </summary>
        private readonly List<INeuralNode> inputNodes;

        /// <summary>
        /// A listing of the network's output nodes (Output Layer).
        /// </summary>
        private readonly List<INeuralNode> outputNodes;

        /// <summary>
        /// The network's name (for logging purposes).
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuralNetwork"/> class.
        /// </summary>
        public NeuralNetwork()
        {
            // Initialize the class name that will appear in log files.
            this.Id = nextId++;
            this.name = ClassName + "_" + this.Id;
            const string MethodName = "ctor";
            Logger.TraceIn(this.name, MethodName);

            this.inboundConnections = new List<INeuralConnection>();
            this.outboundConnections = new List<INeuralConnection>();
            this.inputNodes = new List<INeuralNode>();
            this.outputNodes = new List<INeuralNode>();

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Gets the number of inputs expected by the network's input layer.
        /// </summary>
        /// <value>
        /// The number of expected inputs.
        /// </value>
        public int InputSize
        {
            get
            {
                return this.inputNodes.Sum(node => node.InputSize);
            }
        }

        /// <summary>
        /// Gets the number of ouputs published by the network's output layer.
        /// </summary>
        /// <value>
        /// The size of the output array.
        /// </value>
        public int OutputSize
        {
            get
            {
                return this.outputNodes.Sum(node => node.OutputSize);
            }
        }

        /// <summary>
        /// Gets the network's last-calculated output values from the output layer.
        /// </summary>
        public double[] CachedOutputs { get; private set; }

        /// <summary>
        /// Gets the node's name (for logging purposes).
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the node's inbound connections.
        /// </summary>
        public IList<INeuralConnection> InboundConnections
        {
            get
            {
                return this.inboundConnections;
            }
        }

        /// <summary>
        /// Gets the node's outbound connections.
        /// </summary>
        public IList<INeuralConnection> OutboundConnections
        {
            get
            {
                return this.outboundConnections;
            }
        }

        /// <summary>
        /// Gets the network's input nodes.
        /// </summary>
        public IList<INeuralNode> InputNodes
        {
            get
            {
                return this.inputNodes;
            }
        }

        /// <summary>
        /// Gets the network's output nodes.
        /// </summary>
        public IList<INeuralNode> OutputNodes
        {
            get
            {
                return this.outputNodes;
            }
        }

        /// <summary>
        /// Adds an inbound connection from another node (either another network or a neuron) in the outer network.
        /// </summary>
        /// <param name="inboundConnection">The inbound connection to add.</param>
        public void AddInboundConnection(INeuralConnection inboundConnection)
        {
            const string MethodName = "AddInboundConnection";
            Logger.TraceIn(this.name, MethodName);

            if (inboundConnection == null)
            {
                throw new ArgumentNullException("inboundConnection");
            }

            if (!this.inboundConnections.Contains(inboundConnection))
            {
                this.inboundConnections.Add(inboundConnection);
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Adds an outbound connection to another node (either another network or a neuron) in the outer network.
        /// </summary>
        /// <param name="outboundConnection">The outbound connection to add.</param>
        public void AddOutboundConnection(INeuralConnection outboundConnection)
        {
            const string MethodName = "AddOutboundConnection";
            Logger.TraceIn(this.name, MethodName);

            if (outboundConnection == null)
            {
                throw new ArgumentNullException("outboundConnection");
            }

            if (!this.outboundConnections.Contains(outboundConnection))
            {
                this.outboundConnections.Add(outboundConnection);
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Fires the network (as a node) with the provided input.
        /// </summary>
        /// <param name="input">The input signal.</param>
        /// <remarks>
        /// This method will be invoked when this network is actually a node (subnetwork) of
        /// another network.  Once all of the network's inbound connections have fired, it will
        /// aggregate the signals and use them as inputs for its own computations.  Then it will
        /// read the resulting outputs from its output layer and fire its outbound connections,
        /// thus propagating the computations of the higher-level network.
        /// </remarks>
        public void Fire(double input)
        {
            const string MethodName = "Fire(double)";
            Logger.TraceIn(this.name, MethodName);
            
            // If all inbound connections are reporting a signal, then this network can perform its
            // own computations.
            if (this.inboundConnections.All(connection => connection.IsFired))
            {
                var inputs = this.GetInputSignals();
                var digitizedInputs = this.DigitizeValues(inputs);  // Digitize the input signals for clarity.
                var aggregatedInputs = this.AllocateValues(digitizedInputs, this.InputSize);
                this.ComputeOutputs(aggregatedInputs);

                // Propagate the computations of the higher-level network.
                this.FireOutputs();
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Fires the network (as a node) with the provided inputs.
        /// </summary>
        /// <param name="inputs">The input signals.</param>
        /// <remarks>
        /// <para>
        /// This method will be invoked when this network is actually a node (subnetwork) of
        /// another network.  Note that this method differs from the other overload of Fire()
        /// in that it receives an array of values instead of a single value.  That is because
        /// this method is intended to be invoked directly by the higher network, rather than
        /// by an inbound connection. (For example, when this network is an input node of the higher
        /// network.)
        /// </para>
        /// <para>
        /// This method operates in a similar manner as its counter-part, with the exception that it
        /// receives all inputs at once and so does not need to wait for all inputs to be ready.  Therefore,
        /// it will immediately process the provided inputs by allocating them into the correct number of
        /// values to match its input nodes, using the allocated inputs to calculate outputs, and propagating
        /// the calculated outputs to the outbound connections.
        /// </para>
        /// </remarks>
        public void Fire(double[] inputs)
        {
            const string MethodName = "Fire(double[])";
            Logger.TraceIn(this.name, MethodName);

            var allocatedInputs = this.AllocateValues(inputs, this.InputSize);
            var digitizedInputs = this.DigitizeValues(allocatedInputs);
            this.ComputeOutputs(digitizedInputs);
            this.FireOutputs();

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Computes output values from the provided input values.
        /// </summary>
        /// <param name="inputs">The input values.</param>
        /// <returns>
        /// The computed output values.
        /// </returns>
        public double[] ComputeOutputs(double[] inputs)
        {
            const string MethodName = "ComputeOutputs";
            Logger.TraceIn(this.name, MethodName);

            if (inputs == null)
            {
                throw new ArgumentNullException("inputs");
            }

            if (inputs.Length != this.InputSize)
            {
                throw new InvalidOperationException("The number of inputs does not match the network's Input Size.  Num Inputs: " + inputs.Length + "; Expected: " + this.InputSize);
            }

            Logger.Debug(this.name, MethodName, "inputs", inputs);

            //// Sequentially assign the inputs to the input layer based on each input node's expected number of input values.
            ////
            //// (This logic is intended to handle an input layer where each input node requires a different number of input values.
            //// For example, some nodes may be neurons requiring one input value each, and some nodes may be sub-networks requiring
            //// multiple input values each.)

            var nodeIndex = 0;
            var inputArrayCursor = 0;
            
            while (inputArrayCursor < inputs.Length)
            {
                // Chunk off the next group of input values.
                var nodeInputSize = this.inputNodes[nodeIndex].InputSize;
                var nodeInputs = new double[nodeInputSize];
                Array.Copy(inputs, inputArrayCursor, nodeInputs, 0, nodeInputSize);
                Logger.Debug(this.name, MethodName, "nodeInputs", nodeInputs);
                
                // Fire each input with its designated input values.
                this.inputNodes[nodeIndex].Fire(nodeInputs);

                // Loop control.
                nodeIndex++;
                inputArrayCursor += nodeInputSize;
            }

            //// Now retrieve the output values from the output layer.
            ////
            //// (This logic is similar to the previous logic in that it is intended to handle an output layer where each output node 
            //// publishes a different number of output values.)
            
            this.CachedOutputs = new double[this.OutputSize];
            var outputArrayCursor = 0;
            nodeIndex = 0;

            while (outputArrayCursor < this.CachedOutputs.Length)
            {
                Logger.Debug(this.name, MethodName, "nodeOutputs", this.outputNodes[nodeIndex].CachedOutputs);
                Array.Copy(this.outputNodes[nodeIndex].CachedOutputs, 0, this.CachedOutputs, outputArrayCursor, this.outputNodes[nodeIndex].CachedOutputs.Length);

                outputArrayCursor += this.outputNodes[nodeIndex].CachedOutputs.Length;
                nodeIndex++;
            }

            Logger.Debug(this.name, MethodName, "CachedOutputs", this.CachedOutputs);
            Logger.TraceOut(this.name, MethodName);

            return this.CachedOutputs;
        }

        /// <summary>
        /// Adds a new input node to the network.
        /// </summary>
        /// <param name="node">The node.</param>
        public void AddInputNode(INeuralNode node)
        {
            const string MethodName = "AddInputNode";
            Logger.TraceIn(this.name, MethodName);

            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (!this.inputNodes.Contains(node))
            {
                this.inputNodes.Add(node);
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Adds a new output node to the network.
        /// </summary>
        /// <param name="node">The node.</param>
        public void AddOutputNode(INeuralNode node)
        {
            const string MethodName = "AddOutputNode";
            Logger.TraceIn(this.name, MethodName);

            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (!this.outputNodes.Contains(node))
            {
                this.outputNodes.Add(node);
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Converts the provided array into a new array of the specified size by either aggregating or distibuting values as appropriate.
        /// </summary>
        /// <param name="values">The values to allocate.</param>
        /// <param name="numAllocations">The number of allocations.</param>
        /// <returns>
        /// A new array of the specified size.
        /// </returns>
        protected double[] AllocateValues(IList<double> values, int numAllocations)
        {
            if (values.Count >= numAllocations)
            {
                return this.AggregateValues(values, numAllocations);
            }

            return this.DistributeValues(values, numAllocations);
        }

        /// <summary>
        /// Digitizes the values by converting all positive number to 1.0 and all other numbers to -1.0.
        /// </summary>
        /// <param name="values">The values to digitize.</param>
        /// <returns>
        /// The list of digitized values.
        /// </returns>
        protected double[] DigitizeValues(double[] values)
        {
            var digitizedValues = new double[values.Length];

            for (var index = 0; index < values.Length; index++)
            {
                digitizedValues[index] = values[index] > 0 ? 1.0d : -1.0d;
            }

            return digitizedValues;
        }

        /// <summary>
        /// Aggregates the provided values into the specified number of aggregations.
        /// </summary>
        /// <param name="values">The values to aggregate.</param>
        /// <param name="numAggregations">The number aggregations to create.</param>
        /// <returns>An array of aggregated values.</returns>
        /// <remarks>
        /// <para>
        /// This method is used to consolidate an array of values into a smaller array of values
        /// by combining elements from the larger array together in order to reduce the number of elements. This is
        /// useful for combining values reported from a large number of outer connections into a smaller number of
        /// receiving nodes.
        /// </para>
        /// <para>
        /// For example, suppose an array of 9 elements needed to be aggregated into an array of 3 elements. It might look like:
        /// <code>
        /// original = { 11, 22, 33, 44, 55, 66, 77, 88, 99 }
        /// new = { 11 + 22 + 33, 44 + 55 + 66, 77 + 88 + 99 } = { 66, 165, 264 }
        /// </code>
        ///  </para>
        /// </remarks>
        private double[] AggregateValues(IList<double> values, int numAggregations)
        {
            const string MethodName = "AggregateValues";
            Logger.TraceIn(this.name, MethodName);

            var aggregatedValues = new double[numAggregations];

            var valueIndex = 0;

            var ratio = values.Count / numAggregations;
            var excess = values.Count % numAggregations;

            for (var aggregatedIndex = 0; aggregatedIndex < numAggregations; aggregatedIndex++)
            {
                var sum = 0.0d;
                for (var counter = 0; counter < ratio; counter++)
                {
                    sum += values[valueIndex];
                    valueIndex++;
                }

                if (aggregatedIndex < excess)
                {
                    sum += values[valueIndex];
                    valueIndex++;
                }

                aggregatedValues[aggregatedIndex] = sum;
            }

            Logger.TraceOut(this.name, MethodName);

            return aggregatedValues;
        }

        /// <summary>
        /// Distributes the provided list of values across the specified number of fields.
        /// </summary>
        /// <param name="values">The values to distribute.</param>
        /// <param name="numDistributions">The number of distributions.</param>
        /// <returns>
        /// An array of distributed values.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is used to expand an array of values into a larger array of values
        /// by copying and repeating elements from the smaller array in such a way that increases the number of elements. This is
        /// useful for distributing values reported from a smaller number of outer connections into a larger number of
        /// receiving nodes.
        /// </para>
        /// <para>
        /// For example, suppose an array of 3 elements needs to be distributed across an array of 5 elements. It might look like:
        /// <code>
        /// original = { 11, 22, 33 }
        /// new = { 11, 11, 22, 22, 33 }
        /// </code>
        ///  </para>
        /// </remarks>
        private double[] DistributeValues(IList<double> values, int numDistributions)
        {
            const string MethodName = "DistributeValues";
            Logger.TraceIn(this.name, MethodName);

            var distributedValues = new double[numDistributions];

            var ratio = numDistributions / values.Count;
            var excess = numDistributions % values.Count;

            var distributedValuesIndex = 0;

            for (var valueIndex = 0; valueIndex < values.Count; valueIndex++)
            {
                for (var ratioCount = 0; ratioCount < ratio; ratioCount++)
                {
                    distributedValues[distributedValuesIndex] = values[valueIndex];
                    distributedValuesIndex++;
                }
                
                if (valueIndex < excess)
                {
                    distributedValues[distributedValuesIndex] = values[valueIndex];
                    distributedValuesIndex++;
                }
            }
            
            Logger.TraceOut(this.name, MethodName);

            return distributedValues;
        }

        /// <summary>
        /// Gets the input signals that are being fired from the network's inbound connections.
        /// </summary>
        /// <returns>
        /// The input signals that are being fired from the network's inbound connections.
        /// </returns>
        private double[] GetInputSignals()
        {
            const string MethodName = "GetInputSignals";
            Logger.TraceIn(this.name, MethodName);

            var inputSignals = new double[this.inboundConnections.Count];  // one signal per connection

            for (var inboundIndex = 0; inboundIndex < this.inboundConnections.Count; inboundIndex++)
            {
                // Read each input signal.
                inputSignals[inboundIndex] = this.inboundConnections[inboundIndex].Output;
                Logger.Debug(this.name, MethodName, "inputSignals", inputSignals);

                // Clear the signal.
                this.inboundConnections[inboundIndex].ClearFire();
            }

            Logger.TraceOut(this.name, MethodName);

            return inputSignals;
        }

        /// <summary>
        /// Fires the network's outbound connections with the cached output values.
        /// </summary>
        /// <remarks>
        /// This method will fire each of the network's outbound connections using its cached output values.
        /// Before firing the connections, it will determine the ratio of connections to values, so that it
        /// knows how to distribute the values across the connections.  It will also digitize the cached outputs
        /// before firing them by converting all negative numbers to -1 and all positive numbers to 1.
        /// </remarks>
        private void FireOutputs()
        {
            const string MethodName = "FireOutputs";
            Logger.TraceIn(this.name, MethodName);

            var outputRatio = this.outboundConnections.Count / this.CachedOutputs.Length;
            var ratioExcess = this.outboundConnections.Count % this.CachedOutputs.Length;

            var connectionIndex = 0;
            var digitizedOutputs = this.DigitizeValues(this.CachedOutputs);  // Digitize the output signals for clarity.
            for (var outputIndex = 0; outputIndex < digitizedOutputs.Length; outputIndex++)
            {
                for (var counter = 0; counter < outputRatio; counter++)
                {
                    this.outboundConnections[connectionIndex].Fire(digitizedOutputs[outputIndex]);
                    connectionIndex++;
                }

                if (outputIndex < ratioExcess)
                {
                    this.outboundConnections[connectionIndex].Fire(digitizedOutputs[outputIndex]);
                    connectionIndex++;
                }
            }

            Logger.TraceOut(this.name, MethodName);
        }
    }
}
