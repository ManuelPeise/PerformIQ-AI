import React from "react";
import { Navigate, Outlet, useLocation } from "react-router-dom";
import StyledBox from "../styledComponents/boxes";
import { useAuthenticationContext } from "../../hooks/useAuthenticationContext";
import Sidebar from "./Sidebar";
import type { ILocalizationProps } from "../../lib/localization/withLocalization";

const sidebarMinWidth = 80;
const sidebarMaxWidth = 300;

interface IAuthorizedLayoutProps extends ILocalizationProps {}

const AuthorizedLayout: React.FC<IAuthorizedLayoutProps> = (props) => {
  const { currentUser } = useAuthenticationContext();

  const [sidebarExpanded, setSidebarExpanded] = React.useState<boolean>(true);

  if (!currentUser) {
    return (
      <Navigate to="/auth/login" state={{ from: useLocation() }} replace />
    );
  }

  const toggleSidebar = React.useCallback(() => {
    setSidebarExpanded((prev) => !prev);
  }, []);

  return (
    <StyledBox
      sx={{
        display: "flex",
        minHeight: "100vh",
        width: "100%",
        m: 0,
        p: 0,
      }}
    >
      <StyledBox
        sx={{
          width: sidebarExpanded ? sidebarMaxWidth : sidebarMinWidth,
          flexShrink: 0,
          transition: "width 10ms esease-in-out",
        }}
      >
        <Sidebar
          {...props}
          isExpanded={sidebarExpanded}
          toggleSidebar={toggleSidebar}
        />
      </StyledBox>

      <StyledBox
        sx={{
          flex: 1,
          minWidth: 0,
          minHeight: "100vh",
        }}
      >
        <Outlet />
      </StyledBox>
    </StyledBox>
  );
};

export default AuthorizedLayout;
