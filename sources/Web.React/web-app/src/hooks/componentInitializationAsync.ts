import React from "react";

export type ComponentInitializationState<TModel> = {
  isInitialized: boolean;
  isLoading: boolean;
  props: TModel | null;
  error: unknown | null;
};

const useComponentInitialization = <TModel>(
  callback: () => Promise<TModel>,
): ComponentInitializationState<TModel> => {
  const callbackRef = React.useRef(callback);

  React.useEffect(() => {
    callbackRef.current = callback;
  }, [callback]);

  const [state, setState] = React.useState<
    ComponentInitializationState<TModel>
  >({
    isInitialized: false,
    isLoading: true,
    props: null,
    error: null,
  });

  React.useEffect(() => {
    let cancelled = false;

    const initialize = async () => {
      try {
        const model = await callbackRef.current();

        if (cancelled) {
          return;
        }

        setState({
          isInitialized: true,
          isLoading: false,
          props: model,
          error: null,
        });
      } catch (error) {
        if (cancelled) {
          return;
        }

        setState({
          isInitialized: false,
          isLoading: false,
          props: null,
          error,
        });
      }
    };

    void initialize();

    return () => {
      cancelled = true;
    };
  }, []);

  return state;
};

export default useComponentInitialization;
