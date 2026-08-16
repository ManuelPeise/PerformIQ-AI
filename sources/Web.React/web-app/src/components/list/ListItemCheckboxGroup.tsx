import React from "react";
import {
  StyledListItem,
  StyledListItemText,
} from "../styledComponents/listComponents";
import { Checkbox, FormControlLabel } from "@mui/material";

export type CheckboxProps = {
  label: string;
  checked: boolean;
  disabled?: boolean;
  onChange: (checked: boolean) => void;
};

interface IProps {
  groupName: string;
  primary: string;
  secondary?: string;
  checkboxGroupProps: CheckboxProps[];
}

const ListItemCheckboxGroup: React.FC<IProps> = (props) => {
  const { groupName, primary, secondary, checkboxGroupProps } = props;

  return (
    <StyledListItem divider key={groupName}>
      <StyledListItemText primary={primary} secondary={secondary} />
      {checkboxGroupProps.map((checkboxProps, key) => (
        <FormControlLabel
          key={key}
          control={
            <Checkbox
              key={`checkbox-${key}`}
              checked={checkboxProps.checked}
              onChange={(e) => checkboxProps.onChange(e.target.checked)}
              disabled={checkboxProps.disabled}
            />
          }
          label={checkboxProps.label}
        />
      ))}
    </StyledListItem>
  );
};

export default ListItemCheckboxGroup;
