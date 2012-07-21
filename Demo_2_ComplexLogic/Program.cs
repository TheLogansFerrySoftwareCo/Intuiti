// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="The Logans Ferry Software Co.">
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

namespace Demo_2_ComplexLogic
{
    using System;

    using log4net.Config;

    using LogansFerry.NeuroDotNet;

    /// <summary>
    /// This application demonstrates using neuro.net to solve a compound logic problem 
    /// by using nested neural networks.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main method.
        /// </summary>
        public static void Main()
        {
            // Configure log4net
            XmlConfigurator.Configure();

            // Compound truth table inputs:
            //  -1.0 = false
            //   1.0 = true
            var inputs = new[]
                {
                    new[] { -1.0, -1.0, -1.0, -1.0 },   // F  F  F  F
                    new[] { -1.0, -1.0, -1.0, 1.0 },    // F  F  F  T
                    new[] { -1.0, -1.0, 1.0, -1.0 },    // F  F  T  F
                    new[] { -1.0, 1.0, -1.0, -1.0 },    // F  T  F  F
                    new[] { 1.0, -1.0, -1.0, -1.0 },    // T  F  F  F
                    new[] { -1.0, -1.0, 1.0, 1.0 },     // F  F  T  T
                    new[] { -1.0, 1.0, -1.0, 1.0 },     // F  T  F  T
                    new[] { 1.0, -1.0, -1.0, 1.0 },     // T  F  F  T
                    new[] { -1.0, 1.0, 1.0, -1.0 },     // F  T  T  F
                    new[] { 1.0, -1.0, 1.0, -1.0 },     // T  F  T  F
                    new[] { 1.0, 1.0, -1.0, -1.0 },     // T  T  F  F
                    new[] { -1.0, 1.0, 1.0, 1.0 },      // F  T  T  T
                    new[] { 1.0, -1.0, 1.0, 1.0 },      // T  F  T  T
                    new[] { 1.0, 1.0, -1.0, 1.0 },      // T  T  F  T
                    new[] { 1.0, 1.0, 1.0, -1.0 },      // T  T  T  F
                    new[] { 1.0, 1.0, 1.0, 1.0 }        // T  T  T  T
                };

            // Compound truth table answers:
            //  -1.0 = false
            //   1.0 = true
            //
            // This builds on Demo #1 by using two OR operations as the inputs for the XOR operation as such:
            // answer = (input1 OR input2) XOR (input3 OR input4)
            var idealOutputs = new[]
                {
                    new[] { -1.0 },     // (F OR F) XOR (F OR F) = F 
                    new[] { 1.0 },      // (F OR F) XOR (F OR T) = T 
                    new[] { 1.0 },      // (F OR F) XOR (T OR F) = T 
                    new[] { 1.0 },      // (F OR T) XOR (F OR F) = T 
                    new[] { 1.0 },      // (T OR F) XOR (F OR F) = T  
                    new[] { 1.0 },      // (F OR F) XOR (T OR T) = T 
                    new[] { -1.0 },     // (F OR T) XOR (F OR T) = F 
                    new[] { -1.0 },     // (T OR F) XOR (F OR T) = F 
                    new[] { -1.0 },     // (F OR T) XOR (T OR F) = F  
                    new[] { -1.0 },     // (T OR F) XOR (T OR F) = F  
                    new[] { 1.0 },      // (T OR T) XOR (F OR F) = T 
                    new[] { -1.0 },     // (F OR T) XOR (T OR T) = F 
                    new[] { -1.0 },     // (T OR F) XOR (T OR T) = F 
                    new[] { -1.0 },     // (T OR T) XOR (F OR T) = F 
                    new[] { -1.0 },     // (T OR T) XOR (T OR F) = F 
                    new[] { -1.0 }      // (T OR T) XOR (T OR T) = F 
                };

            // Create a neural network for learning the truth table.
            var network = ConstructNestedNetwork();

            //// Display the network's answers for the truth table before it has learned anything.

            Console.WriteLine("Compound Logic Problem (Untrained)");
            Console.WriteLine("------------------------");

            foreach (var input in inputs)
            {
                var outputs = network.ComputeOutputs(input);

                var firstInputText = (input[0] > 0) ? "True" : "False";
                var secondInputText = (input[1] > 0) ? "True" : "False";
                var thirdInputText = (input[2] > 0) ? "True" : "False";
                var fourthInputText = (input[3] > 0) ? "True" : "False";
                var answerText = (outputs[0] > 0) ? "True" : "False";

                Console.WriteLine("(" + firstInputText + " OR " + secondInputText + ") XOR (" + thirdInputText + " OR " + fourthInputText + ") = " + answerText + "  (" + outputs[0] + ")");
            }

            // Train the network on the truth table.
            network.Train(2000, 0.01f, 0.0f, inputs, idealOutputs);

            //// Display the network's answers now that it has studied the truth table.

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Compound Logic Problem (Trained)");
            Console.WriteLine("------------------------");

            foreach (var input in inputs)
            {
                var outputs = network.ComputeOutputs(input);

                var firstInputText = (input[0] > 0) ? "True" : "False";
                var secondInputText = (input[1] > 0) ? "True" : "False";
                var thirdInputText = (input[2] > 0) ? "True" : "False";
                var fourthInputText = (input[3] > 0) ? "True" : "False";
                var answerText = (outputs[0] > 0) ? "True" : "False";

                Console.WriteLine("(" + firstInputText + " OR " + secondInputText + ") XOR (" + thirdInputText + " OR " + fourthInputText + ") = " + answerText + "  (" + outputs[0] + ")");
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Constructs a new neural network with nested sub-networks.
        /// </summary>
        /// <returns>
        /// A constructed neural network.
        /// </returns>
        private static ISupervisedLearnerNetwork ConstructNestedNetwork()
        {
            // The activation function for the hidden and output layers.
            var sigmoidFunction = new HyperbolicTangentFunction();

            // Create a backpropagation network for supervised learning.
            var nestedNetwork = new BackpropagationNetwork();
            
            // INPUT LAYER:  2 Sub-networks
            // (Each of these sub-networks are used to "associate" the two pairs of inputs which
            // represent the OR operations that feed into the XOR operation.)
            var subNetwork1 = ConstructSubnetwork();
            var subNetwork2 = ConstructSubnetwork();
            nestedNetwork.AddInputNode(subNetwork1);
            nestedNetwork.AddInputNode(subNetwork2);

            // OUTPUT LAYER:  Create an ouput layer with one neuron.
            var outputNeuron = new BackpropagationNeuron(sigmoidFunction);
            nestedNetwork.AddOutputNode(outputNeuron);

            // HIDDEN LAYER:  Create a hidden layer with three neurons.
            // (Note that the hidden layer isn't defined explicitly like the output and input layers.)
            var hiddenNeuron1 = new BackpropagationNeuron(sigmoidFunction);
            var hiddenNeuron2 = new BackpropagationNeuron(sigmoidFunction);
            var hiddenNeuron3 = new BackpropagationNeuron(sigmoidFunction);

            // Manually wire the network to be a feedforward network just as in Demo #1.  
            // Notice that the sub-networks receive connections the same as regular neurons do.
            //
            // Input Layer to Hidden Layer
            new BackpropagationConnection(subNetwork1, hiddenNeuron1);
            new BackpropagationConnection(subNetwork1, hiddenNeuron2);
            new BackpropagationConnection(subNetwork1, hiddenNeuron3);
            new BackpropagationConnection(subNetwork2, hiddenNeuron1);
            new BackpropagationConnection(subNetwork2, hiddenNeuron2);
            new BackpropagationConnection(subNetwork2, hiddenNeuron3);

            // Hidden Layer to Output Layer
            new BackpropagationConnection(hiddenNeuron1, outputNeuron);
            new BackpropagationConnection(hiddenNeuron2, outputNeuron);
            new BackpropagationConnection(hiddenNeuron3, outputNeuron);

            return nestedNetwork;
        }

        /// <summary>
        /// Constructs a new neural network.
        /// </summary>
        /// <returns>
        /// A constructed neural network.
        /// </returns>
        /// <remarks>
        /// The subnetwork created in this method is just a normal network
        /// with all neurons (same as the network in demo #1).
        /// </remarks>
        private static ISupervisedLearnerNetwork ConstructSubnetwork()
        {
            // Create a backpropagation network for supervised learning.
            var network = new BackpropagationNetwork();

            // Use two activation functions.
            // Linear will be used for the input nodes.
            // Sigmoid will be used for all other nodes.
            var linearFunction = new LinearActivationFunction();
            var sigmoidFunction = new HyperbolicTangentFunction();

            // INPUT LAYER:  Create an input layer with two neurons.
            var inputNeuron1 = new BackpropagationNeuron(linearFunction);
            var inputNeuron2 = new BackpropagationNeuron(linearFunction);
            network.AddInputNode(inputNeuron1);
            network.AddInputNode(inputNeuron2);

            // OUTPUT LAYER:  Create an ouput layer with one neuron.
            var outputNeuron = new BackpropagationNeuron(sigmoidFunction);
            network.AddOutputNode(outputNeuron);

            // HIDDEN LAYER:  Create a hidden layer with three neurons.
            // (Note that the hidden layer isn't defined explicitly like the output and input layers.)
            var hiddenNeuron1 = new BackpropagationNeuron(sigmoidFunction);
            var hiddenNeuron2 = new BackpropagationNeuron(sigmoidFunction);
            var hiddenNeuron3 = new BackpropagationNeuron(sigmoidFunction);

            // So far, the network isn't usable, because no nodes are ever connected by default.

            // Manually wire the input layer to the hidden layer.  Notice that we'll connect every
            // input neuron to every output neuron in order to create a proper feedforward network.
            // If we wanted a different structure, we would use a different wiring pattern.
            new BackpropagationConnection(inputNeuron1, hiddenNeuron1);
            new BackpropagationConnection(inputNeuron1, hiddenNeuron2);
            new BackpropagationConnection(inputNeuron1, hiddenNeuron3);
            new BackpropagationConnection(inputNeuron2, hiddenNeuron1);
            new BackpropagationConnection(inputNeuron2, hiddenNeuron2);
            new BackpropagationConnection(inputNeuron2, hiddenNeuron3);

            // Manually wire the hidden layer to the output layer.
            new BackpropagationConnection(hiddenNeuron1, outputNeuron);
            new BackpropagationConnection(hiddenNeuron2, outputNeuron);
            new BackpropagationConnection(hiddenNeuron3, outputNeuron);

            return network;
        }
    }
}
