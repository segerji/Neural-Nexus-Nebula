using System;
using NumSharp;
using System.Collections.Generic;

public class OrbBrain
{
    private readonly int _inputSize;
    private readonly int _outputSize;
    private readonly int[] _hiddenLayers;
    private readonly Random _random;
    private NDArray _weights;

    public int InputSize => _inputSize;
    public int OutputSize => _outputSize;
    public int[] HiddenLayers => _hiddenLayers;

    public OrbBrain(int inputSize, int outputSize, int[] hiddenLayers)
    {
        _inputSize = inputSize;
        _outputSize = outputSize;
        _hiddenLayers = hiddenLayers;
        _random = new Random();

        // Initialize weights for the neural network
        _weights = InitializeWeights();
    }

    private NDArray InitializeWeights()
    {
        var weightsList = new List<NDArray>();

        int prevLayerSize = _inputSize;
        foreach (var layerSize in _hiddenLayers)
        {
            var layerWeights = np.random.randn(prevLayerSize, layerSize).flatten();
            weightsList.Add(layerWeights);
            prevLayerSize = layerSize;
        }

        var outputWeights = np.random.randn(prevLayerSize, _outputSize).flatten();
        weightsList.Add(outputWeights);

        // Concatenate all flattened weights into a single NDArray
        var weights = np.concatenate(weightsList.ToArray());

        return weights;
    }


    public float[] Predict(float[] inputs)
    {
        var inputTensor = np.array(inputs);
        var layerInput = inputTensor.reshape(1, -1); // Ensure layerInput is 2D with shape (1, inputSize)
        var incomingLayerSize = _inputSize;

        int weightIndex = 0;
        foreach (var layerSize in _hiddenLayers)
        {
            var endWeightIndex = weightIndex + incomingLayerSize * layerSize;
            var layerWeights = _weights[np.arange(weightIndex, endWeightIndex)];
            layerWeights = layerWeights.reshape((incomingLayerSize, layerSize));
            layerInput = np.dot(layerInput, layerWeights);
            layerInput = np.tanh(layerInput);
            weightIndex = endWeightIndex;
            incomingLayerSize = layerSize;
        }

        var outputWeights = _weights[np.arange(weightIndex, weightIndex + incomingLayerSize * _outputSize)];
        outputWeights = outputWeights.reshape((incomingLayerSize, _outputSize));
        var output = np.dot(layerInput, outputWeights);
        return output.GetData<float>().ToArray();
    }



    public float[] GetWeights()
    {
        return _weights.GetData<float>().ToArray();
    }

    public void SetWeights(float[] weights)
    {
        _weights = np.array(weights);
    }
}
