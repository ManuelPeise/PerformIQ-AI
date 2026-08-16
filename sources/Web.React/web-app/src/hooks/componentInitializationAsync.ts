import React from "react";

export type ComponentInitializationState<TModel> = {
  isInitialized: boolean;
  isLoading: boolean;
  props: TModel;
  error: unknown | null;
};

const useComponentInitialization = <TModel>(
  callback: () => Promise<TModel>,
): ComponentInitializationState<TModel> => {
  const [state, setState] = React.useState<
    ComponentInitializationState<TModel>
  >({
    isInitialized: false,
    isLoading: true,
    props: {} as TModel,
    error: null,
  });

  React.useEffect(() => {
    let cancelled = false;

    const initialize = async () => {
      try {
        const model = await callback();

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
          props: {} as TModel,
          error,
        });
      }
    };

    void initialize();

    return () => {
      cancelled = true;
    };
  }, [callback]);

  return state;
};

export default useComponentInitialization;
