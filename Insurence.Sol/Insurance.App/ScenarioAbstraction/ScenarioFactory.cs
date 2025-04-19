namespace Insurance.App.Scenario;

public class ScenarioFactory
{
    private readonly Dictionary<UserState, IScenario> _scenarios;

    public ScenarioFactory(IEnumerable<IScenario> scenarios)
    {
        _scenarios = scenarios.ToDictionary(s => s.State);
    }

    public IScenario? GetScenario(UserState state)
        => _scenarios.TryGetValue(state, out var scenario) ? scenario : null;
}
