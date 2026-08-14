import { Button } from "@mui/material";
import { styled } from "@mui/material/styles";
import { SpacingEnum } from "../../lib/enums/spacingEnum";

const FloatingActionButton = styled(Button)(({ theme }) => ({
  margin: theme.spacing(SpacingEnum.sm),
  backgroundColor: theme.palette.primary.main,
  color: theme.palette.primary.contrastText,
  borderRadius: "8px",
  opacity: 0.7,
  "&:hover": {
    backgroundColor: theme.palette.primary.dark,
    opacity: 1,
  },
}));

const ConfirmButton = styled(Button)(({ theme }) => ({
  margin: theme.spacing(SpacingEnum.sm),
  padding: theme.spacing(SpacingEnum.xs, SpacingEnum.md),
  backgroundColor: theme.palette.primary.main,
  color: theme.palette.primary.contrastText,
  borderRadius: "8px",
  opacity: 0.7,
  "&:hover": {
    backgroundColor: theme.palette.primary.dark,
    opacity: 1,
  },
}));

const CancelButton = styled(Button)(({ theme }) => ({
  margin: theme.spacing(SpacingEnum.sm),
  padding: theme.spacing(SpacingEnum.xs, SpacingEnum.md),
  backgroundColor: theme.palette.text.secondary,
  color: theme.palette.text.primary,
  borderRadius: "8px",
  opacity: 0.7,
  "&:hover": {
    backgroundColor: theme.palette.primary.dark,
    opacity: 1,
  },
}));

export { FloatingActionButton, ConfirmButton, CancelButton };
