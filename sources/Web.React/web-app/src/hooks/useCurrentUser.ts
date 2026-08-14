import React from "react";
import { CurrentUserContext } from "../components/context/CurrentUserContext";

export const useCurrentUser = () => {
  const context = React.useContext(CurrentUserContext);

  if (!context) {
    throw new Error("useCurrentUser must be used within a CurrentUserProvider");
  }
  return context;
};
