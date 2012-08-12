// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NeuralNetworkTests.cs" company="The Logans Ferry Software Co.">
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

namespace LogansFerry.NeuroDotNet.UnitTests
{
    using System;

    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// A collection of unit tests for the NeuralNetwork.cs class.
    /// </summary>
    [TestFixture]
    public class NeuralNetworkTests
    {
        /// <summary>
        /// Property:       InputSize
        /// Requirement:    The network's input size should be equal the sum of it's input layer inputs sizes.
        /// </summary>
        [Test]
        public void InputSize_BasedOnSumOfInputLayerSizes()
        {
            const int InputSize1 = 2;
            const int InputSize2 = 5;
            const int Expected = InputSize1 + InputSize2;

            // Setup:  Program mock input nodes.
            var mockNode1 = new Mock<INeuralNode>();
            var mockNode2 = new Mock<INeuralNode>();
            mockNode1.SetupGet(mock => mock.InputSize).Returns(InputSize1);
            mockNode2.SetupGet(mock => mock.InputSize).Returns(InputSize2);

            // setup network
            var network = new NeuralNetwork();
            network.AddInputNode(mockNode1.Object);
            network.AddInputNode(mockNode2.Object);

            // Verify
            Assert.AreEqual(Expected, network.InputSize);
            mockNode1.VerifyGet(mock => mock.InputSize, Times.Once());
            mockNode2.VerifyGet(mock => mock.InputSize, Times.Once());
        }

        /// <summary>
        /// Property:       InputSize
        /// Requirement:    The network's input size should be zero when it has no input nodes.
        /// </summary>
        [Test]
        public void InputSize_NoExceptionWithNoInputLayer()
        {
            // setup network
            var network = new NeuralNetwork();
            
            // Verify
            Assert.AreEqual(0, network.InputSize);
        }

        /// <summary>
        /// Property:      OutputSize
        /// Requirement:    The network's output size should be equal to the sum of the output sizes of its output layers.
        /// </summary>
        [Test]
        public void OutputSize_BasedOnSumOfOutputLayerSizes()
        {
            const int OutputSize1 = 2;
            const int OutputSize2 = 5;
            const int Expected = OutputSize1 + OutputSize2;

            // Setup:  Program mock output nodes.
            var mockNode1 = new Mock<INeuralNode>();
            var mockNode2 = new Mock<INeuralNode>();
            mockNode1.SetupGet(mock => mock.OutputSize).Returns(OutputSize1);
            mockNode2.SetupGet(mock => mock.OutputSize).Returns(OutputSize2);

            // setup network
            var network = new NeuralNetwork();
            network.AddOutputNode(mockNode1.Object);
            network.AddOutputNode(mockNode2.Object);

            // Verify
            Assert.AreEqual(Expected, network.OutputSize);
            mockNode1.VerifyGet(mock => mock.OutputSize, Times.Once());
            mockNode2.VerifyGet(mock => mock.OutputSize, Times.Once());
        }

        /// <summary>
        /// Property:       OutputSize
        /// Requirement:    The network's output size should be zero when there are no output nodes.
        /// </summary>
        [Test]
        public void OutputSize_NoExceptionWithNoOutputLayer()
        {
            // setup network
            var network = new NeuralNetwork();

            // Verify
            Assert.AreEqual(0, network.OutputSize);
        }

        /// <summary>
        /// Method:         AddInboundConnection
        /// Requirement:    The method will throw a Null Argument Exception when a null is provided.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddInboundConnection_ValidatesArgs()
        {
            // Setup
            var network = new NeuralNetwork();

            // Execute/Verify
            network.AddInboundConnection(null);
        }

        /// <summary>
        /// Method:         AddInboundConnection
        /// Requirement:    Will not add duplicates
        /// </summary>
        [Test]
        public void AddInboundConnection_WillNotAddDuplicates()
        {
            // Setup
            var mockConnection1 = new Mock<INeuralConnection>();
            var mockConnection2 = new Mock<INeuralConnection>();

            var network = new NeuralNetwork();

            // Execute
            network.AddInboundConnection(mockConnection1.Object);
            network.AddInboundConnection(mockConnection2.Object);
            network.AddInboundConnection(mockConnection1.Object);

            // Verify
            Assert.AreEqual(2, network.InboundConnections.Count);
            Assert.IsTrue(network.InboundConnections.Contains(mockConnection1.Object));
            Assert.IsTrue(network.InboundConnections.Contains(mockConnection2.Object));
        }

