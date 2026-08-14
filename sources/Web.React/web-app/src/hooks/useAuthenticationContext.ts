import React from "react";
import { AuthenticationContext } from "../components/context/AuthContextProvider";

export const useAuthenticationContext = () => {
  const context = React.useContext(AuthenticationContext);
  if (!context) {
    throw new Error(
      "useAuthenticationContext must be used within an AuthContextProvider",
    );
  }
  return context;
};
