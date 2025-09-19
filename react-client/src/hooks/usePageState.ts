import { useReducer } from 'react';

interface PageState {
  selectedCustomer: string;
  selectedEvent: string;
  selectedOrderType: string;
  showEditor: boolean;
  validationErrors: any[];
}

type PageAction = 
  | { type: 'SET_CUSTOMER'; payload: string }
  | { type: 'SET_EVENT'; payload: string }
  | { type: 'SET_ORDER_TYPE'; payload: string }
  | { type: 'TOGGLE_EDITOR'; payload?: boolean }
  | { type: 'SET_VALIDATION_ERRORS'; payload: any[] }
  | { type: 'CLEAR_VALIDATION_ERRORS' }
  | { type: 'RESET' };

const initialState: PageState = {
  selectedCustomer: '',
  selectedEvent: '',
  selectedOrderType: '',
  showEditor: false,
  validationErrors: []
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

  return {
    ...state,
    setCustomer: (customer: string) => dispatch({ type: 'SET_CUSTOMER', payload: customer }),
    setEvent: (event: string) => dispatch({ type: 'SET_EVENT', payload: event }),
    setOrderType: (orderType: string) => dispatch({ type: 'SET_ORDER_TYPE', payload: orderType }),
    toggleEditor: (show?: boolean) => dispatch({ type: 'TOGGLE_EDITOR', payload: show }),
    setValidationErrors: (errors: any[]) => dispatch({ type: 'SET_VALIDATION_ERRORS', payload: errors }),
    clearValidationErrors: () => dispatch({ type: 'CLEAR_VALIDATION_ERRORS' }),
    reset: () => dispatch({ type: 'RESET' })
  };
}