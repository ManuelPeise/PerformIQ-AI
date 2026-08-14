import React from "react";
import type { ICurrentUserContext } from "../../lib/types/auth/ICurrentUserContext";
import type { ICurrentUser } from "../../lib/types/auth/ICurrentUser";
import { performIq } from "../../hooks/performIq";

export const CurrentUserContext = React.createContext<ICurrentUserContext>({
  currentUser: null,
  jwtToken: null,
  assignCurrentUser: async () => Promise.resolve(),
  updateJwtToken: () => {},
  unassignCurrentUser: () => {},
});

const CurrentUserProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const currentUserApi = performIq.createStatelessApi<ICurrentUser, void>();

  const [currentUser, setCurrentUser] =
    React.useState<ICurrentUserContext["currentUser"]>(null);
  const [jwtToken, setJwtToken] = React.useState<string | null>(null);

  const updateJwtToken = React.useCallback((token: string) => {
    currentUserApi.updateJwtToken(token);
    setJwtToken(token);
  }, []);

  const assignCurrentUser = React.useCallback(async () => {
    const currentUser = await currentUserApi.sendGetRequest({
      serviceUrl: "auth/getcurrentuser",
    });

    setCurrentUser(currentUser);
  }, [currentUserApi]);

  const unassignCurrentUser = React.useCallback(() => {
    setCurrentUser(null);
  }, []);

  return (
    <CurrentUserContext.Provider
      value={{
        currentUser,
        jwtToken,
        assignCurrentUser,
        updateJwtToken,
        unassignCurrentUser,
      }}
    >
      {children}
    </CurrentUserContext.Provider>
  );
};

export default CurrentUserProvider;
