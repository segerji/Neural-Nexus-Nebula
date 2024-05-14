using NNN;
using System;

Environment.SetEnvironmentVariable("OMP_NUM_THREADS", "24"); // Set to the number of virtual cores
Environment.SetEnvironmentVariable("TF_NUM_INTRAOP_THREADS", "24");
Environment.SetEnvironmentVariable("TF_NUM_INTEROP_THREADS", "2"); // Adjust based on your workload


using var game = new NeuralNexusNebula();
game.Run();