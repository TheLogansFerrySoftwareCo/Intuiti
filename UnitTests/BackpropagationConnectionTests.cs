// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackpropagationConnectionTests.cs" company="The Logans Ferry Software Co.">
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
    /// A collection of unit tests for the BackpropagationConnection.cs class.
    /// </summary>
    [TestFixture]
    public class BackpropagationConnectionTests
    {
        /// <summary>
        /// The required precision of floating point calculations
        /// </summary>
        private const double Epsilon = 0.000000000001d;

        /// <summary>
        /// Method:         Constructor
        /// Requirement:    Will initialize the connection end points and weight value.
        /// </summary>
        [Test]
        public void Ctor_InitializesClass()
        {
            // Setup test values
            var mockTarget = new Mock<ISupervisedLearnerNode>();
            var mockSource = new Mock<ISupervisedLearnerNode>();
            const double ExpectedWeight = 0.11d;

            // Execute
            var actual = new BackpropagationConnection(ExpectedWeight, mockSource.Object, mockTarget.Object);

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
            var mockTarget = new Mock<ISupervisedLearnerNode>();
            const double ExpectedWeight = 0.11d;

            // Execute
            new BackpropagationConnection(ExpectedWeight, null, mockTarget.Object);
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
            var mockSource = new Mock<ISupervisedLearnerNode>();
            const double ExpectedWeight = 0.11d;

            // Execute
            new BackpropagationConnection(ExpectedWeight, mockSource.Object, null);
        }

        /// <summary>
        /// Method:         ReportError
        /// Requirement:    Modifies and passes on the error signal.
        /// </summary>
        [Test]
        public void ReportError_ModifiesErrorSignal()
        {
            // Setup test values
            var mockTarget = new Mock<ISupervisedLearnerNode>();
            var mockSource = new Mock<ISupervisedLearnerNode>();
            const double Weight = 0.25d;
            const double OriginalError = 12.34d;
            const double ExpectedError = OriginalError * Weight;
            
            var connection = new BackpropagationConnection(Weight, mockSource.Object, mockTarget.Object);

            // Execute
            var originalFlag = connection.IsReportingError;
            connection.ReportError(OriginalError);
            var finalFlag = connection.IsReportingError;
            var actualError = connection.ErrorSignal;

            // Verify
            Assert.IsFalse(originalFlag);
            Assert.IsTrue(finalFlag);
            Assert.AreEqual(ExpectedError, actualError, Epsilon);
            mockSource.Verify(mock => mock.CalculateError(actualError), Times.Once());
        }

        /// <summary>
        /// Method:         ApplyWeightAdjustments.
        /// Requirement:    Uses momentum and learning rate to apply a weight adjustment.
        /// </summary>
        [Test]
        public void ApplyWeightAdjustments_UpdatesWeight()
        {
            //// SETUP
            const double InitialWeight = 0.25d;
            const double FireInput = 1.234d;
            const double ErrorSignal1A = -2.3d;
            const double ErrorSignal1B = -9.87d;
            const double AccumulatedError1 = ErrorSignal1A + ErrorSignal1B;
            const double ErrorSignal2 = 3.2345d;
            const float Momentum = 0.9f;
            const float LearningRate = 0.1f;

            const double ExpectedWeight1 = (AccumulatedError1 * FireInput * LearningRate) + InitialWeight;
            const double ExpectedWeight2 = ExpectedWeight1 + (ErrorSignal2 * FireInput * LearningRate) + (Momentum * (AccumulatedError1 * FireInput * LearningRate));

            var mockTarget = new Mock<ISupervisedLearnerNode>();
            var mockSource = new Mock<ISupervisedLearnerNode>();
            
            // Create the test object.
            var connection = new BackpropagationConnection(InitialWeight, mockSource.Object, mockTarget.Object);
            connection.Fire(FireInput);

            // EXECUTION
            connection.ReportError(ErrorSignal1A);
            connection.ReportError(ErrorSignal1B);
            connection.ApplyWeightAdjustments(LearningRate, Momentum);
            var actual1 = connection.Weight;

            connection.ClearCachedErrors();
            connection.ReportError(ErrorSignal2);
            connection.ApplyWeightAdjustments(LearningRate, Momentum);
            var actual2 = connection.Weight;

            // VERIFICATION
            Assert.AreEqual(ExpectedWeight1, actual1, Epsilon);
            Assert.AreEqual(ExpectedWeight2, actual2, Epsilon);
        }

        /// <summary>
        /// Method:         ApplyWeightAdjustments.
        /// Requirement:    Propogates through the network.
        /// </summary>
        [Test]
        public void ApplyWeightAdjustments_PropogatesThroughNetwork()
        {
            //// SETUP
            const double InitialWeight = 0.25d;
            const float Momentum = 0.9f;
            const float LearningRate = 0.1f;

            var mockTarget = new Mock<ISupervisedLearnerNode>();
            var mockSource = new Mock<ISupervisedLearnerNode>();
            
            // Create the test object.
            var connection = new BackpropagationConnection(InitialWeight, mockSource.Object, mockTarget.Object);
            
            // EXECUTION
            connection.ApplyWeightAdjustments(LearningRate, Momentum);
            
            // VERIFICATION
            mockTarget.Verify(mock => mock.ApplyWeightAdjustments(LearningRate, Momentum), Times.Once());
        }

        /// <summary>
        /// Method:         ClearReportingFlag
        /// Requirement:    Clears the IsReportingFlag
        /// </summary>
        [Test]
        public void ClearReportingFlag_ClearsReportingFlag()
        {
            //// SETUP
            const double InitialWeight = 0.25d;
            const double ErrorSignal = -2.3d;
            
            var mockTarget = new Mock<ISupervisedLearnerNode>();
            var mockSource = new Mock<ISupervisedLearnerNode>();

            // Create the test object.
            var connection = new BackpropagationConnection(InitialWeight, mockSource.Object, mockTarget.Object);
            
            // EXECUTION
            connection.ReportError(ErrorSignal);
            var preClearFlag = connection.IsReportingError;
            connection.ClearReportingFlag();
            var postClearFlag = connection.IsReportingError;

            // VERIFICATION
            Assert.IsTrue(preClearFlag);
            Assert.IsFalse(postClearFlag);
        }

        /// <summary>
        /// Method:         ClearCachedErrors
        /// Requirement:    Propogates through the network.
        /// </summary>
        [Test]
        public void ClearCachedErrors_PropogatesThroughNetwork()
        {
            //// SETUP
            const double InitialWeight = 0.25d;
            
            var mockTarget = new Mock<ISupervisedLearnerNode>();
            var mockSource = new Mock<ISupervisedLearnerNode>();

            // Create the test object.
            var connection = new BackpropagationConnection(InitialWeight, mockSource.Object, mockTarget.Object);

            // EXECUTION
            connection.ClearCachedErrors();

            // VERIFICATION
            mockTarget.Verify(mock => mock.ClearCachedErrors(), Times.Once());
        }
    }
}
