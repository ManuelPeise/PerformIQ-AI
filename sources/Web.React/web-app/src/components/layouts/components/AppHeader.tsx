import React from "react";
import { IconButton, Paper, Typography } from "@mui/material";
import LogoutIcon from "@mui/icons-material/Logout";
import StyledBox from "../../styledComponents/boxes";

interface IAppBarProps {
  onLogout: () => void;
  pageTitle: string;
}

const AppHeader: React.FC<IAppBarProps> = (props) => {
  const { onLogout, pageTitle } = props;

  return (
    <StyledBox
      sx={{
        display: "flex",
        justifyContent: "flex-end",
        width: "100%",
        height: "64px",
      }}
    >
      <Paper
        elevation={3}
        sx={{
          display: "flex",
          flexDirection: "row",
          alignItems: "center",
          padding: 2,
          margin: 0,
          width: "100%",
          borderRadius: 0,
        }}
      >
        <StyledBox>
          <Typography variant="h6">{pageTitle}</Typography>
        </StyledBox>
        <StyledBox sx={{ flex: 1 }} />
        <StyledBox
          sx={{ display: "flex", justifyContent: "flex-end", width: "100%" }}
        >
          <IconButton onClick={onLogout}>
            <LogoutIcon sx={{ width: 24, height: 24 }} />
          </IconButton>
        </StyledBox>
      </Paper>
    </StyledBox>
  );
};

export default AppHeader;
