import { Card } from "@mui/material";
import { styled } from "@mui/material/styles";
import { SpacingEnum } from "../../lib/enums/spacingEnum";

const StyledCard = styled(Card)(({ theme }) => ({
  padding: theme.spacing(SpacingEnum.md),
  backgroundColor: theme.palette.background.paper,
}));

export default StyledCard;
