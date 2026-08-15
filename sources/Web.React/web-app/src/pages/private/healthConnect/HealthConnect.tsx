import React from "react";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";
import DefaultPageContainer from "../../../components/layouts/DefaultPageContainer";
import { useAuthenticationContext } from "../../../hooks/useAuthenticationContext";
import StyledPaper from "../../../components/styledComponents/paper";
import { StyledList } from "../../../components/styledComponents/listComponents";

interface IHealthConnectProps extends ILocalizationProps {}

const HealthConnect: React.FC<IHealthConnectProps> = (props) => {
  const { onLogoutUser } = useAuthenticationContext();
  return (
    <DefaultPageContainer pageTitle="Health Connect" onLogout={onLogoutUser}>
      <StyledPaper sx={{ width: "100%" }}>
        <StyledList></StyledList>
      </StyledPaper>
    </DefaultPageContainer>
  );
};

export default HealthConnect;
