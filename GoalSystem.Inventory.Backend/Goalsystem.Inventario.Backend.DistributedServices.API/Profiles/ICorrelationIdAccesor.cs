namespace GoalSystem.Inventario.Backend.API.Profiles
{
    public interface ICorrelationIdAccesor
    {
        /// <summary>
        /// Gets the scoped Correlation Id
        /// </summary>
        /// <returns></returns>
        string GetCorrelationId();
    }
}
