import React from "react";
import StyledBox from "../styledComponents/boxes";
import { Link } from "react-router-dom";

interface IProps {
  text: string;
  linkText: string;
  href: string;
}

const FormLink: React.FC<IProps> = (props) => {
  const { text, linkText, href } = props;

  return (
    <StyledBox
      sx={{
        display: "flex",
        flexDirection: "row",
        justifyContent: "center",
        alignItems: "center",
        color: "inherit",
        gap: "4px",
      }}
    >
      <span>{linkText}</span>
      <Link
        to={href}
        style={{
          textDecoration: "none",
          color: "inherit",
        }}
      >
        <strong>{text}</strong>
      </Link>
    </StyledBox>
  );
};

export default FormLink;
