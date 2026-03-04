public interface IAISimulationService
{
    AISimulationState Simulate(in AIActionContext actionContext);
}