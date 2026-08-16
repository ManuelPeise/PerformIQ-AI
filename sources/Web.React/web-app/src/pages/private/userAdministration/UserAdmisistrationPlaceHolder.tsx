import React from "react";
import type { ILocalizationProps } from "../../../lib/localization/withLocalization";
import StyledBox from "../../../components/styledComponents/boxes";
import { Typography } from "@mui/material";

interface IProps extends ILocalizationProps {}

const UserAdministrationPlaceHolder: React.FC<IProps> = (props) => {
  const { getResource } = props;

  return (
    <StyledBox
      sx={{
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
        alignItems: "center",
        height: "100%",
      }}
    >
      <Typography variant="h4" gutterBottom>
        {getResource("common.titleNoUserSelected")}
      </Typography>
      <Typography variant="body1">
        {getResource("common.subTitleNoUserSelected")}
      </Typography>
    </StyledBox>
  );
};

export default UserAdministrationPlaceHolder;
