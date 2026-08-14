import React from "react";
import { Outlet } from "react-router-dom";
import { Box } from "@mui/material";
import backgroundImage from "../../assets/img/landing-page-background.jpg";

const UnauthorizedLayout: React.FC = () => {
  return (
    <Box
      sx={{
        width: "100%",
        height: "100%",
        display: "flex",
        flexDirection: "column",
        backgroundImage: `url(${backgroundImage})`,
        backgroundSize: "cover",
        backgroundPosition: "center",
        overflow: "hidden",
      }}
    >
      <Outlet />
    </Box>
  );
};

export default UnauthorizedLayout;
