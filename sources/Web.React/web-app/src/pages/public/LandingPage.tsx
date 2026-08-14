import React from "react";
import {
  withLocalization,
  type ILocalizationProps,
} from "../../lib/localization/withLocalization";
import StyledBox from "../../components/styledComponents/boxes";
import StyledCard from "../../components/styledComponents/cards";
import StyledCaption from "../../components/styledComponents/typography";
import StyledDivider from "../../components/styledComponents/dividers";
import { FloatingActionButton } from "../../components/styledComponents/buttons";
import { useNavigate } from "react-router-dom";

interface IProps extends ILocalizationProps {}

const LandingPage: React.FC<IProps> = (props) => {
  const { getResource } = props;

  const navigation = useNavigate();

  const handleGetStartedClick = React.useCallback(() => {
    navigation("/auth/login");
  }, [navigation]);

  return (
    <StyledBox
      sx={{ display: "flex", justifyContent: "center", alignItems: "center" }}
    >
      <StyledCard sx={{ margin: 0, padding: 4, opacity: 0.8 }}>
        <StyledBox
          sx={{
            flexDirection: "column",
            justifyContent: "center",
            alignItems: "center",
          }}
        >
          <StyledCaption variant="h1">
            {getResource("common.labelAppName")}
          </StyledCaption>
          <StyledDivider variant="inset" />
          <StyledCaption variant="h2">
            {getResource("common.labelAppHero")}
          </StyledCaption>
          <StyledDivider variant="middle" />
          <StyledCaption variant="h4">
            {getResource("common.descriptionAppHero")}
          </StyledCaption>
        </StyledBox>
        <StyledBox
          sx={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
          }}
        >
          <FloatingActionButton onClick={handleGetStartedClick}>
            {getResource("common.labelGetStarted")}
          </FloatingActionButton>
        </StyledBox>
      </StyledCard>
    </StyledBox>
  );
};

export default withLocalization(["common"], LandingPage);
