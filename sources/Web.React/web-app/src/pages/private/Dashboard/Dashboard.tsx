import React from "react";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";
import { useAuthenticationContext } from "../../../hooks/useAuthenticationContext";
import DefaultPageContainer from "../../../components/layouts/DefaultPageContainer";

interface IProps extends ILocalizationProps {}

const Dashboard: React.FC<IProps> = () => {
  const { currentUser, onLogoutUser } = useAuthenticationContext();

  return (
    <DefaultPageContainer onLogout={onLogoutUser} pageTitle="Dashboard">
      <div>
        <h1>Welcome, {currentUser?.email}!</h1>
        <p>This is your dashboard.</p>
      </div>
    </DefaultPageContainer>
  );
};

export default Dashboard;
