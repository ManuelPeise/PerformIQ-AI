import { Typography } from "@mui/material";
import { styled } from "@mui/material/styles";
import { SpacingEnum } from "../../lib/enums/spacingEnum";

const StyledCaption = styled(Typography)(({ theme }) => ({
  marginTop: theme.spacing(SpacingEnum.sm),
  marginBottom: theme.spacing(SpacingEnum.sm),
  color: theme.palette.text.secondary,
}));

export default StyledCaption;
