using System.Collections.Generic;

public interface IPathfinder<State, Transition>
{
	/// <summary>
	/// Should return a estimate of shortest distance. The estimate must me admissible (never overestimate)
	/// </summary>
	float Heuristic(State fromLocation, State toLocation);
	
	/// <summary>
	/// Return the legal moves from a state
	/// </summary>
	List<Transition> Expand(State position);

	/// <summary>
	/// Return the legal moves from a state considering steps
	/// </summary>
	List<Transition> ExpandMovement(State state, int steps);
	
	/// <summary>
	/// Return the cost between two adjecent locations for path purposes
	/// </summary>
	float PathCost(State fromLocation, Transition transition);

	/// <summary>
	/// Returns the new state after an transition has been applied
	/// </summary>
	State ApplyTransition(State location, Transition transition);

	/// <summary>
	/// Returns the new steps value after checking for terrain effect on state
	/// </summary>
	int StepsForState(State state, int steps);
}