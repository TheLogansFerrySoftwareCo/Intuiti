// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Neuron.cs" company="The Logans Ferry Software Co.">
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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An individual neuron within a neural network.
    /// </summary>
    public class Neuron : INeuralNode
    {
        /// <summary>
        /// The neuron's ID number (for logging purposes).
        /// </summary>
        protected readonly long Id;

        /// <summary>
        /// The name of the class (for logging purposes).
        /// </summary>
        private const string ClassName = "Neuron";

        /// <summary>
        /// The random number utility that will be used to generate weights.
        /// </summary>
        /// <remarks>
        /// This is a static member so that it is only seeded once, thus ensuring a different
        /// number for each assignment.  Otherwise, connection objects that are instantiated
        /// too quickly in sequence would receive the same time-based seed value, which would
        /// result in them having the same "random" weight assignment.
        /// </remarks>
        private static readonly Random Randomizer = new Random();

        /// <summary>
        /// The next ID number that will be assigned to an object of this class.
        /// </summary>
        private static long nextId;

        /// <summary>
        /// A listing of the neuron's inbound connections from other nodes.
        /// </summary>
        private readonly List<INeuralConnection> inboundConnections;

        /// <summary>
        /// A listing of the neuron's outbound connections to other nodes.
        /// </summary>
        private readonly List<INeuralConnection> outboundConnections;

        /// <summary>
        /// The neuron's activation function that will be used when firing.
        /// </summary>
        private readonly IActivationFunction activationFunction;

        /// <summary>
        /// The last-calculated output for this neuron.
        /// </summary>
        /// <remarks>
        /// This value is stored in a single-element array in order to satisfy its interface properties. 
        /// </remarks>
        private readonly double[] cachedOutput;

        /// <summary>
        /// The neuron's name (for logging purposes).
        /// </summary>
        private readonly string name;

        /// <summary>
        /// An accumlation of input signal from inbound connections that have fired.
        /// </summary>
        private double pendingInputSignal;

        /// <summary>
        /// The neuron's bias (threshold).
        /// </summary>
        private double bias;

        /// <summary>
        /// Initializes a new instance of the <see cref="Neuron"/> class.
        /// </summary>
        /// <param name="activationFunction">The activation function.</param>
        public Neuron(IActivationFunction activationFunction)
        {
            // Initialize the class name for loggin purposes.
            this.Id = nextId++;
            this.name = ClassName + "_" + this.Id;
            const string MethodName = "Ctor";
            Logger.TraceIn(this.name, MethodName);

            if (activationFunction == null)
            {
                throw new ArgumentNullException("activationFunction");
            }

            this.activationFunction = activationFunction;
            
            this.inboundConnections = new List<INeuralConnection>();
            this.outboundConnections = new List<INeuralConnection>();

            this.InitializeBias();

            // Initialize the single-element array for the cached output value.
            // This value is stored in an array solely to satisfy its interface requirements.
            this.cachedOutput = new double[1];

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Gets the number of inputs expected by the neuron.
        /// </summary>
        /// <value>
        /// The number of expected inputs.
        /// </value>
        public int InputSize
        {
            get
            {
                // Input layer neurons only want one input.  All others want one input
                // per inbound connection.
                return this.IsInputNode ? 1 : this.inboundConnections.Count;
            }
        }

        /// <summary>
        /// Gets the number of ouputs published by the neuron.
        /// </summary>
        /// <value>
        /// The size of the outputs array.
        /// </value>
        public int OutputSize
        {
            get
            {
                // Neurons only ever output one value.
                return 1;
            }
        }

        /// <summary>
        /// Gets the node's last-computed (cached) output.
        /// </summary>
        public double[] CachedOutputs
        {
            get
            {
                return this.cachedOutput;
            }
        }

        /// <summary>
        /// Gets the neuron's name (for logging purposes).
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets or sets the neuron's activation bias (threshold).
        /// </summary>
        /// <value>
        /// The activation bias.
        /// </value>
        public double Bias
        {
            get
            {
                // Input nodes have no bias, so they return zero.
                // All others return a normal value.
                return this.IsInputNode ? 0 : this.bias;
            }

            set
            {
                // Input nodes have no bias, so set them  to zero.
                // Set all others normally.
                this.bias = this.IsInputNode ? 0 : value;
            }
        }

        /// <summary>
        /// Gets the neuron's activation function.
        /// </summary>
        protected IActivationFunction ActivationFunction
        {
            get
            {
                return this.activationFunction;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this node is an input node for its network.
        /// </summary>
        /// <value>
        /// <c>true</c> if this node is an input node; otherwise, <c>false</c>.
        /// </value>
        private bool IsInputNode
        {
            get
            {
                return this.inboundConnections.Count <= 0;
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

            this.inboundConnections.Add(inboundConnection);

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

            this.outboundConnections.Add(outboundConnection);

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Fires the neuron with the provided input.
        /// </summary>
        /// <param name="input">The input signal.</param>
        /// <remarks>
        /// This method will accumulate input signals from all inbound connections.
        /// When the final inbound connection fires, then this neuron will process
        /// the total signal through its activation function and fire its outbound
        /// connections.
        /// </remarks>
        public void Fire(double input)
        {
            const string MethodName = "Fire(double)";
            Logger.TraceIn(this.name, MethodName);

            // Accumulate the input signal.
            this.pendingInputSignal += input;
            Logger.Debug(this.name, MethodName, "pendingInputSignal", this.pendingInputSignal);

            // If all inbound connections have fired, then activate the neuron.
            // (Note: This condition will be true for input nodes with no inbound connections.)
            if (this.inboundConnections.All(connection => connection.IsFired))
            {
                this.Activate();

                // Clear the input signal and inbound connections
                this.pendingInputSignal = 0.0d;
                foreach (var connection in this.inboundConnections)
                {
                    connection.ClearFire();
                }
            }
            else
            {
                Logger.Debug(this.name, MethodName, "Not all inbound connections have fired yet.");
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Fires the neuron with the provided inputs.
        /// </summary>
        /// <param name="inputs">The input signals.</param>
        /// <remarks>
        /// <para>
        /// Note that this method differs from the other overload of Fire() in that it receives an 
        /// array of values instead of a single value.  That is because this method is intended to be invoked 
        /// directly by the higher network, rather than by an inbound connection. (For example, when this 
        /// neuron is an input node of the higher network.)
        /// </para>
        /// <para>
        /// This method operates in a similar manner as its counter-part, with the exception that it
        /// receives all inputs at once and so does not need to wait for all inputs to be ready.  Therefore,
        /// it will immediately sum the inputs into one signal and activate the neuron as normal.
        /// </para>
        /// </remarks>
        public void Fire(double[] inputs)
        {
            const string MethodName = "Fire(double[])";
            Logger.TraceIn(this.name, MethodName);

            if (inputs == null)
            {
                throw new ArgumentNullException("inputs");
            }

            if (inputs.Length <= 0)
            {
                throw new ArgumentException("Input array cannot be zero-length.", "inputs");
            }

            // Sum the inputs and activate the neuron.
            this.pendingInputSignal = inputs.Sum();
            Logger.Debug(this.name, MethodName, "AccumulatedInputs", this.pendingInputSignal);
            this.Activate();

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Activates the neuron by computing an output value and firing outbound connections.
        /// </summary>
        private void Activate()
        {
            const string MethodName = "Activate";
            Logger.TraceIn(this.name, MethodName);

            // Calculate an output value by adding the neuron's bias to it's pending input signal and
            // processing the sum through the activation function.
            var biasedInput = this.pendingInputSignal + this.Bias;
            Logger.Debug(this.name, MethodName, "biasedInput", biasedInput);
            var output = this.activationFunction.Invoke(biasedInput);
            Logger.Debug(this.name, MethodName, "activatedOutput", output);

            // Cache the output.
            this.cachedOutput[0] = output;

            // Fire each of the outbound connections with the output signal.
            foreach (var connection in this.outboundConnections)
            {
                connection.Fire(output);
            }

            Logger.TraceOut(this.name, MethodName);
        }

        /// <summary>
        /// Initializes the bias by randomly assigning a value.
        /// </summary>
        private void InitializeBias()
        {
            const string MethodName = "Activate";
            Logger.TraceIn(this.name, MethodName);

            // Bias values will initialize between -1 and 1.
            const double Max = 1.0d;
            const double Min = -1.0d;

            this.bias = (Randomizer.NextDouble() * (Max - Min)) + Min;

            Logger.TraceOut(this.name, MethodName);
        }
    }
}
