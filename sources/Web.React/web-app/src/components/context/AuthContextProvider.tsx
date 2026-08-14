import React, { type PropsWithChildren } from "react";
import type { IAuthenticationContext } from "./types/IAuthenticationContext";
import { performIq } from "../../hooks/performIq";
import type { IAuthResponseModel } from "../../lib/types/auth/IAuthResponseModel";
import type { ILoginRequestModel } from "../../lib/types/auth/ILoginRequestModel";
import type { IRegisterRequestModel } from "../../lib/types/auth/IRegisterRequestModel";
import { useCurrentUser } from "../../hooks/useCurrentUser";

export const AuthenticationContext =
  React.createContext<IAuthenticationContext>({
    currentUser: null,
    onAuthenticateUser: async () => false,
    onRegisterUser: async () => false,
    onLogoutUser: async () => {
      return Promise.resolve();
    },
  });

type AuthenticationContextProps = PropsWithChildren;

const AuthenticationContextProvider: React.FC<AuthenticationContextProps> = (
  props,
) => {
  const { children } = props;

  const authenticationApi = performIq.createStatelessApi<
    IAuthResponseModel,
    ILoginRequestModel
  >();

  const registerApi = performIq.createStatelessApi<
    boolean,
    IRegisterRequestModel
  >();

  const {
    currentUser,
    updateJwtToken,
    assignCurrentUser,
    unassignCurrentUser,
  } = useCurrentUser();

  const onAuthenticateUser = React.useCallback(
    async (loginRequest: ILoginRequestModel): Promise<boolean> => {
      const response = await authenticationApi.sendPostRequest(
        {
          serviceUrl: "auth/login",
        },
        loginRequest,
      );

      if (response?.accessToken) {
        updateJwtToken(response.accessToken);

        await assignCurrentUser();
      }

      return Boolean(response && response.accessToken);
    },
    [authenticationApi, updateJwtToken, assignCurrentUser],
  );

  const onRegisterUser = React.useCallback(
    async (registerRequest: IRegisterRequestModel): Promise<boolean> => {
      const response = await registerApi.sendPostRequest(
        {
          serviceUrl: "auth/register",
        },
        registerRequest,
      );
      return Boolean(response);
    },
    [registerApi],
  );

  const onLogoutUser = React.useCallback(async () => {
    await unassignCurrentUser();
  }, [unassignCurrentUser]);

  return (
    <AuthenticationContext.Provider
      value={{
        currentUser,
        onAuthenticateUser: onAuthenticateUser,
        onRegisterUser: onRegisterUser,
        onLogoutUser: onLogoutUser,
      }}
    >
      {children}
    </AuthenticationContext.Provider>
  );
};

export default AuthenticationContextProvider;