        /// <summary>
        /// Method:         AddOutboundConnection
        /// Requirement:    The method will throw a Null Argument Exception when a null is provided.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOutboundConnection_ValidatesArgs()
        {
            // Setup
            var network = new NeuralNetwork();

            // Execute/Verify
            network.AddOutboundConnection(null);
        }

        /// <summary>
        /// Method:         AddOutboundConnection
        /// Requirement:    Will not add duplicates
        /// </summary>
        [Test]
        public void AddOutboundConnection_WillNotAddDuplicates()
        {
            // Setup
            var mockConnection1 = new Mock<INeuralConnection>();
            var mockConnection2 = new Mock<INeuralConnection>();

            var network = new NeuralNetwork();

            // Execute
            network.AddOutboundConnection(mockConnection1.Object);
            network.AddOutboundConnection(mockConnection2.Object);
            network.AddOutboundConnection(mockConnection1.Object);

            // Verify
            Assert.AreEqual(2, network.OutboundConnections.Count);
            Assert.IsTrue(network.OutboundConnections.Contains(mockConnection1.Object));
            Assert.IsTrue(network.OutboundConnections.Contains(mockConnection2.Object));
        }

        /// <summary>
        /// Method:         AddInputNode
        /// Requirement:    The method will throw a Null Argument Exception when a null is provided.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddInputNode_ValidatesArgs()
        {
            // Setup
            var network = new NeuralNetwork();

            // Execute/Verify
            network.AddInputNode(null);
        }

        /// <summary>
        /// Method:         AddInputNode
        /// Requirement:    Will not add duplicates
        /// </summary>
        [Test]
        public void AddInputNode_WillNotAddDuplicates()
        {
            // Setup
            var mockNode1 = new Mock<INeuralNode>();
            var mockNode2 = new Mock<INeuralNode>();

            var network = new NeuralNetwork();

            // Execute
            network.AddInputNode(mockNode1.Object);
            network.AddInputNode(mockNode2.Object);
            network.AddInputNode(mockNode1.Object);

            // Verify
            Assert.AreEqual(2, network.InputNodes.Count);
            Assert.IsTrue(network.InputNodes.Contains(mockNode1.Object));
            Assert.IsTrue(network.InputNodes.Contains(mockNode2.Object));
        }

        /// <summary>
        /// Method:         AddOutputNode
        /// Requirement:    The method will throw a Null Argument Exception when a null is provided.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOutputNode_ValidatesArgs()
        {
            // Setup
            var network = new NeuralNetwork();

            // Execute/Verify
            network.AddOutputNode(null);
        }

        /// <summary>
        /// Method:         AddOutputNode
        /// Requirement:    Will not add duplicates
        /// </summary>
        [Test]
        public void AddOutputNode_WillNotAddDuplicates()
        {
            // Setup
            var mockNode1 = new Mock<INeuralNode>();
            var mockNode2 = new Mock<INeuralNode>();

            var network = new NeuralNetwork();

            // Execute
            network.AddOutputNode(mockNode1.Object);
            network.AddOutputNode(mockNode2.Object);
            network.AddOutputNode(mockNode1.Object);

            // Verify
            Assert.AreEqual(2, network.OutputNodes.Count);
            Assert.IsTrue(network.OutputNodes.Contains(mockNode1.Object));
            Assert.IsTrue(network.OutputNodes.Contains(mockNode2.Object));
        }

