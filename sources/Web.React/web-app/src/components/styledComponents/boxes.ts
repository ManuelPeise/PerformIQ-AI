import { Box } from "@mui/material";
import { styled } from "@mui/material/styles";

const StyledBox = styled(Box)(() => ({
  width: "100%",
  height: "100%",
  padding: 0,
  backgroundColor: "transparent",
  border: `none`,
  display: "flex",
}));

export default StyledBox;
