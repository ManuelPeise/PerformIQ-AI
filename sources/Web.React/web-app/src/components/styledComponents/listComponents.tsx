import {
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from "@mui/material";
import { styled } from "@mui/material/styles";

const StyledList = styled(List)(({ theme }) => ({
  width: "100%",
  padding: theme.spacing(2),
  margin: 0,
}));

const StyledListItem = styled(ListItem, {
  shouldForwardProp: (prop) => prop !== "divider",
})<{ divider?: boolean }>(({ theme, divider }) => ({
  width: "100%",
  padding: theme.spacing(2),
  margin: 0,
  backgroundColor: "transparent",
  border: "none",
  paddingBottom: theme.spacing(1),
  ...(divider && {
    borderBottom: `1px solid ${theme.palette.divider}`,
  }),
}));

const StyledListItemText = styled(ListItemText)(({ theme }) => ({
  padding: 0,
  margin: 0,
  backgroundColor: "transparent",
  border: `none`,
  primary: {
    fontSize: "1rem",
    fontWeight: 500,
    color: theme.palette.text.primary,
  },
  secondary: {
    fontSize: "0.875rem",
    fontWeight: 300,
    color: theme.palette.text.secondary,
  },
}));

const StyledListItemIcon = styled(ListItemIcon)(({ theme }) => ({
  color: theme.palette.text.primary,
  width: 40,
  height: 40,
}));

const StyledListItemButton = styled(ListItemButton)(() => ({
  margin: 0,
  padding: 0,
  backgroundColor: "transparent",
  border: "none",
}));

export {
  StyledListItem,
  StyledList,
  StyledListItemText,
  StyledListItemIcon,
  StyledListItemButton,
};
