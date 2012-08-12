// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NeuralConnectionTests.cs" company="The Logans Ferry Software Co.">
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
    /// A collection of unit tests for the NeuralConnection.cs class.
    /// </summary>
    [TestFixture]
    public class NeuralConnectionTests
    {
        /// <summary>
        /// Method:         Constructor
        /// Requirement:    Will initialize the connection end points and weight value.
        /// </summary>
        [Test]
        public void Ctor_InitializesClass()
        {
            // Setup test values
            var mockTarget = new Mock<INeuralNode>();
            var mockSource = new Mock<INeuralNode>();
            const double ExpectedWeight = 0.11d;

            // Execute
            var actual = new NeuralConnection(ExpectedWeight, mockSource.Object, mockTarget.Object);

            // Verify
            mockSource.Verify(mock => mock.AddOutboundConnection(actual), Times.Once());
            mockTarget.Verify(mock => mock.AddInboundConnection(actual), Times.Once());

            Assert.AreEqual(ExpectedWeight, actual.Weight);
            Assert.AreSame(mockSource.Object, actual.SourceNode);
            Assert.AreSame(mockTarget.Object, actual.TargetNode);
        }

        /// <summary>
        /// Method:         Constructor
        /// Requirement:    Validates SourceNode for null.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_ValidatesSourceNode()
        {
            // Setup test values
            var mockTarget = new Mock<INeuralNode>();
            const double ExpectedWeight = 0.11d;

            // Execute
            new NeuralConnection(ExpectedWeight, null, mockTarget.Object);
        }

        /// <summary>
        /// Method:         Constructor
        /// Requirement:    Validates TargetNode for null.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_ValidatesTargetNode()
        {
            // Setup test values
            var mockSource = new Mock<INeuralNode>();
            const double ExpectedWeight = 0.11d;

            // Execute
            new NeuralConnection(ExpectedWeight, mockSource.Object, null);
        }

        /// <summary>
        /// Method:         Fire
        /// Requirement:    Caches the provided input value.
        /// </summary>
        [Test]
        public void Fire_CachesInput()
        {
            // Setup test values
            var mockTarget = new Mock<INeuralNode>();
            var mockSource = new Mock<INeuralNode>();
            const double Weight = 0.1d;
            const double Expected = 0.123d;
            
            // Setup target
            var connection = new NeuralConnection(Weight, mockSource.Object, mockTarget.Object);

            // Execute
            connection.Fire(Expected);

            // Verify
            Assert.AreEqual(Expected, connection.CachedInput);
        }

        /// <summary>
        /// Method:         Fire
        /// Requirement:    Modifies signal by multiplying input times weight, then passes signal to the target.
        /// </summary>
        [Test]
        public void Fire_ModifiesInputSignal()
        {
            // Setup test values
            var mockTarget = new Mock<INeuralNode>();
            var mockSource = new Mock<INeuralNode>();
            const double Weight = 0.123d;
            const double Input = 2.34d;
            const double Expected = Weight * Input;

            // Setup target
            var connection = new NeuralConnection(Weight, mockSource.Object, mockTarget.Object);

            // Execute
            connection.Fire(Input);

            // Verify
            mockTarget.Verify(mock => mock.Fire(Expected));
        }

        /// <summary>
        /// Method:         Fire/ClearFire
        /// Requirement:    Fire sets IsFired flag; ClearFire removes IsFired flag.
        /// </summary>
        [Test]
        public void Fire_MaintainsFiredFlag()
        {
            // Setup test values
            var mockTarget = new Mock<INeuralNode>();
            var mockSource = new Mock<INeuralNode>();
            const double Weight = 0.123d;
            const double Input = 2.34d;
            
            // Setup target
            var connection = new NeuralConnection(Weight, mockSource.Object, mockTarget.Object);

            // Execute
            var preFiredFlag = connection.IsFired;
            
            connection.Fire(Input);
            var postFiredFlag = connection.IsFired;

            connection.ClearFire();
            var postClearedFlag = connection.IsFired;

            // Verify
            Assert.IsFalse(preFiredFlag);
            Assert.IsTrue(postFiredFlag);
            Assert.IsFalse(postClearedFlag);
        }

        /// <summary>
        /// Method:         Fire
        /// Requirement:    Throws Invalid Operation when IsFired is true.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Fire_WontFireWhenAlreadyFired()
        {
            // Setup test values
            var mockTarget = new Mock<INeuralNode>();
            var mockSource = new Mock<INeuralNode>();
            const double Weight = 0.123d;
            const double Input = 2.34d;
            
            // Setup target
            var connection = new NeuralConnection(Weight, mockSource.Object, mockTarget.Object);

            // Execute
            connection.Fire(Input);
            connection.Fire(Input);
        }
    }
}
