import React from "react";
import type { CheckboxProps } from "./ListItemCheckboxGroup";
import {
  StyledListItem,
  StyledListItemText,
} from "../styledComponents/listComponents";
import { Checkbox } from "@mui/material";

interface IProps {
  primary: string;
  secondary?: string;
  checkboxProps: CheckboxProps;
}

const ListItemCheckbox: React.FC<IProps> = (props) => {
  const { primary, secondary, checkboxProps } = props;

  return (
    <StyledListItem divider>
      <StyledListItemText primary={primary} secondary={secondary} />
      <Checkbox
        checked={checkboxProps.checked}
        onChange={(e) => checkboxProps.onChange(e.target.checked)}
        disabled={checkboxProps.disabled}
      />
    </StyledListItem>
  );
};

export default ListItemCheckbox;