        /// <summary>
        /// Method:         Fire (double)
        /// Requirement:    The Fire method will not cause the network to fire/activate until all inbound connections have fired.
        /// </summary>
        [Test]
        public void Fire_DoesNotFireUntilAllConnectionsFired()
        {
            //// SETUP

            // Create 2 input and 2 output mock nodes
            var mockInputNode1 = new Mock<INeuralNode>();
            var mockInputNode2 = new Mock<INeuralNode>();
            var mockOutputNode1 = new Mock<INeuralNode>();
            var mockOutputNode2 = new Mock<INeuralNode>();

            // Create 2 inbound and 2 outbound mock connections.
            var mockInboundFired = new Mock<INeuralConnection>();
            var mockInboundUnfired = new Mock<INeuralConnection>();
            var mockOutbound1 = new Mock<INeuralConnection>();
            var mockOutbound2 = new Mock<INeuralConnection>();

            // program the mock inbounds such that one is fired and one isn't
            mockInboundFired.SetupGet(mock => mock.IsFired).Returns(true);
            mockInboundUnfired.SetupGet(mock => mock.IsFired).Returns(false);
            
            // Create the test object.
            var network = new NeuralNetwork();
            network.AddInboundConnection(mockInboundFired.Object);
            network.AddInboundConnection(mockInboundUnfired.Object);
            network.AddOutboundConnection(mockOutbound1.Object);
            network.AddOutboundConnection(mockOutbound2.Object);
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);

            // Archive the initialized output values for later verification.
            var initialOutput = network.CachedOutputs;

            // EXECUTION
            const double Input = -2.3d;
            network.Fire(Input);

            // VERIFICATION:  The fire signals were checked...
            mockInboundFired.Verify(mock => mock.IsFired, Times.Exactly(1));
            mockInboundUnfired.Verify(mock => mock.IsFired, Times.Exactly(1));

            // ...but no activation activities occurred.
            mockInputNode1.Verify(mock => mock.Fire(It.IsAny<double[]>()), Times.Never());
            mockInputNode2.Verify(mock => mock.Fire(It.IsAny<double[]>()), Times.Never());
            mockOutputNode1.Verify(mock => mock.CachedOutputs, Times.Never());
            mockOutputNode2.Verify(mock => mock.CachedOutputs, Times.Never());
            mockOutbound1.Verify(mock => mock.Fire(It.IsAny<double>()), Times.Never());
            mockOutbound2.Verify(mock => mock.Fire(It.IsAny<double>()), Times.Never());
            mockInboundFired.Verify(mock => mock.ClearFire(), Times.Never());
            mockInboundUnfired.Verify(mock => mock.ClearFire(), Times.Never());
            Assert.AreEqual(initialOutput, network.CachedOutputs);
        }

