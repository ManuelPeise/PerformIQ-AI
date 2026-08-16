import React from "react";
import { isEqual } from "lodash";
import type { PropsWithChildren } from "react";

export type SharedStateContextType<TModel> = {
  data: TModel | null;
  isDirty: boolean;

  useModel: (model: TModel | null) => void;
  resetModel: () => void;

  updateState: (partialState: Partial<TModel>) => void;

  updateArrayItem: <TKey extends keyof TModel>(
    index: number,
    key: TKey,
    partialArrayItem: Partial<TModel>,
  ) => void;
};

const SharedStateContext =
  React.createContext<SharedStateContextType<any> | null>(null);

type SharedStateProviderProps = PropsWithChildren;

const SharedStateProvider: React.FC<SharedStateProviderProps> = ({
  children,
}) => {
  const originalStateRef = React.useRef<any | null>(null);

  const [contextState, setContextState] = React.useState<any | null>(null);

  const useModel = React.useCallback(
    (model: any | null) => {
      originalStateRef.current = model;
      setContextState(model);
    },
    [originalStateRef],
  );

  const resetModel = React.useCallback(() => {
    setContextState(originalStateRef.current);
  }, []);

  const updateState = React.useCallback(
    (partialState: Partial<any>) => {
      setContextState(() => {
        if (contextState === null) {
          return contextState;
        }

        return {
          ...contextState,
          ...partialState,
        };
      });
    },
    [contextState],
  );

  const updateArrayItem = React.useCallback(
    <TKey extends keyof any>(
      index: number,
      key: TKey,
      partialArrayItem: Partial<any>,
    ) => {
      setContextState(() => {
        if (!Array.isArray(contextState[key])) {
          return contextState;
        }

        if (index < 0 || index >= contextState.length) {
          return contextState;
        }

        const updatedArray = [...contextState];

        updatedArray[index] = {
          ...updatedArray[index],
          ...partialArrayItem,
        };

        return updatedArray;
      });
    },
    [contextState],
  );

  const isDirty = React.useMemo(() => {
    return !isEqual(originalStateRef.current, contextState);
  }, [contextState]);

  return (
    <SharedStateContext.Provider
      value={{
        data: contextState,
        isDirty,
        useModel,
        resetModel,
        updateState,
        updateArrayItem,
      }}
    >
      {children}
    </SharedStateContext.Provider>
  );
};

export { SharedStateContext, SharedStateProvider };
