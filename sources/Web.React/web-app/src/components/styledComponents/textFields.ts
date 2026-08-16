import { TextField } from "@mui/material";
import { styled } from "@mui/material/styles";
import { SpacingEnum } from "../../lib/enums/spacingEnum";

const StyledTextField = styled(TextField)(({ theme }) => ({
  margin: theme.spacing(SpacingEnum.sm),

  "& .MuiOutlinedInput-root": {
    borderRadius: 8,

    "& input:-webkit-autofill, & input:-webkit-autofill:hover, & input:-webkit-autofill:focus, & input:-webkit-autofill:active":
      {
        WebkitBoxShadow: `0 0 0 1000px ${theme.palette.background.paper} inset !important`,
        WebkitTextFillColor: `${theme.palette.text.primary} !important`,
        caretColor: `${theme.palette.text.primary} !important`,
        backgroundColor: `${theme.palette.background.paper} !important`,
        transition: "background-color 99999s ease-in-out 0s",
      },
  },
}));

export default StyledTextField;
