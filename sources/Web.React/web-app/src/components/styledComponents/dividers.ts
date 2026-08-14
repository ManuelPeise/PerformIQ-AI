import { Divider } from "@mui/material";
import { styled } from "@mui/material/styles";
import { SpacingEnum } from "../../lib/enums/spacingEnum";

const StyledDivider = styled(Divider)(({ theme }) => ({
  height: 1,
  backgroundColor: theme.palette.divider,
  border: "none",

  "&.MuiDivider-fullWidth": {
    margin: theme.spacing(SpacingEnum.sm, 0),
    width: "100%",
  },

  "&.MuiDivider-middle": {
    margin: theme.spacing(SpacingEnum.sm),
    width: "75%",
  },

  "&.MuiDivider-inset": {
    margin: theme.spacing(SpacingEnum.sm, 0, SpacingEnum.sm, 7),
    width: "50%",
  },
}));

export default StyledDivider;
