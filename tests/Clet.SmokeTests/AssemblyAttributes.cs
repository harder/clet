using Xunit;

// Smoke tests shell out to a shared test-copied clet binary. Keep process-level
// cases serial so child processes do not race over the same output artifacts.
[assembly: CollectionBehavior (DisableTestParallelization = true)]
