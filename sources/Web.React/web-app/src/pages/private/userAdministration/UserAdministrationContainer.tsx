import React from "react";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";
import type { IUserDataExportModel } from "./models/IUserDataExportModel";
import UserAdministration from "./UserAdministration";
import { performIq } from "../../../hooks/performIq";
import { SharedStateProvider } from "../../../components/context/SharedStateProvider";

interface IProps extends ILocalizationProps {}

export interface IUserManagementInitializationProps {
  users: IUserDataExportModel[];
  updateUserCallback: (user: IUserDataExportModel) => Promise<boolean>;
}

const UserAdministrationContainer: React.FC<IProps> = (props) => {
  const getUsersApi = performIq.createStatelessApi<
    IUserDataExportModel[],
    void
  >();

  const updateUsersApi = performIq.createStatelessApi<
    boolean,
    IUserDataExportModel
  >();

  const initializeAsync =
    React.useCallback(async (): Promise<IUserManagementInitializationProps> => {
      const [users] = await Promise.all([
        await getUsersApi.sendGetRequest({
          serviceUrl: "administration/userservice/getusers",
        }),
      ]);

      const updateUserCallback = async (
        user: IUserDataExportModel,
      ): Promise<boolean> => {
        const result = await updateUsersApi.sendPostRequest(
          {
            serviceUrl: "administration/userservice/updateuser",
          },
          user,
        );

        return result;
      };

      return {
        users,
        updateUserCallback,
      };
    }, []);

  const initializationProps =
    performIq.useComponentInitializationAsync<IUserManagementInitializationProps>(
      initializeAsync,
    );

  initializationProps.error &&
    console.error(
      "Error during UserAdministrationContainer initialization:",
      initializationProps.error,
    );

  if (initializationProps.error) {
    return <div>Error: {String(initializationProps.error)}</div>;
  }
  if (!initializationProps.isInitialized || initializationProps.isLoading) {
    return null;
  }

  const propsWithInitialization: IProps & IUserManagementInitializationProps = {
    ...props,
    ...initializationProps.props,
  };

  return (
    <SharedStateProvider>
      <UserAdministration {...propsWithInitialization} />
    </SharedStateProvider>
  );
};

export default UserAdministrationContainer;
