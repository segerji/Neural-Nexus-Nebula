using System;
using NNN.Entities.Orbs;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NNN.Systems;
using NNN.Systems.Services;

public class GeneticAlgorithm
{
    private readonly IDrawingService _drawingService;
    private readonly Rectangle _bounds;
    private readonly EventBus _eventBus;
    private readonly Texture2D _orbTexture;
    private readonly float _mutationRate;
    private readonly Random _random;

    public GeneticAlgorithm(float mutationRate, IDrawingService drawingService, Rectangle bounds, EventBus eventBus, Texture2D orbTexture)
    {
        _drawingService = drawingService;
        _bounds = bounds;
        _eventBus = eventBus;
        _orbTexture = orbTexture;
        _mutationRate = mutationRate;
        _random = new Random();
    }

    public List<AlienOrb> Evolve(List<AlienOrb> population)
    {
        // Evaluate fitness of each orb
        population.Sort((a, b) => b.KnowledgeScore.CompareTo(a.KnowledgeScore));

        // Check if any top performer has a KnowledgeScore over 20
        if (population.All(orb => orb.KnowledgeScore <= 50))
        {
            // Randomize all weights and biases again
            foreach (var orb in population)
            {
                orb.Brain = new OrbBrain(orb.Brain.InputSize, orb.Brain.OutputSize, orb.Brain.HiddenLayers);
            }
            return population;
        }

        // Select the top performers
        var topPerformers = population.Take(Convert.ToInt32(population.Count * 0.2)).ToList();

        // Create new generation
        var newPopulation = new List<AlienOrb>();

        for (int i = 0; i < population.Count; i++)
        {
            var parent1 = topPerformers[_random.Next(topPerformers.Count)];
            var parent2 = topPerformers[_random.Next(topPerformers.Count)];

            var childBrain = Crossover(parent1.Brain, parent2.Brain);
            Mutate(childBrain);

            var childOrb = new AlienOrb(_drawingService, _bounds, _eventBus)
            {
                Brain = childBrain
            };
            childOrb.Initialize(_orbTexture, null, parent1.Speed);

            newPopulation.Add(childOrb);
        }

        return newPopulation;
    }

    private OrbBrain Crossover(OrbBrain parent1, OrbBrain parent2)
    {
        var parent1Weights = parent1.GetWeights();
        var parent2Weights = parent2.GetWeights();

        var childWeights = new float[parent1Weights.Length];

        for (int i = 0; i < childWeights.Length; i++)
        {
            childWeights[i] = _random.NextDouble() < 0.5 ? parent1Weights[i] : parent2Weights[i];
        }

        var childBrain = new OrbBrain(parent1.InputSize, parent1.OutputSize, parent1.HiddenLayers);
        childBrain.SetWeights(childWeights);
        return childBrain;
    }

    private void Mutate(OrbBrain brain)
    {
        var weights = brain.GetWeights();

        for (int i = 0; i < weights.Length; i++)
        {
            if (_random.NextDouble() < _mutationRate)
            {
                weights[i] += (float)(_random.NextDouble() * 2 - 1); // Small random change
            }
        }

        brain.SetWeights(weights);
    }
}
