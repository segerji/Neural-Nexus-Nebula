using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Tensorflow.Binding;
using System;
using NDArray = Tensorflow.NumPy.NDArray;
using np = Tensorflow.NumPy.np;
using Tensorflow;

public class OrbBrain
{
    private readonly int _inputSize;
    private readonly int _outputSize;
    private readonly int[] _hiddenLayers;
    private readonly Random _random;
    private float[] _weights;
    private float[] _biases;

    public int InputSize => _inputSize;
    public int OutputSize => _outputSize;
    public int[] HiddenLayers => _hiddenLayers;

    public OrbBrain(int inputSize, int outputSize, int[] hiddenLayers)
    {
        _inputSize = inputSize;
        _outputSize = outputSize;
        _hiddenLayers = hiddenLayers;
        _random = new Random();

        // Initialize weights and biases for the neural network
        _weights = InitializeWeights();
        _biases = InitializeBiases();
    }

    private float[] InitializeWeights()
    {
        var weightsList = new List<float>();

        int prevLayerSize = _inputSize;
        foreach (var layerSize in _hiddenLayers)
        {
            var layerWeights = np.random.randn(prevLayerSize, layerSize) / Math.Sqrt(prevLayerSize);
            weightsList.AddRange(layerWeights.astype(np.float32).ToArray<float>());
            prevLayerSize = layerSize;
        }

        var outputWeights = np.random.randn(prevLayerSize, _outputSize) / Math.Sqrt(prevLayerSize);
        weightsList.AddRange(outputWeights.astype(np.float32).ToArray<float>());

        return weightsList.ToArray();
    }

    private float[] InitializeBiases()
    {
        var biasesList = new List<float>();

        foreach (var layerSize in _hiddenLayers)
        {
            var layerBiases = np.zeros(layerSize, dtype: np.float32);
            biasesList.AddRange(layerBiases.ToArray<float>());
        }

        var outputBiases = np.zeros(_outputSize, dtype: np.float32);
        biasesList.AddRange(outputBiases.ToArray<float>());

        return biasesList.ToArray();
    }

    public float[] Predict(float[] inputs)
    {
        var inputTensor = tf.constant(np.array(inputs).reshape(new Shape(1, -1))); // Ensure inputTensor is 2D with shape (1, inputSize)
        var layerInput = inputTensor;
        var incomingLayerSize = _inputSize;

        int weightIndex = 0;
        int biasIndex = 0;
        foreach (var layerSize in _hiddenLayers)
        {
            var endWeightIndex = weightIndex + incomingLayerSize * layerSize;
            var layerWeights = tf.constant(_weights.Skip(weightIndex).Take(incomingLayerSize * layerSize).ToArray());
            layerWeights = tf.reshape(layerWeights, new int[] { incomingLayerSize, layerSize });
            var layerBiases = tf.constant(_biases.Skip(biasIndex).Take(layerSize).ToArray());
            layerInput = tf.add(tf.matmul(layerInput, layerWeights), layerBiases);
            layerInput = tf.nn.relu(layerInput);
            weightIndex = endWeightIndex;
            biasIndex += layerSize;
            incomingLayerSize = layerSize;
        }

        var outputWeights = tf.constant(_weights.Skip(weightIndex).Take(incomingLayerSize * _outputSize).ToArray());
        outputWeights = tf.reshape(outputWeights, new int[] { incomingLayerSize, _outputSize });
        var outputBiases = tf.constant(_biases.Skip(biasIndex).Take(_outputSize).ToArray());
        var output = tf.add(tf.matmul(layerInput, outputWeights), outputBiases);

        // Normalize the output to be between -1 and 1 using tanh activation
        output = tf.nn.tanh(output);

        return output.ToArray<float>();
    }

    public float[] GetWeights()
    {
        return _weights;
    }

    public void SetWeights(float[] weights)
    {
        _weights = weights;
    }

    public float[] GetBiases()
    {
        return _biases;
    }

    public void SetBiases(float[] biases)
    {
        _biases = biases;
    }

    public static void SaveBrains(List<OrbBrain> brains, string filePath)
    {
        var brainDataList = brains.Select(brain => new BrainData
        {
            InputSize = brain.InputSize,
            OutputSize = brain.OutputSize,
            HiddenLayers = brain.HiddenLayers,
            Weights = brain.GetWeights(),
            Biases = brain.GetBiases()
        }).ToList();

        var json = JsonConvert.SerializeObject(brainDataList, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public static List<OrbBrain> LoadBrains(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var brainDataList = JsonConvert.DeserializeObject<List<BrainData>>(json);

        var brains = brainDataList.Select(brainData =>
        {
            var brain = new OrbBrain(brainData.InputSize, brainData.OutputSize, brainData.HiddenLayers);
            brain.SetWeights(brainData.Weights);
            brain.SetBiases(brainData.Biases);
            return brain;
        }).ToList();

        return brains;
    }

    private class BrainData
    {
        public int InputSize { get; set; }
        public int OutputSize { get; set; }
        public int[] HiddenLayers { get; set; }
        public float[] Weights { get; set; }
        public float[] Biases { get; set; }
    }
}