import isEqual from "lodash/isEqual";

export type ReducerAction<TModel> =
  | Partial<TModel>
  | ((s: TModel) => Partial<TModel>);

export function reducerFunction<TState>(
  state: TState,
  update: ReducerAction<TState>,
): TState {
  const stateUpdate = typeof update === "function" ? update(state) : update;

  const keys = Object.keys(stateUpdate) as Array<keyof TState>;

  const isChanged =
    state != null && keys.some((key) => !isEqual(state[key], stateUpdate[key]));

  if (!isChanged) {
    return state;
  }

  return stateUpdate ? { ...state, ...stateUpdate } : state;
}
