import React from "react";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";
import VerticalTabPage from "../../../components/layouts/VerticalTabPage";
import DefaultPageContainer from "../../../components/layouts/DefaultPageContainer";
import { useAuthenticationContext } from "../../../hooks/useAuthenticationContext";
import Dashboard from "../dashboard/Dashboard";

interface IUserAdministrationProps extends ILocalizationProps {}

const UserAdministration: React.FC<IUserAdministrationProps> = (props) => {
  const { onLogoutUser } = useAuthenticationContext();
  return (
    <DefaultPageContainer
      onLogout={onLogoutUser}
      pageTitle="User Administration"
    >
      <VerticalTabPage
        getResource={props.getResource}
        items={[
          {
            title: "User Management",
            subtitle: "Manage users and their permissions",
            component: Dashboard,
          },
          {
            title: "User Management",
            subtitle: "Manage users and their permissions",
            component: () => <div>B</div>,
          },
          {
            title: "User Management",
            subtitle: "Manage users and their permissions",
            component: () => <div>C</div>,
          },
        ]}
      />
    </DefaultPageContainer>
  );
};

export default UserAdministration;
