using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.StateTransitionSystem;
using Litium.Foundation.Security;
using System;
using System.Collections.Generic;

namespace Distancify.LitiumAddOns.Foundation.StateMachine
{
    public abstract class FiniteStateTransitionBuilder<TStateMachine, TCarrier, TState>
        where TStateMachine : FiniteStateMachine<TCarrier>
        where TCarrier : BaseCarrier<TCarrier>
        where TState : struct, IConvertible
    {
        private readonly TStateMachine _stateMachine;
        private readonly Dictionary<TState, State<TCarrier>> _states;
        private readonly TState _initialState;

        public FiniteStateTransitionBuilder(TStateMachine stateMachine, TState initialState)
        {
            _stateMachine = stateMachine;
            _initialState = initialState;
            _states = new Dictionary<TState, State<TCarrier>>();
        }

        public void AddInitialState()
        {
            _stateMachine.AddInitialState(new State<TCarrier>(GetShortFromState(_initialState), _initialState.ToString()));
        }

        public void AllowStateTransition(TState from, TState to)
        {
            _stateMachine.AddStateTransition(_states[from], _states[to], TransitionConditionsMet, true);
        }

        public void AllowStateTransitionByProgramOnly(TState from, TState to)
        {
            _stateMachine.AddStateTransition(_states[from], _states[to], TransitionConditionsMet, false);
        }

        public void BuildStates()
        {
            foreach (TState state in Enum.GetValues(typeof(TState)))
            {
                var stateId = GetShortFromState(state);
                _states.Add(state, new State<TCarrier>(stateId, state.ToString(), EntryAction, ExitAction));
            }

            SetStateTransitions();
        }

        public virtual void EntryAction(TCarrier carrier, State<TCarrier> currentState, SecurityToken token) { }

        public virtual void ExitAction(TCarrier carrier, State<TCarrier> currentState, SecurityToken token) { }

        public abstract bool TransitionConditionsMet(TCarrier carrier, State<TCarrier> startState, State<TCarrier> endState, SecurityToken token);

        public abstract void SetStateTransitions();

        private short GetShortFromState(TState state)
        {
            return (short) Convert.ToInt32(state);
        }
    }
}