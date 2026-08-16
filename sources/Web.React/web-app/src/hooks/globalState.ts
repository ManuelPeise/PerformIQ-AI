import React from "react";
import isEqual from "lodash/isEqual";

type GlobalStateSubscription<TModel> = Partial<TModel>;
type GlobalStateSubscriptionCallback<TModel> = (
  state: TModel,
) => GlobalStateSubscription<TModel>;

export type GlobalStateResult<TModel> = {
  isModifiedState: boolean;
  useModel: (model: TModel) => void;
  update: (partialState: Partial<TModel>) => void;
  subscribe: (
    callback: GlobalStateSubscriptionCallback<TModel>,
  ) => GlobalStateSubscription<TModel>;
  revertChanges: () => void;
};

export const createGlobalState = <TModel>(): GlobalStateResult<TModel> => {
  const originalStateRef = React.useRef<TModel | null>(null);
  const [state, setState] = React.useState<TModel | null>(null);

  const useModel = React.useCallback((model: TModel) => {
    originalStateRef.current = model;
    setState(model);
  }, []);

  const isModifiedState = React.useMemo(() => {
    return !isEqual(originalStateRef.current, state);
  }, [state]);

  const update = React.useCallback(
    (partialState: Partial<TModel>) => {
      if (state === null) {
        throw new Error(
          "State is null. Please initialize the state using useModel before updating.",
        );
      }
      setState({ ...state, ...partialState });
    },
    [state],
  );

  const subscribe = React.useCallback(
    (
      callback: GlobalStateSubscriptionCallback<TModel>,
    ): GlobalStateSubscription<TModel> => {
      if (state === null) {
        throw new Error(
          "State is null. Please initialize the state using useModel before subscribing.",
        );
      }
      const result = callback(state);

      return result;
    },
    [state],
  );

  const revertChanges = React.useCallback(() => {
    if (originalStateRef.current !== null) {
      setState(originalStateRef.current);
    }
  }, []);
  return { isModifiedState, useModel, update, subscribe, revertChanges };
};