        /// <summary>
        /// Method:         Fire
        /// Requirements:   1) The Fire method will cause the network to fire/activate after the final inbound connection fired.
        ///                 2) Activation causes network to initiate internal computations.
        ///                 3) Internal computations are propogated forward to outbound connections.
        /// </summary>
        [Test]
        public void Fire_FiresWhenAllConnectionsFired()
        {
            const double Input1 = -2.3d;
            const double DigitizedInput1 = -1.0d;
            const double Input2 = 0.3d;
            const double DigitizedInput2 = 1.0d;
            const double Output1 = -0.01d;
            const double DigitizedOutput1 = -1.0d;
            const double Output2 = 2.6d;
            const double DigitizedOutput2 = 1.0d;

            var outputs = new[] { Output1, Output2 };

            //// SETUP

            // Create 2 input and 2 output mock nodes
            var mockInputNode1 = new Mock<INeuralNode>();
            var mockInputNode2 = new Mock<INeuralNode>();
            var mockOutputNode1 = new Mock<INeuralNode>();
            var mockOutputNode2 = new Mock<INeuralNode>();

            // Program mock nodes

            // Input nodes have an input size of 1
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(1);

            // Output nodes have an output size of 1
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(1);

            // Configure the output nodes to return the output constants.
            mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output1 });
            mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output2 });

            // Create 2 inbound and 1 outbound mock connections.
            var mockInboundFired = new Mock<INeuralConnection>();
            var mockInboundUnfired = new Mock<INeuralConnection>();
            var mockOutbound1 = new Mock<INeuralConnection>();
            var mockOutbound2 = new Mock<INeuralConnection>();

            // program the mock inbounds such that both have fired.
            mockInboundFired.SetupGet(mock => mock.IsFired).Returns(true);
            mockInboundUnfired.SetupGet(mock => mock.IsFired).Returns(true);

            // Each connection should provide one of the raw inputs.
            mockInboundFired.SetupGet(mock => mock.Output).Returns(Input1);
            mockInboundUnfired.SetupGet(mock => mock.Output).Returns(Input2);

            // Create the test object.
            var network = new NeuralNetwork();
            network.AddInboundConnection(mockInboundFired.Object);
            network.AddInboundConnection(mockInboundUnfired.Object);
            network.AddOutboundConnection(mockOutbound1.Object);
            network.AddOutboundConnection(mockOutbound2.Object);
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);

            // EXECUTION
            network.Fire(Input2);

            // VERIFICATION

            // The fire signals were checked...
            mockInboundFired.Verify(mock => mock.IsFired, Times.Once());
            mockInboundUnfired.Verify(mock => mock.IsFired, Times.Once());

            // ...and all activation activities occurred.
            mockInputNode1.Verify(mock => mock.Fire(new[] { DigitizedInput1 }), Times.Once());
            mockInputNode2.Verify(mock => mock.Fire(new[] { DigitizedInput2 }), Times.Once());
            mockOutputNode1.Verify(mock => mock.CachedOutputs, Times.AtLeastOnce());
            mockOutputNode2.Verify(mock => mock.CachedOutputs, Times.AtLeastOnce());
            mockOutbound1.Verify(mock => mock.Fire(DigitizedOutput1), Times.Once());
            mockOutbound2.Verify(mock => mock.Fire(DigitizedOutput2), Times.Once());
            mockInboundFired.Verify(mock => mock.ClearFire(), Times.Once());
            mockInboundUnfired.Verify(mock => mock.ClearFire(), Times.Once());
            Assert.AreEqual(outputs, network.CachedOutputs);
        }

        /// <summary>
        /// Method:         Fire (double[])
        /// Requirements:   1) The network immediately activates with the provided inputs.
        ///                 2) Activation causes network to initiate internal computations.
        ///                 3) Internal computations are propogated forward to outbound connections.
        /// </summary>
        [Test]
        public void FireArray_ActivatesImmediately()
        {
            const double Input1 = -2.3d;
            const double DigitizedInput1 = -1.0d;
            const double Input2 = 0.3d;
            const double DigitizedInput2 = 1.0d;
            const double Output1 = -0.01d;
            const double DigitizedOutput1 = -1.0d;
            const double Output2 = 2.6d;
            const double DigitizedOutput2 = 1.0d;

            var outputs = new[] { Output1, Output2 };

            //// SETUP

            // Create 2 input and 2 output mock nodes
            var mockInputNode1 = new Mock<INeuralNode>();
            var mockInputNode2 = new Mock<INeuralNode>();
            var mockOutputNode1 = new Mock<INeuralNode>();
            var mockOutputNode2 = new Mock<INeuralNode>();

            // Program mock nodes

            // Input nodes have an input size of 1
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(1);

            // Output nodes have an output size of 1
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(1);

            // Configure the output nodes to return the output constants.
            mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output1 });
            mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output2 });

            // Create 2 inbound and 1 outbound mock connections.
            var mockInboundFired = new Mock<INeuralConnection>();
            var mockInboundUnfired = new Mock<INeuralConnection>();
            var mockOutbound1 = new Mock<INeuralConnection>();
            var mockOutbound2 = new Mock<INeuralConnection>();

            // Create the test object.
            var network = new NeuralNetwork();
            network.AddInboundConnection(mockInboundFired.Object);
            network.AddInboundConnection(mockInboundUnfired.Object);
            network.AddOutboundConnection(mockOutbound1.Object);
            network.AddOutboundConnection(mockOutbound2.Object);
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);

            // EXECUTION
            network.Fire(new[] { Input1, Input2 });

            // VERIFICATION 

            // The inbound connections were not used...
            mockInboundFired.Verify(mock => mock.IsFired, Times.Never());
            mockInboundUnfired.Verify(mock => mock.IsFired, Times.Never());
            mockInboundFired.Verify(mock => mock.ClearFire(), Times.Never());
            mockInboundUnfired.Verify(mock => mock.ClearFire(), Times.Never());
            
            // ...and the activation activities occurred.
            mockInputNode1.Verify(mock => mock.Fire(new[] { DigitizedInput1 }), Times.Once());
            mockInputNode2.Verify(mock => mock.Fire(new[] { DigitizedInput2 }), Times.Once());
            mockOutputNode1.Verify(mock => mock.CachedOutputs, Times.AtLeastOnce());
            mockOutputNode2.Verify(mock => mock.CachedOutputs, Times.AtLeastOnce());
            mockOutbound1.Verify(mock => mock.Fire(DigitizedOutput1), Times.Once());
            mockOutbound2.Verify(mock => mock.Fire(DigitizedOutput2), Times.Once());
            Assert.AreEqual(outputs, network.CachedOutputs);
        }

        /// <summary>
        /// Method:         Fire(double[])
        /// Requirement:    Method can accept an input array with more inputs than the input layer size by aggregating values.
        /// </summary>
        [Test]
        public void FireArray_AcceptsNumInputsGreaterThanExpected()
        {
            const double Input1 = -2.3d;
            const double Input2 = 3.0d;
            const double DigitizedInput = 1.0d;
            const double Output1 = -0.01d;
            const double DigitizedOutput1 = -1.0d;
            const double Output2 = 2.6d;
            const double DigitizedOutput2 = 1.0d;

            var outputs = new[] { Output1, Output2 };

            //// SETUP

            // Create 1 input and 2 output mock nodes
            var mockInputNode = new Mock<INeuralNode>();
            var mockOutputNode1 = new Mock<INeuralNode>();
            var mockOutputNode2 = new Mock<INeuralNode>();

            // Program mock nodes

            // Input nodes have an input size of 1
            mockInputNode.SetupGet(mock => mock.InputSize).Returns(1);

            // Output nodes have an output size of 1
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(1);

            // Configure the output nodes to return the output constants.
            mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output1 });
            mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output2 });

            // Create 2 inbound and 1 outbound mock connections.
            var mockInboundFired = new Mock<INeuralConnection>();
            var mockInboundUnfired = new Mock<INeuralConnection>();
            var mockOutbound1 = new Mock<INeuralConnection>();
            var mockOutbound2 = new Mock<INeuralConnection>();

            // Create the test object.
            var network = new NeuralNetwork();
            network.AddInboundConnection(mockInboundFired.Object);
            network.AddInboundConnection(mockInboundUnfired.Object);
            network.AddOutboundConnection(mockOutbound1.Object);
            network.AddOutboundConnection(mockOutbound2.Object);
            network.AddInputNode(mockInputNode.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);

            // EXECUTION
            network.Fire(new[] { Input1, Input2 });

            // VERIFICATION 

            // The inbound connections were not used...
            mockInboundFired.Verify(mock => mock.IsFired, Times.Never());
            mockInboundUnfired.Verify(mock => mock.IsFired, Times.Never());
            mockInboundFired.Verify(mock => mock.ClearFire(), Times.Never());
            mockInboundUnfired.Verify(mock => mock.ClearFire(), Times.Never());
            
            // ...and the activation activities occurred.
            mockInputNode.Verify(mock => mock.Fire(new[] { DigitizedInput }), Times.Once());
            mockOutputNode1.Verify(mock => mock.CachedOutputs, Times.AtLeastOnce());
            mockOutputNode2.Verify(mock => mock.CachedOutputs, Times.AtLeastOnce());
            mockOutbound1.Verify(mock => mock.Fire(DigitizedOutput1), Times.Once());
            mockOutbound2.Verify(mock => mock.Fire(DigitizedOutput2), Times.Once());
            Assert.AreEqual(outputs, network.CachedOutputs);
        }

        /// <summary>
        /// Method:         Fire(double[])
        /// Requirement:    Method can accept an input array with fewer inputs than the input layer size by distributing values.
        /// </summary>
        [Test]
        public void FireArray_AcceptsNumInputsFewerThanExpected()
        {
            const double Input1 = -2.3d;
            const double DigitizedInput1 = -1.0d;
            const double Input2 = 0.3d;
            const double DigitizedInput2 = 1.0d;
            const double Output1 = -0.01d;
            const double DigitizedOutput1 = -1.0d;
            const double Output2 = 2.6d;
            const double DigitizedOutput2 = 1.0d;

            var outputs = new[] { Output1, Output2 };

            //// SETUP

            // Create 2 input and 2 output mock nodes
            var mockInputNode1 = new Mock<INeuralNode>();
            var mockInputNode2 = new Mock<INeuralNode>();
            var mockOutputNode1 = new Mock<INeuralNode>();
            var mockOutputNode2 = new Mock<INeuralNode>();

            // Program mock nodes

            // Input nodes have an input sizes greater than provided.
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(2);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(3);

            // Output nodes have an output size of 1
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(1);

            // Configure the output nodes to return the output constants.
            mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output1 });
            mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output2 });

            // Create 2 inbound and 1 outbound mock connections.
            var mockInboundFired = new Mock<INeuralConnection>();
            var mockInboundUnfired = new Mock<INeuralConnection>();
            var mockOutbound1 = new Mock<INeuralConnection>();
            var mockOutbound2 = new Mock<INeuralConnection>();

            // Create the test object.
            var network = new NeuralNetwork();
            network.AddInboundConnection(mockInboundFired.Object);
            network.AddInboundConnection(mockInboundUnfired.Object);
            network.AddOutboundConnection(mockOutbound1.Object);
            network.AddOutboundConnection(mockOutbound2.Object);
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);

            // EXECUTION
            network.Fire(new[] { Input1, Input2 });

            // VERIFICATION 

            // The inbound connections were not used...
            mockInboundFired.Verify(mock => mock.IsFired, Times.Never());
            mockInboundUnfired.Verify(mock => mock.IsFired, Times.Never());
            mockInboundFired.Verify(mock => mock.ClearFire(), Times.Never());
            mockInboundUnfired.Verify(mock => mock.ClearFire(), Times.Never());
            
            // ...and the activation activities occurred.

            // Inputs were distributed across input nodes, repeating as necessary.
            mockInputNode1.Verify(mock => mock.Fire(new[] { DigitizedInput1, DigitizedInput1 }), Times.Once());
            mockInputNode2.Verify(mock => mock.Fire(new[] { DigitizedInput1, DigitizedInput2, DigitizedInput2 }), Times.Once());

            mockOutputNode1.Verify(mock => mock.CachedOutputs, Times.AtLeastOnce());
            mockOutputNode2.Verify(mock => mock.CachedOutputs, Times.AtLeastOnce());
            mockOutbound1.Verify(mock => mock.Fire(DigitizedOutput1), Times.Once());
            mockOutbound2.Verify(mock => mock.Fire(DigitizedOutput2), Times.Once());
            Assert.AreEqual(outputs, network.CachedOutputs);
        }

        /// <summary>
        /// Method:         ComputeOutputs
        /// Requirement:    1) The method should distribute the values within the input set across the input nodes, 
        ///                 based on the InputSize of each input node.
        ///                 2) The method should return a combined array of the output values of the output nodes.
        /// </summary>
        [Test]
        public void ComputeOutputs_DistributesInputSetAcrossInputNodesBySize()
        {
            //// SETUP

            const double Input1 = 1.0;
            const double Input2 = 2.0;
            const double Input3 = 3.0;
            const double Input4 = 4.0;
            const double Input5 = 5.0;
            const double Input6 = 6.0;

            const double Output1 = 0.1;
            const double Output2 = 0.2;
            const double Output3 = 0.3;
            const double Output4 = 0.4;
            const double Output5 = 0.5;
            const double Output6 = 0.6;

            // Create mock input nodes.
            var mockInputNode1 = new Mock<INeuralNode>();
            var mockInputNode2 = new Mock<INeuralNode>();
            var mockInputNode3 = new Mock<INeuralNode>();

            // Program input sizes.
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(2);
            mockInputNode3.SetupGet(mock => mock.InputSize).Returns(3);

            // Create mock output nodes.
            var mockOutputNode1 = new Mock<INeuralNode>();
            var mockOutputNode2 = new Mock<INeuralNode>();
            var mockOutputNode3 = new Mock<INeuralNode>();

            // Program output sizes.
            mockOutputNode1.SetupGet(mock => mock.OutputSize).Returns(1);
            mockOutputNode2.SetupGet(mock => mock.OutputSize).Returns(2);
            mockOutputNode3.SetupGet(mock => mock.OutputSize).Returns(3);

            // Program outputs
            mockOutputNode1.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output1 });
            mockOutputNode2.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output2, Output3 });
            mockOutputNode3.SetupGet(mock => mock.CachedOutputs).Returns(new[] { Output4, Output5, Output6 });

            // Create the test object.
            var network = new NeuralNetwork();
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddInputNode(mockInputNode3.Object);
            network.AddOutputNode(mockOutputNode1.Object);
            network.AddOutputNode(mockOutputNode2.Object);
            network.AddOutputNode(mockOutputNode3.Object);

            //// EXECUTE

            var inputs = new[] { Input1, Input2, Input3, Input4, Input5, Input6 };
            var expected = new[] { Output1, Output2, Output3, Output4, Output5, Output6 };

            var actual = network.ComputeOutputs(inputs);

            //// VERIFY
            
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, network.CachedOutputs);
            mockInputNode1.Verify(mock => mock.Fire(new[] { Input1 }));
            mockInputNode2.Verify(mock => mock.Fire(new[] { Input2, Input3 }));
            mockInputNode3.Verify(mock => mock.Fire(new[] { Input4, Input5, Input6 }));
        }

        /// <summary>
        /// Method:         ComputeOutputs
        /// Requirement:    Throws Invalid Operation Exception with more than expected number of inputs.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ComputeOutputs_ThrowsInvalidOperationWithTooManyInputs()
        {
            //// SETUP

            const double Input1 = 1.0;
            const double Input2 = 2.0;
            const double Input3 = 3.0;
            const double Input4 = 4.0;
            const double Input5 = 5.0;
            const double Input6 = 6.0;

            // Create mock input nodes.
            var mockInputNode1 = new Mock<INeuralNode>();
            var mockInputNode2 = new Mock<INeuralNode>();
            var mockInputNode3 = new Mock<INeuralNode>();

            // Program input sizes.
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(2);
            mockInputNode3.SetupGet(mock => mock.InputSize).Returns(2);

            // Create the test object.
            var network = new NeuralNetwork();
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddInputNode(mockInputNode3.Object);
            
            //// EXECUTE

            var inputs = new[] { Input1, Input2, Input3, Input4, Input5, Input6 };
            network.ComputeOutputs(inputs);
        }

        /// <summary>
        /// Method:         ComputeOutputs
        /// Requirement:    Throws Invalid Operation Exception with fewer than expected number of inputs.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ComputeOutputs_ThrowsInvalidOperationWithTooFewInputs()
        {
            //// SETUP

            const double Input1 = 1.0;
            const double Input2 = 2.0;
            const double Input3 = 3.0;
            const double Input4 = 4.0;
            const double Input5 = 5.0;

            // Create mock input nodes.
            var mockInputNode1 = new Mock<INeuralNode>();
            var mockInputNode2 = new Mock<INeuralNode>();
            var mockInputNode3 = new Mock<INeuralNode>();

            // Program input sizes.
            mockInputNode1.SetupGet(mock => mock.InputSize).Returns(1);
            mockInputNode2.SetupGet(mock => mock.InputSize).Returns(2);
            mockInputNode3.SetupGet(mock => mock.InputSize).Returns(3);

            // Create the test object.
            var network = new NeuralNetwork();
            network.AddInputNode(mockInputNode1.Object);
            network.AddInputNode(mockInputNode2.Object);
            network.AddInputNode(mockInputNode3.Object);

            //// EXECUTE

            var inputs = new[] { Input1, Input2, Input3, Input4, Input5 };
            network.ComputeOutputs(inputs);
        }

        /// <summary>
        /// Method:         ComputeOutputs
        /// Requirement:    Throw Null Argument Exception when a null is provided.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ComputeOutputs_ValidatesNullArgs()
        {
            var network = new NeuralNetwork();
            network.ComputeOutputs(null);
        }
    }
}
