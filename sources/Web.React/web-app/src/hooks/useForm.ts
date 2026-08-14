import React from "react";
import { reducerFunction, type ReducerAction } from "../lib/reducer";
import isEqual from "lodash/isEqual";

type Subscription<TModel> = Partial<TModel>;

type SubscriptionCallback<TModel> = (state: TModel) => Subscription<TModel>;

type FormModel<TModel> = {
  originalModel: TModel;
  currentModel: TModel;
};

type UseFormResult<TModel> = {
  state: TModel;
  isModified: boolean;
  isValid: boolean;
  subscribe: (callback: SubscriptionCallback<TModel>) => Subscription<TModel>;
  handleChange: (update: ReducerAction<FormModel<TModel>>) => void;
  resetForm: () => void;
};

export const useForm = <TModel>(
  initialValues: TModel,
  validationCallback?: (state: TModel) => boolean,
): UseFormResult<TModel> => {
  const validationCallbackRef =
    validationCallback != null ? React.useRef(validationCallback) : null;

  const [state, dispatch] = React.useReducer(
    reducerFunction as React.Reducer<
      FormModel<TModel>,
      ReducerAction<FormModel<TModel>>
    >,
    { originalModel: initialValues, currentModel: initialValues },
  );

  const isModified = React.useMemo(() => {
    return isEqual(state.originalModel, state.currentModel) === false;
  }, [state.originalModel, state.currentModel]);

  const resetForm = React.useCallback(() => {
    dispatch({ currentModel: state.originalModel });
  }, [dispatch, state.originalModel]);

  const handleChange = React.useCallback(
    (update: ReducerAction<FormModel<TModel>>) => {
      dispatch(update);
    },
    [dispatch],
  );

  const subScribe = React.useCallback(
    (callback: SubscriptionCallback<TModel>): Subscription<TModel> => {
      return callback(state.currentModel);
    },
    [state.currentModel],
  );

  const isValid = React.useMemo(() => {
    if (
      validationCallbackRef != null &&
      validationCallbackRef.current != null
    ) {
      return validationCallbackRef.current(state.currentModel);
    }
    return true;
  }, [state.currentModel]);

  return {
    state: state.currentModel,
    isModified,
    isValid,
    subscribe: subScribe,
    handleChange,
    resetForm,
  };
};
