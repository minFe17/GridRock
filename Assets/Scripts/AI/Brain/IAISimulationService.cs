public interface IAISimulationService
{
    AISimulationState Simulate(in AIActionContext actionContext);
    AISimulationState SimulateCandidate(in AIActionContext actionContext, in IAIActionCandidate candidate);
}