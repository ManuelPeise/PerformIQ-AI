import React from "react";
import StyledBox from "../styledComponents/boxes";
import AppHeader from "./components/AppHeader";

interface IDefaultPageContainerProps {
  pageTitle: string;
  onLogout: () => void;
  children: React.ReactNode;
}

const DefaultPageContainer: React.FC<IDefaultPageContainerProps> = (props) => {
  const { pageTitle, onLogout, children } = props;

  return (
    <StyledBox
      sx={{
        width: "100%",
        padding: 0,
        margin: 0,
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
      }}
    >
      <StyledBox
        sx={{
          padding: 0,
          margin: 0,
          width: "100%",
          flex: 1,
        }}
      >
        <AppHeader onLogout={onLogout} pageTitle={pageTitle} />
      </StyledBox>
      <StyledBox sx={{ padding: 2 }}>{children}</StyledBox>
    </StyledBox>
  );
};

export default DefaultPageContainer;
