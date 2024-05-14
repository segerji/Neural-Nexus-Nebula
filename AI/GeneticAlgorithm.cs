using System;
using NNN.Entities.Orbs;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NNN.Systems;
using NNN.Systems.Services;
using System.IO;

public class GeneticAlgorithm
{
    private readonly IDrawingService _drawingService;
    private readonly Rectangle _bounds;
    private readonly EventBus _eventBus;
    private readonly Texture2D _orbTexture;
    private readonly Texture2D _bestPerformingAlienOrbTexture;
    private readonly Texture2D _thirdGenerationAlienOrbTexture;
    private readonly float _mutationRate;
    private readonly Random _random;
    private readonly string _saveFilePath = "top_performers_brains.json";

    public GeneticAlgorithm(float mutationRate, IDrawingService drawingService, Rectangle bounds, EventBus eventBus, Texture2D orbTexture, Texture2D bestPerformingAlienOrbTexture, Texture2D thirdGenerationAlienOrbTexture)
    {
        _drawingService = drawingService;
        _bounds = bounds;
        _eventBus = eventBus;
        _orbTexture = orbTexture;
        _bestPerformingAlienOrbTexture = bestPerformingAlienOrbTexture;
        _thirdGenerationAlienOrbTexture = thirdGenerationAlienOrbTexture;
        _mutationRate = mutationRate;
        _random = new Random();
    }

    public List<AlienOrb> Evolve(List<AlienOrb> population)
    {
        // Evaluate fitness of each orb
        population.Sort((a, b) => b.KnowledgeScore.CompareTo(a.KnowledgeScore));

        // Select the top performers
        var topPerformers = population.Take(4).ToList();

        // Save the top 4 performers' brains
        var topBrains = topPerformers.Select(p => p.Brain).ToList();
        OrbBrain.SaveBrains(topBrains, _saveFilePath);

        // Create new generation
        var newPopulation = new List<AlienOrb>();

        // Elitism: keep the top 4 performers as is
        foreach (var topPerformer in topPerformers)
        {
            var newTopPerformer = new AlienOrb(_drawingService, _bounds, _eventBus)
            {
                Brain = topPerformer.Brain
            };

            if(topPerformer.Texture != _orbTexture)
            {
                newTopPerformer.Initialize(_thirdGenerationAlienOrbTexture, null);
            }
            else
            {
                newTopPerformer.Initialize(_bestPerformingAlienOrbTexture, null);
            }
            
            newPopulation.Add(newTopPerformer);
        }

        // Fill the rest of the new population
        for (int i = 0; i < (population.Count - 4); i++)
        {
            var parent1 = topPerformers[_random.Next(topPerformers.Count)];
            var parent2 = topPerformers[_random.Next(topPerformers.Count)];

            var childBrain = Crossover(parent1.Brain, parent2.Brain);
            Mutate(childBrain);

            var childOrb = new AlienOrb(_drawingService, _bounds, _eventBus)
            {
                Brain = childBrain
            };
            childOrb.Initialize(_orbTexture, null);

            newPopulation.Add(childOrb);
        }

        return newPopulation;
    }

    private OrbBrain Crossover(OrbBrain parent1, OrbBrain parent2)
    {
        var parent1Weights = parent1.GetWeights();
        var parent2Weights = parent2.GetWeights();
        var parent1Biases = parent1.GetBiases();
        var parent2Biases = parent2.GetBiases();

        var childWeights = new float[parent1Weights.Length];
        var childBiases = new float[parent1Biases.Length];

        for (int i = 0; i < childWeights.Length; i++)
        {
            childWeights[i] = _random.NextDouble() < 0.5 ? parent1Weights[i] : parent2Weights[i];
        }

        for (int i = 0; i < childBiases.Length; i++)
        {
            childBiases[i] = _random.NextDouble() < 0.5 ? parent1Biases[i] : parent2Biases[i];
        }

        var childBrain = new OrbBrain(parent1.InputSize, parent1.OutputSize, parent1.HiddenLayers);
        childBrain.SetWeights(childWeights);
        childBrain.SetBiases(childBiases);
        return childBrain;
    }

    private void Mutate(OrbBrain brain)
    {
        var weights = brain.GetWeights();
        var biases = brain.GetBiases();

        for (int i = 0; i < weights.Length; i++)
        {
            if (_random.NextDouble() < _mutationRate)
            {
                weights[i] += (float)(_random.NextDouble() * 2 - 1); // Small random change
            }
        }

        for (int i = 0; i < biases.Length; i++)
        {
            if (_random.NextDouble() < _mutationRate)
            {
                biases[i] += (float)(_random.NextDouble() * 2 - 1); // Small random change
            }
        }

        brain.SetWeights(weights);
        brain.SetBiases(biases);
    }

    public List<AlienOrb> LoadTopPerformers()
    {
        if (File.Exists(_saveFilePath))
        {
            var topBrains = OrbBrain.LoadBrains(_saveFilePath);
            var topPerformers = topBrains.Select(brain =>
            {
                var orb = new AlienOrb(_drawingService, _bounds, _eventBus)
                {
                    Brain = brain
                };
                orb.Initialize(_bestPerformingAlienOrbTexture, null);
                return orb;
            }).ToList();

            return topPerformers;
        }
        return null;
    }
}
