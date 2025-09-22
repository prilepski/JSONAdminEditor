import { useReducer, useCallback } from 'react';
import { ValidationError } from '../types';

interface PageState {
  selectedCustomer: string;
  selectedEvent: string;
  selectedOrderType: string;
  showEditor: boolean;
  validationErrors: ValidationError[];
}

type PageAction =
  | { type: 'SET_CUSTOMER'; payload: string }
  | { type: 'SET_EVENT'; payload: string }
  | { type: 'SET_ORDER_TYPE'; payload: string }
  | { type: 'TOGGLE_EDITOR'; payload?: boolean }
  | { type: 'SET_VALIDATION_ERRORS'; payload: ValidationError[] }
  | { type: 'CLEAR_VALIDATION_ERRORS' }
  | { type: 'RESET' };

const initialState: PageState = {
  selectedCustomer: '',
  selectedEvent: '',
  selectedOrderType: '',
  showEditor: false,
  validationErrors: [] as ValidationError[],
};

function pageReducer(state: PageState, action: PageAction): PageState {
  switch (action.type) {
    case 'SET_CUSTOMER':
      return { ...state, selectedCustomer: action.payload };
    case 'SET_EVENT':
      return { ...state, selectedEvent: action.payload };
    case 'SET_ORDER_TYPE':
      return { ...state, selectedOrderType: action.payload };
    case 'TOGGLE_EDITOR':
      return { ...state, showEditor: action.payload ?? !state.showEditor };
    case 'SET_VALIDATION_ERRORS':
      return { ...state, validationErrors: action.payload };
    case 'CLEAR_VALIDATION_ERRORS':
      return { ...state, validationErrors: [] };
    case 'RESET':
      return initialState;
    default:
      return state;
  }
}

export function usePageState() {
  const [state, dispatch] = useReducer(pageReducer, initialState);

  const setCustomer = useCallback(
    (customer: string) => dispatch({ type: 'SET_CUSTOMER', payload: customer }),
    []
  );
  const setEvent = useCallback(
    (event: string) => dispatch({ type: 'SET_EVENT', payload: event }),
    []
  );
  const setOrderType = useCallback(
    (orderType: string) => dispatch({ type: 'SET_ORDER_TYPE', payload: orderType }),
    []
  );
  const toggleEditor = useCallback(
    (show?: boolean) => dispatch({ type: 'TOGGLE_EDITOR', payload: show }),
    []
  );
  const setValidationErrors = useCallback(
    (errors: ValidationError[]) => dispatch({ type: 'SET_VALIDATION_ERRORS', payload: errors }),
    []
  );
  const clearValidationErrors = useCallback(
    () => dispatch({ type: 'CLEAR_VALIDATION_ERRORS' }),
    []
  );
  const reset = useCallback(() => dispatch({ type: 'RESET' }), []);

  return {
    ...state,
    setCustomer,
    setEvent,
    setOrderType,
    toggleEditor,
    setValidationErrors,
    clearValidationErrors,
    reset,
  };
}
