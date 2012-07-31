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

namespace Demo_1_XOr
{
    using System;

    using log4net.Config;

    using LogansFerry.NeuroDotNet;

    /// <summary>
    /// This application demonstrates using neuro.net to solve the classic XOR problem.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The random number utility that will be used to generate weights.
        /// </summary>
        private static readonly Random Randomizer = new Random();

        /// <summary>
        /// The main method.
        /// </summary>
        public static void Main()
        {
            // Configure log4net
            XmlConfigurator.Configure();

            // XOR truth table inputs:
            //  -1.0 = false
            //   1.0 = true
            var inputs = new[]
                {
                    new[] { -1.0, -1.0 },   // F, F
                    new[] { 1.0, -1.0 },    // T, F
                    new[] { -1.0, 1.0 },    // F, T
                    new[] { 1.0, 1.0 }      // T, T
                };

            // XOR truth table answers:
            //  -1.0 = false
            //   1.0 = true
            var idealOutputs = new[]
                {
                    new[] { -1.0 },     // F (F XOR F)
                    new[] { 1.0 },      // T (T XOR F)
                    new[] { 1.0 },      // T (F XOR T)
                    new[] { -1.0 }      // F (T XOR T)
                };

            // Create a neural network for learning the truth table.
            var network = ConstructNetwork();

            //// Display the network's answers for the truth table before it has learned anything.
            
            Console.WriteLine("XOR Problem (Untrained)");
            Console.WriteLine("------------------------");
            
            foreach (var input in inputs)
            {
                var outputs = network.ComputeOutputs(input);

                var firstInputText = (input[0] > 0) ? "True" : "False";
                var secondInputText = (input[1] > 0) ? "True" : "False";
                var answerText = (outputs[0] > 0) ? "True" : "False";

                Console.WriteLine("XOR(" + firstInputText + ", " + secondInputText + ") = " + answerText + "  (raw output=" + outputs[0] + ")");
            }

            // Train the network on the truth table.
            network.Train(10000, 0.1f, 0.9f, inputs, idealOutputs);

            //// Display the network's answers now that it has studied the truth table.

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("XOR Problem (Trained)");
            Console.WriteLine("------------------------");

            foreach (var input in inputs)
            {
                var outputs = network.ComputeOutputs(input);

                var firstInputText = (input[0] > 0) ? "True" : "False";
                var secondInputText = (input[1] > 0) ? "True" : "False";
                var answerText = (outputs[0] > 0) ? "True" : "False";

                Console.WriteLine("XOR(" + firstInputText + ", " + secondInputText + ") = " + answerText + "  (raw output=" + outputs[0] + ")");
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Constructs a new neural network.
        /// </summary>
        /// <returns>
        /// A constructed neural network.
        /// </returns>
        private static ISupervisedLearnerNetwork ConstructNetwork()
        {
            // Create a backpropagation network for supervised learning.
            var network = new BackpropagationNetwork(new RmsCalculator());

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
            new BackpropagationConnection(GetNextRandomWeight(), inputNeuron1, hiddenNeuron1);
            new BackpropagationConnection(GetNextRandomWeight(), inputNeuron1, hiddenNeuron2);
            new BackpropagationConnection(GetNextRandomWeight(), inputNeuron1, hiddenNeuron3);
            new BackpropagationConnection(GetNextRandomWeight(), inputNeuron2, hiddenNeuron1);
            new BackpropagationConnection(GetNextRandomWeight(), inputNeuron2, hiddenNeuron2);
            new BackpropagationConnection(GetNextRandomWeight(), inputNeuron2, hiddenNeuron3);

            // Manually wire the hidden layer to the output layer.
            new BackpropagationConnection(GetNextRandomWeight(), hiddenNeuron1, outputNeuron);
            new BackpropagationConnection(GetNextRandomWeight(), hiddenNeuron2, outputNeuron);
            new BackpropagationConnection(GetNextRandomWeight(), hiddenNeuron3, outputNeuron);
            
            return network;
        }

        /// <summary>
        /// Returns a new random number within the configured weight range.
        /// </summary>
        /// <returns>
        /// The next weight value.
        /// </returns>
        private static double GetNextRandomWeight()
        {
            // Weights will be values between -1 and 1.
            const double Max = 1.0d;
            const double Min = -1.0d;

            return (Randomizer.NextDouble() * (Max - Min)) + Min;
        }
    }
}
