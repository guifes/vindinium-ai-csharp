using System.Collections.Generic;

public interface IPathfinder<State, Transition>
{
	float Heuristic(State fromLocation, State toLocation);
	List<Transition> Expand(State position, State toState);
	double PathCost(State fromLocation, Transition transition);
	State ApplyTransition(State location, Transition transition);
}