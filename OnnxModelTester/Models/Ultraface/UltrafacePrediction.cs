// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using OnnxModelTester.PrePostProcessing;

namespace OnnxModelTester.Models.Ultraface
{
    public class UltrafacePrediction
    {
        public PredictionBox Box { get; set; }
        public float Confidence { get; set; }
    }
}