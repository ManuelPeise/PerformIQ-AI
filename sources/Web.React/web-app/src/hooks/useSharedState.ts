import React from "react";
import {
  SharedStateContext,
  type SharedStateContextType,
} from "../components/context/SharedStateProvider";

export const useSharedState = <TModel>() => {
  const context = React.useContext<SharedStateContextType<TModel> | null>(
    SharedStateContext,
  );

  if (!context) {
    throw new Error("useSharedState must be used inside SharedStateProvider");
  }

  return context;
};
